using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiBehaviourScript : MonoBehaviour {
  BoardBehaviourScript board;
  TextMeshProUGUI seasonText;
  // Start is called before the first frame update
  void Start() {
    board = GameObject.Find("/Border").GetComponent<BoardBehaviourScript>();
    seasonText = transform.Find("Canvas/Season").gameObject.GetComponent<TextMeshProUGUI>();
    Debug.Log($"UI: {seasonText} init");
  }

  // Update is called once per frame
  void Update() {
    if (board.win) {
      seasonText.text = "Win";
    } else {
      seasonText.text = $"{board.season}";
    }
  }
}
