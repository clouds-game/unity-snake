using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehaviourScript : MonoBehaviour {
  Vector3 _keyPoint;
  public Vector3 keyPoint { get { return transform.position + Vector3.Scale(transform.localScale, _keyPoint); }}

  static Sprite[] bodySprite;

  // Start is called before the first frame update
  void Start() {
    if (bodySprite == null) {
      bodySprite = Resources.LoadAll<Sprite>("snake_lemon_body");
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
