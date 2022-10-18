using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

public class UiBehaviourScript : MonoBehaviour {
  BoardBehaviourScript board;
  TextMeshProUGUI seasonText;
  GameObject popup;
  // Start is called before the first frame update
  void Start() {
    board = GameObject.Find("/Border").GetComponent<BoardBehaviourScript>();
    seasonText = GameObject.Find("Canvas/Season").GetComponent<TextMeshProUGUI>();
    popup = GameObject.Find("Canvas/Popup");
    popup.SetActive(false);
    Debug.Log($"UI: {seasonText} init");
  }

  // Update is called once per frame
  void Update() {
    if (board.win) {
      var text = popup.transform.Find("Text").GetComponent<TextMeshProUGUI>();
      text.text = "You WIN!";
      text.color = Color.yellow;
      popup.transform.Find("Hint").GetComponent<TextMeshProUGUI>().text = "";
      popup.SetActive(true);
    // } else if (board.stuck) {
    //   var text = popup.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    //   text.text = "Maybe dead";
    //   text.color = Color.red;
    //   popup.transform.Find("Hint").GetComponent<TextMeshProUGUI>().text = "press w/s/a/d to continue, space to restart";
    //   popup.SetActive(true);
    } else {
      popup.SetActive(false);
    }
    seasonText.text = $"{board.season}";

    if (popup.activeInHierarchy) {
      if (Input.GetKeyDown(KeyCode.Space)) {
      // popup.transform.Find("Button").GetComponent<Button>().clicked.Invoke();
        SceneManager.LoadScene("SampleScene");
      }
    }
  }
}
