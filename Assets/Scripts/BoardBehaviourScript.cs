using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehaviourScript : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    var box = this.GetComponent<BoxCollider2D>();
    var edge = this.gameObject.AddComponent<EdgeCollider2D>();
    var bounds = box.bounds;
    edge.points = new Vector2[] {
      new Vector2(bounds.min.x, bounds.min.y),
      new Vector2(bounds.min.x, bounds.max.y),
      new Vector2(bounds.max.x, bounds.max.y),
      new Vector2(bounds.max.x, bounds.min.y),
      new Vector2(bounds.min.x, bounds.min.y),
    };
  }

  // Update is called once per frame
  void Update() {
    if (transform.Find("Coin") == null) {
      var baseObj = transform.Find("CoinBase");
      var coin = Instantiate(baseObj, randomRange(), Quaternion.identity, transform);
      coin.name = "Coin";
      coin.gameObject.SetActive(true);
    }
  }

  Vector3 randomRange() {
    var box = this.GetComponent<BoxCollider2D>();
    var bounds = box.bounds;
    var point = new Vector3(
      Random.Range(bounds.min.x, bounds.max.x),
      Random.Range(bounds.min.y, bounds.max.y),
      Random.Range(bounds.min.z, bounds.max.z)
    );
    return point;
  }
}
