using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehaviourScript : MonoBehaviour {
  Vector3 _keyPoint;
  public Vector3 keyPoint { get { return transform.position + Vector3.Scale(transform.localScale, _keyPoint); }}

  static string skin_body;
  static Sprite[] bodySprite;

  // Start is called before the first frame update
  void Start() {
    if (ConfigBehaviourScript.skin_body != skin_body) {
      skin_body = ConfigBehaviourScript.skin_body;
      bodySprite = Resources.LoadAll<Sprite>(skin_body);
    }
    GetComponent<SpriteRenderer>().sprite = bodySprite[Random.Range(0, bodySprite.Length-1)];
    _keyPoint = randomPoint();
  }

  Vector3 randomPoint() {
    var r = Random.Range(0.2f, 0.3f);
    var t = Random.Range(0.0f, 2*Mathf.PI);
    return new Vector3(Mathf.Cos(t), Mathf.Sin(t)) * r;
  }
}
