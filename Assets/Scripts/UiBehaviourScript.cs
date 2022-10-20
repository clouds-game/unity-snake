using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiBehaviourScript : MonoBehaviour {
  BoardBehaviourScript board;
  TextMeshProUGUI tips;
  Sprite[] seasonIcons;
  Image background;
  Sprite[] bgImages;
  GameObject popup;
  GameObject dialog;
  float playing_end;
  int playing = -1;
  SortedSet<int> played = new SortedSet<int>();
  static string[] dialogText = new string[] {
    // Spring 0,1
    "我沉眠了多久，真是陌生的世界啊。",
    "也许该吃点什么，小心点不要咬到自己呢。",
    // Summer 2,3
    "越来越热了，我能借着这个灵气修炼吗？",
    "好像记得，前世的目标是化龙来着，也不知道这一世能不能完成。",
    // Autumn 4,5,6
    "灵力好像溢出了，吃点仙草精炼一下吧。",
    "真好吃，身体都变轻快了一些呢。",
    "忽然想这些散落的果实不会也是我灵力溢出凝结出来的吧。",
    // Winter 7,8
    "好冷啊，这就是世界的终焉吗。",
    "如果燃烧自己能让世界重新温暖起来吗。",
    // Spring 2 9
    "终于熬过来了，又是熟悉美好的绿色世界。",
    // Stuck 10
    "冻住了吗，再不挣扎就又要沉眠了。",
    // End 10,11,12
    "我又回到了最初的大小，这个世界也稳定了下来。",
    "这循环的，就叫四季吧。话说回来我成功化龙了吗？",
    "我先睡一会，到了春天，万物就会复苏了。",
  };
  // Start is called before the first frame update
  void Start() {
    board = GameObject.Find("/Border").GetComponent<BoardBehaviourScript>();
    tips = GameObject.Find("Canvas/Tips").GetComponent<TextMeshProUGUI>();
    seasonIcons = Resources.LoadAll<Sprite>("season_icons");
    background = GameObject.Find("Background/Image").GetComponent<Image>();
    bgImages = new Sprite[] {
      Resources.Load<Sprite>("bg_spring"),
      Resources.Load<Sprite>("bg_summer"),
      Resources.Load<Sprite>("bg_autumn"),
      Resources.Load<Sprite>("bg_winter"),
    };
    popup = GameObject.Find("Canvas/Popup"); popup.SetActive(false);
    dialog = GameObject.Find("Canvas/Dialog"); dialog.SetActive(false);
    Debug.Log($"UI: {tips} init ({seasonIcons.Length} sprite loads)");
  }

  // Update is called once per frame
  void Update() {
    checkPlaying();

    if (board.win) {
      popup.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui_win");
      popup.SetActive(true);
      playDialog(new int[] { 11, 12, 13 });
    } else if (board.stuck) {
      playDialog(new int[] { 10 });
      if (played.Contains(10) && playing_end <= Time.time) {
        popup.GetComponent<Image>().sprite = Resources.Load<Sprite>("ui_lose");
        popup.SetActive(true);
      }
    } else {
      popup.SetActive(false);
    }

    // tips
    var tipsText = "";
    if (!board.moved) {
      tipsText += "移动: WSAD\n";
    } else {
      if (!Input.GetKey(KeyCode.LeftShift)) {
        tipsText += "加速: Shift\n";
      }
      if (board.stuck) {
        tipsText += "还能动吗 小蛇?\n";
      }
      if (board.reverse_count > 0) {
        tipsText += "掉头: Z\n";
      } else if (board.stuck) {
        tipsText += "已经不能掉头了\n";
      }
      if (board.season == Season.Autumn) {
        tipsText += "不吃球也会长个子哦\n";
      }
    }
    if (tipsText.Length > 0 && tipsText[tipsText.Length-1] == '\n') {
      tipsText.Remove(tipsText.Length-1);
    }
    tips.text = tipsText;
    if (board.season == Season.Winter) {
      tips.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
    } else {
      tips.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
    }

    // Season
    switch (board.season) {
      case Season.Spring:
        GameObject.Find("Canvas/Season/Summer").GetComponent<Image>().sprite = seasonIcons[1];
        GameObject.Find("Canvas/Season/Autumn").GetComponent<Image>().sprite = seasonIcons[2];
        GameObject.Find("Canvas/Season/Winter").GetComponent<Image>().sprite = seasonIcons[3];
        GameObject.Find("Canvas/Season/Spring").GetComponent<Image>().sprite = seasonIcons[4];
        background.sprite = bgImages[0];
        if (board.year == 1) playDialog(new int[] { 0, 1 });
        if (board.year == 2) playDialog(new int[] { 9 });
        break;
      case Season.Summer:
        GameObject.Find("Canvas/Season/Summer").GetComponent<Image>().sprite = seasonIcons[5];
        background.sprite = bgImages[1];
        if (board.year == 1) playDialog(new int[] { 2, 3 });
        break;
      case Season.Autumn:
        GameObject.Find("Canvas/Season/Autumn").GetComponent<Image>().sprite = seasonIcons[6];
        background.sprite = bgImages[2];
        if (board.year == 1) playDialog(new int[] { 4, 5, 6 });
        break;
      case Season.Winter:
        GameObject.Find("Canvas/Season/Winter").GetComponent<Image>().sprite = seasonIcons[7];
        background.sprite = bgImages[3];
        if (board.year == 1) playDialog(new int[] { 7, 8 });
        break;
    }

    if (popup.activeInHierarchy) {
      if (Input.GetKeyDown(KeyCode.Space)) {
      // popup.transform.Find("Button").GetComponent<Button>().clicked.Invoke();
        SceneManager.LoadScene("SampleScene");
      }
    }
  }

  void playDialog(int[] index) {
    for (int i = 0; i < index.Length; i++) {
      playDialog(index[i], false);
    }
  }

  void playDialog(int index, bool force = false) {
    if (played.Contains(index)) {
      return;
    }
    if (playing >= 0 && !force) {
      // TODO something playing
      return;
    }
    Debug.Log($"playing dialog {index}");
    playing = index;
    playing_end = Time.time + 5;
    dialog.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = dialogText[index];
    dialog.SetActive(true);
    played.Add(index);
  }

  void checkPlaying() {
    if (playing < 0) {
      return;
    }
    if (Input.GetKeyDown(KeyCode.Space) || Time.time >= playing_end) {
      dialog.SetActive(false);
      playing = -1;
    }
  }
}
