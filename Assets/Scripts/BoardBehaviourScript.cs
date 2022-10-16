using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Season {
  Spring,
  Summer,
  Autumn,
  Winter,
}

public class BoardBehaviourScript : MonoBehaviour {
  Camera mainCamera;
  Season season = Season.Spring;

  // Start is called before the first frame update
  void Start() {
    mainCamera = Camera.allCameras[0];
    var box = this.GetComponent<BoxCollider2D>();
    box.size = new Vector2(Screen.width * box.size.y / Screen.height, box.size.y);
    var edge = this.gameObject.AddComponent<EdgeCollider2D>();
    var bounds = box.bounds;
    edge.points = new Vector2[] {
      new Vector2(bounds.min.x, bounds.min.y),
      new Vector2(bounds.min.x, bounds.max.y),
      new Vector2(bounds.max.x, bounds.max.y),
      new Vector2(bounds.max.x, bounds.min.y),
      new Vector2(bounds.min.x, bounds.min.y),
    };
    setSeason(Season.Spring);
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

  void setSeason(Season season) {
    Debug.Log($"enter season {season}");
    this.season = season;
    Debug.Log($"current color {mainCamera.backgroundColor}");
    switch (season) {
      case Season.Spring: mainCamera.backgroundColor = Color.green; break;
      case Season.Summer: mainCamera.backgroundColor = Color.red; break;
      case Season.Autumn: mainCamera.backgroundColor = Color.yellow; break;
      case Season.Winter: mainCamera.backgroundColor = Color.gray; break;
      default: mainCamera.backgroundColor = Color.blue; break;
    }

  }
}
