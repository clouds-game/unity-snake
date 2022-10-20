using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {
  LinkedList<Transform> bodies = new LinkedList<Transform>();
  Transform joint_base;
  LineRenderer body_renderer;
  AnimationCurve body_width_curve = new AnimationCurve();
  Vector3[] body_points = new Vector3[] {};
  BoardBehaviourScript board;
  Rigidbody2D head;
  Transform tail;
  Vector3 tail_tail;
  Vector3 last_head;
  int body_length = 5;
  int season_length_start = 5;
  int season_eated = 0;
  int season_not_eated = 0;
  int season_expanded_start = 0;
  int expanded = 0;
  float last_expanded;
  float body_size {
    get { return transform.localScale.x; }
    set {
      transform.localScale = Vector3.one * value;
      if (joint_base != null)
        joint_base.localScale = Vector3.one * value;
      if (tail != null)
        tail.localScale = Vector3.one * value;
    }
  }
  float speed = 5.0f;

  // Start is called before the first frame update
  void Start() {
    joint_base = transform.parent.Find("JointBase"); joint_base.gameObject.SetActive(false);
    body_renderer = transform.parent.Find("Body").GetComponent<LineRenderer>();
    tail = transform.parent.Find("Tail"); tail.gameObject.SetActive(false);
    board = transform.parent.parent.gameObject.GetComponent<BoardBehaviourScript>();
    Debug.Log($"Hello world {board}! now: {Time.time}");
    head = GetComponent<Rigidbody2D>();
    body_size = 0.3f;
  }

  static float getRotation(Vector2 direction) {
    var angle = Vector2.Angle(Vector2.right, (Vector2)direction);
    if (direction.y < 0) {
      angle = 360 - angle;
    }
    return angle;
  }

  // Update is called once per frame
  void Update() {
    bool shouldUpdate = false;
    var currentSpeed = speed;
    var direction = Vector3.zero;
    if (Input.GetKeyDown("z") && reverseBody()) { return; }
    if (Input.GetKey("w")) { direction += Vector3.up; }
    if (Input.GetKey("s")) { direction += Vector3.down; }
    if (Input.GetKey("a")) { direction += Vector3.left; }
    if (Input.GetKey("d")) { direction += Vector3.right; }
    if (Input.GetKey(KeyCode.LeftShift)) { currentSpeed *= 2.0f; }
    if (direction != Vector3.zero) {
      direction = direction.normalized;
      // transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
      head.rotation = getRotation(direction); // since the is sprite is left
      board.moved = true;
    }
    head.velocity = direction * currentSpeed;
    head.angularVelocity = 0;
    if (Vector3.Distance(transform.position, last_head) > 0.2) {
      shouldUpdate = true;
      last_head = transform.position;
    }

    if (shouldExpand()) {
      shouldUpdate = true;
      Expand();
      if (board.season == Season.Autumn &&
          ((body_length < 30 && season_not_eated >= 5) ||
          (body_length >= 30 && season_not_eated >= body_length * 2 / 3))) {
        body_length += 1;
        season_not_eated = 0;
      }
      if (bodies.Count > body_length) {
        season_not_eated += 1;
      }
    }
    while (bodies.Count > body_length && bodies.Count > 0) {
      shouldUpdate = true;
      tail_tail = bodies.Last.Value.GetComponent<BodyBehaviourScript>().keyPoint;
      Destroy(bodies.Last.Value.gameObject);
      bodies.RemoveLast();
    }

    if (shouldUpdate) {
      if (bodies.Last != null) {
        tail.position = bodies.Last.Value.position;
        tail.rotation = Quaternion.FromToRotation(Vector3.right, tail.position-tail_tail);
        tail.gameObject.SetActive(true);
      } else {
        tail.gameObject.SetActive(false);
      }
    }
    // if (tail_tail != null)
    //   Debug.DrawLine(tail.position, tail_tail, Color.red, 0.1f, false);

    if (!board.stuck && shouldStuck()) {
      board.stuck = true;
      Debug.Log("maybe stucking...");
    }

    checkSeason();

    if (body_length <= 0) {
      board.win = true;
    }

    if (shouldUpdate) {
      int segments = 5;
      updatePoints(segments);
      body_renderer.positionCount = body_points.Length;
      body_renderer.SetPositions(body_points);
      body_renderer.widthCurve = body_width_curve;
      // body_renderer.material = body_renderer.GetComponent<SpriteRenderer>().material;

      var edge_center = new Vector2[] {};
      if (body_points.Length > 3*segments) {
        edge_center = System.Array.ConvertAll(body_points[(2*segments)..^segments], v => (Vector2)v);
      }
      var edge_points = new Vector2[edge_center.Length*4];
      for (int i = 0, n = edge_center.Length; i < n; i++) {
        var scale = body_size * 0.3f;
        edge_points[i] = edge_center[i] + Vector2.up * scale;
        edge_points[n*2 - i - 1] = edge_center[i] + Vector2.down * scale;
        edge_points[n*2 + i] = edge_center[i] + Vector2.left * scale;
        edge_points[n*4 - i - 1] = edge_center[i] + Vector2.right * scale;
      }
      body_renderer.GetComponent<EdgeCollider2D>().points = edge_points;
      body_renderer.GetComponent<EdgeCollider2D>().isTrigger = edge_points.Length == 0;
      // Debug.Log($"updated: {body_points.Length} points");
    }

    // for (var p = bodies.First; p != null && p.Next != null; p = p.Next) {
    //   Debug.DrawLine(p.Value.GetComponent<BodyBehaviourScript>().keyPoint, p.Next.Value.GetComponent<BodyBehaviourScript>().keyPoint, Color.white, 0.1f, false);
    // }
    // for (var i = 0; i < body_points.Length-1; i++) {
    //   Debug.DrawLine(body_points[i]+Vector3.up*0.3f, body_points[i+1]+Vector3.up*0.3f, Color.red, 0.1f, false);
    //   Debug.DrawLine(body_points[i]+Vector3.down*0.3f, body_points[i+1]+Vector3.down*0.3f, Color.red, 0.1f, false);
    //   Debug.DrawLine(body_points[i]+Vector3.left*0.3f, body_points[i+1]+Vector3.left*0.3f, Color.red, 0.1f, false);
    //   Debug.DrawLine(body_points[i]+Vector3.right*0.3f, body_points[i+1]+Vector3.right*0.3f, Color.red, 0.1f, false);
    // }
  }

  bool shouldStuck() {
    if (board.stuck) return true;
    if (last_expanded > 0 && Time.time - last_expanded > 3) {
      board.stuck = true;
    }
    return board.stuck;
  }

  void updatePoints(int segments = 5) {
    var points = new List<Vector3>();
    var curveX = new AnimationCurve();
    var curveY = new AnimationCurve();
    var curveZ = new AnimationCurve();
    body_width_curve = new AnimationCurve();
    curveX.AddKey(0, transform.position.x);
    curveY.AddKey(0, transform.position.y);
    curveZ.AddKey(0, transform.position.z);
    body_width_curve.AddKey(0, body_size * 0.8f);
    var t = 1;
    for (var p = bodies.First; p != null; p = p.Next) {
      var point = p.Value.GetComponent<BodyBehaviourScript>().keyPoint;
      if (p == bodies.First) { point = p.Value.position; }
      curveX.AddKey(t, point.x);
      curveY.AddKey(t, point.y);
      curveZ.AddKey(t, point.z);
      body_width_curve.AddKey((float)t / Mathf.Min(bodies.Count, 1), p.Value.localScale.x * 0.8f);
      t++;
    }
    for (int i = 0; i < t; i++) {
      curveX.SmoothTangents(i, 0);
      curveY.SmoothTangents(i, 0);
      curveZ.SmoothTangents(i, 0);
    }

    for (int i = 0; i < t * segments; i++) {
      float time = (float)i/segments;
      points.Add(new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time)));
    }

    body_points = points.ToArray();
  }

  bool shouldExpand() {
    if (body_length <= 0) { return false; }
    if (bodies.Count == 0) { return true; }
    return Vector3.Distance(transform.position, bodies.First.Value.position) >= 0.4f * (transform.localScale.x + bodies.First.Value.localScale.x);
  }

  void Expand() {
    var joint = Instantiate(joint_base, transform.position, Quaternion.identity, transform.parent);
    joint.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0.2f, 0.4f, 0.5f, 1, 0.5f, 1);
    joint.gameObject.name = "Joint";
    joint.gameObject.SetActive(true);
    if (bodies.First != null) {
      bodies.First.Value.GetComponent<Collider2D>().isTrigger = false;
    }
    bodies.AddFirst(joint);
    expanded += 1;
    last_expanded = Time.time;
    board.stuck = false;
  }

  void Eat() {
    Debug.Log($"eat in {board.year} {board.season} (+{season_not_eated}={expanded-season_expanded_start}) => {body_length}");
    season_not_eated = 0;
    season_eated += 1;
    switch (board.season) {
      case Season.Spring:
        goto default;
      case Season.Summer:
        Debug.Log("Summer increase size");
        if (body_size < 1.0f + board.year) {
          body_size += 0.1f;
        }
        goto default;
      case Season.Autumn:
        if (speed < body_size * 15) {
          speed += 0.5f;
        }
        break;
      case Season.Winter:
        if (body_length > 0) {
          body_length -= 2;
        }
        break;
      default:
        body_length += 1;
        break;
    }
  }

  bool reverseBody() {
    if (bodies.Count == 0 || board.reverse_count == 0) {
      return false;
    }
    board.reverse_count -= 1;
    transform.position = bodies.Last.Value.position;
    Destroy(bodies.Last.Value.gameObject);
    bodies.RemoveLast();
    if (bodies.First != null) {
      bodies.First.Value.GetComponent<Collider2D>().isTrigger = true;
    }
    if (bodies.Last != null) {
      bodies.Last.Value.GetComponent<Collider2D>().isTrigger = false;
    }
    var newList = new LinkedList<Transform>();
    while (bodies.Count != 0) {
      newList.AddFirst(bodies.First.Value);
      bodies.RemoveFirst();
    }
    bodies = newList;
    return true;
  }

  void checkSeason() {
    var nextSeason = board.season;
    var season_eated_max = 0;
    var season_expaned_max = 200;
    switch (board.season) {
      case Season.Spring:
        nextSeason = Season.Summer;
        season_eated_max = 5;
        season_expaned_max = 0;
        break;
      case Season.Summer:
        nextSeason = Season.Autumn;
        season_eated_max = 5;
        break;
      case Season.Autumn:
        season_eated_max = 10;
        nextSeason = Season.Winter;
        break;
      case Season.Winter:
        season_expaned_max = 150;
        nextSeason = Season.Spring;
        break;
    }
    if ((season_eated_max > 0 && season_eated >= season_eated_max) ||
        (season_expaned_max > 0 && expanded >= season_expanded_start + season_expaned_max)) {
      board.SendMessage("setSeason", nextSeason);
      season_eated = 0;
      season_not_eated = 0;
      season_expanded_start = expanded;
      season_length_start = body_length;
    }
  }
}
