using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiBehaviourScript : MonoBehaviour {
  BoardBehaviourScript board;
  TextMeshProUGUI tips;
  TextMeshProUGUI score;
  Sprite[] seasonIcons;
  Image background;
  Sprite[] bgImages;
  Slider progress;
  GameObject popup;
  GameObject popupButtonRetry;
  GameObject popupButtonExit;
  GameObject dialog;
  ControlBehaviourScript controller;
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
    "如果燃烧自己能让世界重新温暖起来的话...",
    // Spring2 9
    "终于熬过来了，又是熟悉美好的绿色世界。",
    // Stuck 10
    "冻住了吗，再不挣扎就又要沉眠了。",
    // End 11,12,13
    "我又回到了最初的大小，这个世界也稳定了下来。",
    "这循环的，就叫四季吧。话说回来我成功化龙了吗？",
    "我先睡一会，到了来年春天，万物就会复苏了。",
    // Summer2 14
    "想要化龙就必须长大吗？",
    // Autumn2 15
    "这既视感，我过去真的是条龙也说不定。",
    // Winter2 16
    "如果我全身都燃尽了会发生什么，忽然有点期待呢。",
    // Winter3 17
    "抱元守一，就差一点了！",
    // Lose 18
    "不知道哪里听说: 胜败乃兵家常事，大龙请重新来过。",
    // Autumn3 19
    "这错综复杂的迷宫，这是...我自己？",
    // End2 20
    "没有想到能在春天化龙，不愧是我！这下终于突破循环了吗？",
    // Lose 21
    "...才怪！欢迎回来，让我们继续探险吧！",
  };
  static AudioClip[] _audioClips;
  static AudioClip[] audioClips {
    get {
      if (_audioClips == null) {
        _audioClips = new AudioClip[] {
          Resources.Load<AudioClip>("Audio/dialog0"),
          Resources.Load<AudioClip>("Audio/dialog1"),
          Resources.Load<AudioClip>("Audio/dialog2"),
          Resources.Load<AudioClip>("Audio/dialog3"),
          Resources.Load<AudioClip>("Audio/dialog4"),
          Resources.Load<AudioClip>("Audio/dialog5"),
          Resources.Load<AudioClip>("Audio/dialog6"),
          Resources.Load<AudioClip>("Audio/dialog7"),
          Resources.Load<AudioClip>("Audio/dialog8"),
          Resources.Load<AudioClip>("Audio/dialog9"),
          Resources.Load<AudioClip>("Audio/dialog10"),
          Resources.Load<AudioClip>("Audio/dialog11"),
          Resources.Load<AudioClip>("Audio/dialog12"),
          Resources.Load<AudioClip>("Audio/dialog13"),
          Resources.Load<AudioClip>("Audio/dialog14"),
          Resources.Load<AudioClip>("Audio/dialog15"),
          Resources.Load<AudioClip>("Audio/dialog16"),
          Resources.Load<AudioClip>("Audio/dialog17"),
          Resources.Load<AudioClip>("Audio/dialog18"),
          Resources.Load<AudioClip>("Audio/dialog19"),
          Resources.Load<AudioClip>("Audio/dialog20"),
          Resources.Load<AudioClip>("Audio/dialog21"),
        };
      }
      return _audioClips;
    }
  }
  static AudioClip[] _bgmClips;
  static AudioClip[] bgmClips {
    get {
      if (_bgmClips == null) {
        _bgmClips = new AudioClip[] {
          Resources.Load<AudioClip>("Audio/bgm_spring"),
          Resources.Load<AudioClip>("Audio/bgm_summer"),
          Resources.Load<AudioClip>("Audio/bgm_autumn"),
          Resources.Load<AudioClip>("Audio/bgm_winter"),
        };
      }
      return _bgmClips;
    }
  }
  AudioSource audioSource;

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
    popupButtonRetry = GameObject.Find("Canvas/Popup/ButtonRetry");
    popupButtonExit = GameObject.Find("Canvas/Popup/ButtonExit");
    popup = GameObject.Find("Canvas/Popup"); popup.SetActive(false);
    dialog = GameObject.Find("Canvas/Dialog"); dialog.SetActive(false);
    progress = GameObject.Find("Canvas/Season/Progress").GetComponent<Slider>();
    score = GameObject.Find("Canvas/Score").GetComponent<TextMeshProUGUI>();
    audioSource = GetComponent<AudioSource>();
    audioSource.volume = 0.03f;
    controller = GameObject.Find("Mobile").GetComponent<ControlBehaviourScript>();
    Debug.Log($"UI: {tips} init ({seasonIcons.Length} sprite loads)");
  }

  // Update is called once per frame
  void Update() {
    checkPlaying();

    if (board.win) {
      showPopup(Resources.Load<Sprite>("ui_win"));
      if (board.season == Season.Spring) {
        playDialog(new int[] { 20 });
      } else {
        playDialog(new int[] { 11, 12, 13 });
      }
    } else if (board.stuck) {
      playDialog(new int[] { 10 });
      if (played.Contains(10) && playing_end <= Time.time) {
        showPopup(Resources.Load<Sprite>("ui_lose"));
        played.Remove(21);
        playDialog(new int[] { 18 });
      }
    } else {
      if (played.Contains(18)) {
        played.Remove(18);
        playDialog(new int[] { 21 });
      }
      played.Remove(10);
      popup.SetActive(false);
    }

    // cheat
    if ((board.stuck || board.win) && Input.GetKeyDown("q")) {
      Debug.Log("win immediately");
      board.win = !board.win;
    }
    score.text = $"{board.score}";

    // tips
    var tipsText = "";
    if (!board.moved) {
      tipsText += "移动: WSAD\n";
    } else if (board.win) {
      tipsText += "完结撒花\n";
      tipsText += "复苏: Space\n";
    } else {
      if (!Input.GetKey(KeyCode.LeftShift) && !controller.GetKey("Shift")) {
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
      if (board.stuck && played.Contains(18)) {
        tipsText += "复苏: Space\n";
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
        playBGM(bgmClips[0]);
        progress.value = board.season_progress / 3;
        if (board.year == 1) playDialog(new int[] { 0, 1 });
        if (board.year == 2) playDialog(new int[] { 9 });
        break;
      case Season.Summer:
        GameObject.Find("Canvas/Season/Summer").GetComponent<Image>().sprite = seasonIcons[5];
        progress.value = (board.season_progress + 1) / 3;
        background.sprite = bgImages[1];
        playBGM(bgmClips[1]);
        if (board.year == 1) playDialog(new int[] { 2, 3 });
        if (board.year == 2) playDialog(new int[] { 14 });
        break;
      case Season.Autumn:
        GameObject.Find("Canvas/Season/Autumn").GetComponent<Image>().sprite = seasonIcons[6];
        progress.value = (board.season_progress + 2) / 3;
        background.sprite = bgImages[2];
        playBGM(bgmClips[2]);
        if (board.year == 1) playDialog(new int[] { 4, 5, 6 });
        if (board.year == 2) playDialog(new int[] { 15 });
        if (board.year == 3) playDialog(new int[] { 19 });
        break;
      case Season.Winter:
        GameObject.Find("Canvas/Season/Winter").GetComponent<Image>().sprite = seasonIcons[7];
        progress.value = 1 - board.season_progress;
        background.sprite = bgImages[3];
        playBGM(bgmClips[3]);
        if (board.year == 1) playDialog(new int[] { 7, 8 });
        if (board.year == 2) playDialog(new int[] { 16 });
        if (board.year == 3) playDialog(new int[] { 17 });
        break;
    }

    if (popup.activeInHierarchy) {
      if (Input.GetKeyDown(KeyCode.Space) || ControlBehaviourScript.CheckPosition(popupButtonRetry)) {
      // popup.transform.Find("ButtonRetry").GetComponent<Button>().clicked.Invoke();
        SceneManager.LoadScene("SampleScene");
      }
      if (Input.GetKeyDown(KeyCode.Escape) || ControlBehaviourScript.CheckPosition(popupButtonExit)) {
      // popup.transform.Find("ButtonExit").GetComponent<Button>().clicked.Invoke();
        SceneManager.LoadScene("WelcomeScene");
      }
    }
  }

  void showPopup(Sprite sprite) {
    var image = popup.GetComponent<Image>();
    image.color = Color.white;
    image.sprite = sprite;
    popup.SetActive(true);
  }

  void playBGM(AudioClip clip) {
    if (audioSource.clip == clip) {
      return;
    } else {
      audioSource.clip = clip;
      audioSource.Play();
    }
  }

  void playDialog(int[] index) {
    for (int i = 0; i < index.Length; i++) {
      playDialog(index[i], i == 0);
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
    Debug.Log($"playing dialog {index} with clip {audioClips[index]}.");
    playing = index;
    playing_end = Time.time + 5;
    dialog.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = dialogText[index];
    dialog.SetActive(true);
    var audio = dialog.GetComponent<AudioSource>();
    audio.clip = audioClips[index];
    audio.Play();
    playing_end = Time.time + Mathf.Max(5, audioClips[index].length / audio.pitch);
    played.Add(index);
  }

  void checkPlaying() {
    if (playing < 0 || Input.GetKeyDown(KeyCode.Space) || Time.time >= playing_end) {
      dialog.GetComponent<AudioSource>().Stop();
      dialog.SetActive(false);
      playing = -1;
    }
  }
}
