using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum ButtonState {
  Release, Down, Hold, Up,
}

public class ButtonInput {
  public string name;
  public KeyCode key;
  public Button ui;
  public ButtonState state = ButtonState.Release;
  public int fingerId = int.MinValue;
  public bool active = false;

  public static ButtonInput init(string name, KeyCode key, string path) {
    var self = new ButtonInput();
    self.name = name;
    self.key = key;
    self.ui = GameObject.Find(path).GetComponent<Button>();
    return self;
  }

  public void release() {
    state = ButtonState.Release;
    fingerId = int.MinValue;
    active = false;
  }
}

public class ControlBehaviourScript : MonoBehaviour {
	GameObject leftPanel;
	GameObject rightPanel;
  GameObject joyBase;
  GameObject joyCursor;
  TextMeshProUGUI debugInfo;
  string debugText = "";

  float enableTimeout = -1;
  int trackedLeftId = int.MinValue;

  Vector2 _direction;
  public Vector2 direction { get { return _direction; } }

  ButtonInput[] buttons;

  // Start is called before the first frame update
  void Start() {
		leftPanel = GameObject.Find("Left");
    joyBase = GameObject.Find("Left/Base");
    joyCursor = GameObject.Find("Left/Base/Cursor");
		rightPanel = GameObject.Find("Right");
    buttons = new ButtonInput[] {
      ButtonInput.init("Z", KeyCode.Z, "Right/Button_Key_Z"),
      ButtonInput.init("Shift", KeyCode.LeftShift, "Right/Button_Key_Shift"),
    };
    var debugInfo = GameObject.Find("DebugInfo"); debugInfo.SetActive(false);
    ActiveControl(false);
    Debug.Log($"{buttons} => {buttons[0]} => {buttons[0].ui};");
  }

  // Update is called once per frame
  void Update() {
    for (int i = 0; i < buttons.Length; i++) {
      if (!buttons[i].ui.IsActive() || buttons[i].fingerId == int.MinValue) {
        buttons[i].release();
      }
    }
    if (debugInfo != null) debugText = "";
    if (Input.touchCount == 0) {
      if (Input.GetMouseButtonDown(0)) {
        OnTouch(-1, Input.mousePosition, TouchPhase.Began);
      }
      if (Input.GetMouseButton(0)) {
        OnTouch(-1, Input.mousePosition, TouchPhase.Moved);
      }
      if (Input.GetMouseButtonUp(0)) {
        OnTouch(-1, Input.mousePosition, TouchPhase.Ended);
      }
    }
    for (int i = 0; i < Input.touchCount; i++) {
      var touch = Input.GetTouch(i);
      OnTouch(touch.fingerId, touch.position, touch.phase);
    }
    if (debugInfo != null) debugInfo.text = debugText;

    if (enableTimeout == -1 || (enableTimeout != 0 && Time.time > enableTimeout)) {
      ActiveControl(false);
    }
  }

  void ActiveControl(bool active) {
    joyBase.SetActive(active);
    rightPanel.SetActive(active);
    if (debugInfo != null) debugInfo.gameObject.SetActive(active);
  }


  bool checkButton(KeyCode code, ButtonState state) {
    for (int i = 0; i < buttons.Length; i++) {
      if (buttons[i].key == code && checkButton(buttons[i], state))  {
        return true;
      }
    }
    return false;
  }
  bool checkButton(string code, ButtonState state) {
    for (int i = 0; i < buttons.Length; i++) {
      if (buttons[i].name == code && checkButton(buttons[i], state))  {
        return true;
      }
    }
    return false;
  }
  bool checkButton(ButtonInput button, ButtonState state) {
    if (state == button.state) {
      return true;
    }
    if (state == ButtonState.Hold) {
      return button.state == ButtonState.Down || button.state == ButtonState.Up;
    }
    return false;
  }

  public bool GetKeyDown(KeyCode s) { return checkButton(s, ButtonState.Down); }
  public bool GetKeyDown(string s) { return checkButton(s, ButtonState.Down); }
  public bool GetKey(KeyCode s) { return checkButton(s, ButtonState.Hold); }
  public bool GetKey(string s) { return checkButton(s, ButtonState.Hold); }
  public bool GetKeyUp(KeyCode s) { return checkButton(s, ButtonState.Up); }
  public bool GetKeyUp(string s) { return checkButton(s, ButtonState.Up); }

  static bool checkPosition(Vector2 position, GameObject obj) {
    var rect = obj.GetComponent<RectTransform>().rect;
    rect = new Rect((Vector2)obj.transform.position + rect.position, rect.size);
    // Debug.Log($"touch {position} {rect} {obj.transform.position} {obj.name}");
    return rect.Contains(position);
  }

  void OnTouch(int fingerId, Vector2 position, TouchPhase phase) {
    if (debugInfo != null) debugText += $"{fingerId}: {position} {phase}\n";
    // Debug.Log($"touch {position} {phase}");
    if (checkPosition(position, leftPanel)) {
      if (phase == TouchPhase.Began && trackedLeftId == int.MinValue) {
        joyBase.transform.position = position;
        trackedLeftId = fingerId;
        ActiveControl(true);
      }
    }
    if (fingerId == trackedLeftId) {
      enableTimeout = Time.time + 3;
      switch (phase) {
        case TouchPhase.Began:
        case TouchPhase.Stationary:
        case TouchPhase.Moved:
          // TODO: check id
          var relative = position - (Vector2)joyBase.transform.position;
          // the sprite size is larger than rect
          var radius = joyBase.GetComponent<RectTransform>().rect.size.magnitude / Mathf.Sqrt(2);
          var radius2 = joyCursor.GetComponent<RectTransform>().rect.size.magnitude / Mathf.Sqrt(2);
          radius -= radius2;
          _direction = relative / radius;
          if (_direction.magnitude > 1) {
            _direction = _direction.normalized;
          }
          joyCursor.transform.position = (Vector3)_direction * radius + joyBase.transform.position;
          // Debug.Log($"touch move {relative} {radius} {_direction} => {joyCursor.transform.position}");
          break;
        case TouchPhase.Canceled:
        case TouchPhase.Ended:
          // TODO: check id
          _direction = Vector2.zero;
          joyBase.SetActive(false);
          trackedLeftId = int.MinValue;
          break;
      }
    }
    for (int i = 0; i < buttons.Length; i++) {
      var button = buttons[i];
      if (!button.ui.IsActive()) {
        continue;
      }
      if (checkPosition(position, button.ui.gameObject)) {
        if (phase == TouchPhase.Began && button.fingerId == int.MinValue) {
          button.fingerId = fingerId;
        }
      }
      if (fingerId == button.fingerId) {
        enableTimeout = Time.time + 3;
        button.active = checkPosition(position, button.ui.gameObject);
        switch (phase) {
          case TouchPhase.Began:
            button.state = ButtonState.Down;
            break;
          case TouchPhase.Moved:
          case TouchPhase.Stationary:
            button.state = ButtonState.Hold;
            break;
          case TouchPhase.Ended:
          case TouchPhase.Canceled:
            button.state = ButtonState.Up;
            button.fingerId = int.MinValue;
            break;
        }
      }
      // Debug.Log($"{button.name} {button.fingerId} {button.state} {GetKeyDown(button.name)} {GetKey(button.name)}");
    }
  }
}
