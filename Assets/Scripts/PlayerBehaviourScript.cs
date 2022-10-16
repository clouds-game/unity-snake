using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {
  LinkedList<Transform> bodies = new LinkedList<Transform>();
  Transform body_base;
  Transform board;
  Rigidbody2D head;
  int body_length = 5;
  int expanded = 0;
  float last_expanded;
  bool stuck = false;
  float body_size = 0.3f;

  // Start is called before the first frame update
  void Start() {
    body_base = transform.Find("../BodyBase");
    board = transform.parent.parent;
    Debug.Log($"Hello world {board}! now: {Time.time}");
    head = GetComponent<Rigidbody2D>();
    resizeBody();
  }

  // Update is called once per frame
  void Update() {
    var speed = 5.0f;
    var direction = Vector3.zero;
    if (Input.GetKey("w")) { direction += Vector3.up; }
    if (Input.GetKey("s")) { direction += Vector3.down; }
    if (Input.GetKey("a")) { direction += Vector3.left; }
    if (Input.GetKey("d")) { direction += Vector3.right; }
    if (Input.GetKey(KeyCode.LeftShift)) { speed *= 2.0f; }
    if (direction != Vector3.zero) {
      direction = direction.normalized;
    }
    head.velocity = direction.normalized * speed;
    head.angularVelocity = 0;

    if (shouldExpand()) {
      Expand();
    }

    if (!stuck && shouldStuck()) {
      Debug.Log("maybe stucking...");
    }

    checkSeason();
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
    return Vector3.Distance(transform.position, bodies.First.Value.position) >= 0.8f * body_size;
  }

  void resizeBody() {
    transform.localScale = Vector3.one * body_size;
    body_base.localScale = Vector3.one * body_size;
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
    body_length += 1;
  }

  void checkSeason() {
    const int baseLength = 100;
    if (expanded == baseLength) {
      board.SendMessage("setSeason", Season.Summer);
      expanded += 1;
    }
    if (expanded == baseLength * 2) {
      board.SendMessage("setSeason", Season.Autumn);
      expanded += 1;
    }
    if (expanded == baseLength * 3) {
      board.SendMessage("setSeason", Season.Winter);
      expanded += 1;
    }
    if (expanded == baseLength * 4) {
      board.SendMessage("setSeason", Season.Spring);
      expanded = 0;
    }
  }
}
