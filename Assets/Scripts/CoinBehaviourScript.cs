using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviourScript : MonoBehaviour {
  public Season season;
  static Sprite[] icons;

  // Start is called before the first frame update
  void Start() {
    if (this.name == "CoinBase") {
      this.gameObject.SetActive(false);
      icons = Resources.LoadAll<Sprite>("season_balls");
    } else {
      Sprite sprite = null;
      switch (season) {
        case Season.Spring: sprite = icons[1]; break;
        case Season.Summer: sprite = icons[3]; break;
        case Season.Autumn: sprite = icons[5]; break;
        case Season.Winter: sprite = icons[7]; break;
      }
      if (sprite != null) {
        GetComponent<SpriteRenderer>().sprite = sprite;
      }
    }
  }

  // Update is called once per frame
  void Update() { }

  void OnTriggerEnter2D(Collider2D other) {
    if (this.name != "CoinBase" && other.CompareTag("Player")) {
      other.SendMessage("Eat");
      Destroy(this.gameObject);
    }
  }
}
