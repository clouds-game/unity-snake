using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {
  LinkedList<Transform> bodies = new LinkedList<Transform>();
  Transform body_base;
  int body_length = 5;

  // Start is called before the first frame update
  void Start() {
    Debug.Log("Hello world");
    body_base = transform.Find("../BodyBase");
  }

  // Update is called once per frame
  void Update() {
    var step = Time.deltaTime * 5.0f;
    if (Input.GetKey("w")) {
      transform.position += Vector3.up * step;
    } else if (Input.GetKey("s")) {
      transform.position += Vector3.down * step;
    } else if (Input.GetKey("a")) {
      transform.position += Vector3.left * step;
    } else if (Input.GetKey("d")) {
      transform.position += Vector3.right * step;
    }

    if (body_length != 0 && bodies.Count == 0) {
      Expand();
    }
  }

  void Expand() {
    var body_section = Instantiate(body_base, transform.position, Quaternion.identity, transform.parent);
    body_section.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
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
  }

  void Eat() {
    body_length += 1;
  }
}
