using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
    public float hp = 5;
    public Rigidbody2D rb;

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                if((rb.constraints & RigidbodyConstraints2D.FreezePositionX) == 0) {
                    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
                }
                Debug.Log("LockX");
                Invoke("UnlockX", 0.1f);
                break;
        }
    }

    void UnlockX() {
        if((rb.constraints & RigidbodyConstraints2D.FreezePositionX) != 0) {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
