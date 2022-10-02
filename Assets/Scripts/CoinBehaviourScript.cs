using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviourScript : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    if (this.name == "CoinBase") {
      this.gameObject.SetActive(false);
    }
  }

  // Update is called once per frame
  void Update() { }

  void OnTriggerEnter2D(Collider2D other) {
    if (this.name != "CoinBase" && other.CompareTag("Player")) {
      Destroy(this.gameObject);
    }
  }
}
