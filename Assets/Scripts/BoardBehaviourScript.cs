using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season {
  Spring,
  Summer,
  Autumn,
  Winter,
}

public class BoardBehaviourScript : MonoBehaviour {
  Camera mainCamera;
  CoinBehaviourScript coinBase;
  Season _season = Season.Spring;
  int _year = 0;
  public int year { get { return _year; } }
  public Season season { get { return _season; } }
  public float season_progress { get; set; }
  public bool win { get; set; }
  public int score { get; set; }
  bool _stuck;
  public bool stuck { get { return !win && _stuck; } set { _stuck = value; } }
  public bool moved { get; set; }
  public int reverse_count { get; set; }
  TextMesh seasonText;

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
    coinBase = transform.Find("CoinBase").GetComponent<CoinBehaviourScript>();
    reverse_count = 1;
    setSeason(Season.Spring);
  }

  // Update is called once per frame
  void Update() {
    if (transform.Find("Coin") == null) {
      var coin = Instantiate(coinBase.gameObject, randomRange(), Quaternion.identity, transform);
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
    _season = season;
    if (season == Season.Spring) {
      _year += 1;
    }
    Debug.Log($"current color {mainCamera.backgroundColor}");
    switch (season) {
      case Season.Spring: mainCamera.backgroundColor = Color.green / 3; break;
      case Season.Summer: mainCamera.backgroundColor = Color.red / 2; break;
      case Season.Autumn: mainCamera.backgroundColor = Color.yellow / 2; break;
      case Season.Winter: mainCamera.backgroundColor = Color.gray / 2; break;
      default: mainCamera.backgroundColor = Color.blue / 2; break;
    }
    coinBase.season = season;
  }
}
