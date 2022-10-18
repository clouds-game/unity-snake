using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBehaviourScript : MonoBehaviour {
  public void LoadScene(string name) {
    SceneManager.LoadScene(name);
  }
}
