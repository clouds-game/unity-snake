using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBehaviourScript : MonoBehaviour {
  static string[] skin_list = new string[] {
    "lemon", "meitan", "blue",
  };
  static int skin_index = 0;
  public static string skin_head { get { return $"snake_{skin_list[skin_index]}_head"; } }
  public static string skin_body { get { return $"snake_{skin_list[skin_index]}_body"; } }

  public Button[] buttons;
  public GameObject cursor;


  // Start is called before the first frame update
  void Start() {
    buttons = new Button[] {
      GameObject.Find("Image1").GetComponent<Button>(),
      GameObject.Find("Image2").GetComponent<Button>(),
      GameObject.Find("Image3").GetComponent<Button>(),
    };
    cursor = GameObject.Find("Cursor").gameObject;
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown("a")) {
      skin_index -= 1;
    }
    if (Input.GetKeyDown("d")) {
      skin_index += 1;
    }
    skin_index %= skin_list.Length;
    if (skin_index < 0) {
      skin_index += skin_list.Length;
    }
    cursor.transform.position = buttons[skin_index].transform.position;
  }

  public void setSkin(int index) {
    skin_index = index;
  }
}
