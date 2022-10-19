using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiBehaviourScript : MonoBehaviour {
  BoardBehaviourScript board;
  TextMeshProUGUI seasonText;
  Sprite[] seasonIcons;
  Image background;
  Sprite[] bgImages;
  GameObject popup;
  // Start is called before the first frame update
  void Start() {
    board = GameObject.Find("/Border").GetComponent<BoardBehaviourScript>();
    // seasonText = GameObject.Find("Canvas/Season/Text").GetComponent<TextMeshProUGUI>();
    seasonIcons = Resources.LoadAll<Sprite>("season_icons");
    background = GameObject.Find("Background/Image").GetComponent<Image>();
    bgImages = new Sprite[] {
      Resources.Load<Sprite>("bg_spring"),
      Resources.Load<Sprite>("bg_summer"),
      Resources.Load<Sprite>("bg_autumn"),
      Resources.Load<Sprite>("bg_winter"),
    };
    popup = GameObject.Find("Canvas/Popup");
    popup.SetActive(false);
    Debug.Log($"UI: {seasonText} init ({seasonIcons.Length} sprite loads)");
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
      // popup.SetActive(false);
    }

    // Season
    // seasonText.text = $"{board.season}";
    switch (board.season) {
      case Season.Spring:
        GameObject.Find("Canvas/Season/Spring").GetComponent<Image>().sprite = seasonIcons[4];
        background.sprite = bgImages[0];
        break;
      case Season.Summer:
        GameObject.Find("Canvas/Season/Summer").GetComponent<Image>().sprite = seasonIcons[5];
        background.sprite = bgImages[1];
        break;
      case Season.Autumn:
        GameObject.Find("Canvas/Season/Autumn").GetComponent<Image>().sprite = seasonIcons[6];
        background.sprite = bgImages[2];
        break;
      case Season.Winter:
        GameObject.Find("Canvas/Season/Winter").GetComponent<Image>().sprite = seasonIcons[7];
        background.sprite = bgImages[3];
        break;
    }

    if (popup.activeInHierarchy) {
      if (Input.GetKeyDown(KeyCode.Space)) {
      // popup.transform.Find("Button").GetComponent<Button>().clicked.Invoke();
        SceneManager.LoadScene("SampleScene");
      }
    }
  }
}
