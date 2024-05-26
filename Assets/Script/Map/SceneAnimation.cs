using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAnimation : MonoBehaviour {
    public string sceneName;
    public Vector2 SpawnPosition;

    void OnTriggerEnter2D(Collider2D other) {
        if(LayerMask.LayerToName(other.gameObject.layer) == "Player") {
            SceneTransform.Instance.ChangeScene(sceneName, other.gameObject, SpawnPosition);
        }
    }
}
