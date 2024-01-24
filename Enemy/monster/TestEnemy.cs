using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
    [Header("Status")]
    public float hp = 10;

    [Header("Component")]
    public Rigidbody2D rb;

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
                Invoke("UnlockX", 0.01f);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "zBullet" :
                hp -= other.gameObject.GetComponent<zBullet>().zForce;
                if(hp <= 0) {
                    Destroy(gameObject);
                }
                break;
        }
    }

    void UnlockX() {
        if((rb.constraints & RigidbodyConstraints2D.FreezePositionX) != 0) {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
