using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {
  LinkedList<Transform> bodies = new LinkedList<Transform>();
  Transform body_base;
  BoardBehaviourScript board;
  Rigidbody2D head;
  int body_length = 5;
  int season_length_start = 5;
  int season_eated = 0;
  int season_not_eated = 0;
  int season_expanded_start = 0;
  int expanded = 0;
  float last_expanded;
  bool stuck = false;
  float body_size {
    get { return transform.localScale.x; }
    set {
      transform.localScale = Vector3.one * value;
      if (body_base != null)
        body_base.localScale = Vector3.one * value;
    }
  }
  float speed = 5.0f;

  // Start is called before the first frame update
  void Start() {
    body_base = transform.Find("../BodyBase");
    board = transform.parent.parent.gameObject.GetComponent<BoardBehaviourScript>();
    Debug.Log($"Hello world {board}! now: {Time.time}");
    head = GetComponent<Rigidbody2D>();
    body_size = 0.3f;
  }

  // Update is called once per frame
  void Update() {
    var currentSpeed = speed;
    var direction = Vector3.zero;
    if (Input.GetKey("w")) { direction += Vector3.up; }
    if (Input.GetKey("s")) { direction += Vector3.down; }
    if (Input.GetKey("a")) { direction += Vector3.left; }
    if (Input.GetKey("d")) { direction += Vector3.right; }
    if (Input.GetKey(KeyCode.LeftShift)) { currentSpeed *= 2.0f; }
    if (direction != Vector3.zero) {
      direction = direction.normalized;
    }
    head.velocity = direction.normalized * currentSpeed;
    head.angularVelocity = 0;

    if (shouldExpand()) {
      Expand();
      if (board.season == Season.Autumn && season_not_eated >= body_length * 2 / 3) {
        season_not_eated = 0;
        body_length += 1;
      } else {
        season_not_eated += 1;
      }
    }

    if (!stuck && shouldStuck()) {
      Debug.Log("maybe stucking...");
    }

    checkSeason();

    if (body_length <= 0) {
      board.win = true;
    }
  }

  bool shouldStuck() {
    if (stuck) return true;
    if (last_expanded > 0 && Time.time - last_expanded > 3) {
      stuck = true;
    }
    return stuck;
  }

  bool shouldExpand() {
    if (body_length == 0) { return false; }
    if (bodies.Count == 0) { return true; }
    return Vector3.Distance(transform.position, bodies.First.Value.position) >= 0.4f * (transform.localScale.x + bodies.First.Value.localScale.x);
  }

  void Expand() {
    var body_section = Instantiate(body_base, transform.position, Quaternion.identity, transform.parent);
    body_section.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0.2f, 0.4f, 0.5f, 1, 0.5f, 1);
    body_section.gameObject.name = "Body";
    body_section.gameObject.SetActive(true);
    if (bodies.First != null) {
      bodies.First.Value.GetComponent<Collider2D>().isTrigger = false;
    }
    bodies.AddFirst(body_section);
    while (bodies.Count > body_length) {
      var body_last = bodies.Last.Value;
      bodies.RemoveLast();
      Destroy(body_last.gameObject);
    }
    expanded += 1;
    last_expanded = Time.time;
    stuck = false;
  }

  void Eat() {
    Debug.Log($"eat in {board.season}");
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
        goto default;
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
