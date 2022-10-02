using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehaviourScript : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    if (this.name == "BodyBase") {
      this.gameObject.SetActive(false);
    }
  }

  void OnTriggerExit2D(Collider2D other) {
    if (other.CompareTag("Player")) {
      other.SendMessage("Expand");
    }
  }
}
