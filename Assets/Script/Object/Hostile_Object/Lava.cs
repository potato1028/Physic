using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {
    public Vector2 lavaPosition;

    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            other.transform.position = lavaPosition;
        }
    }
}
