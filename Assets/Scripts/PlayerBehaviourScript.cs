using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    Debug.Log("Hello world");
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
  }
}
