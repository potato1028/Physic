using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Test_EnemyBullet : MonoBehaviour {
    [Header("Status")]
    public float bulletSpeed = 10f;

    [Header("Component")]
    public Rigidbody2D rb;

    void Start() {
        rb.velocity = Vector2.right * bulletSpeed;
        Destroy(gameObject, 0.8f);
    }

    void Enter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                Destroy(gameObject);
                break;
        }
    }
}