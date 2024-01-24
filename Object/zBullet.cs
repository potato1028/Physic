using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class zBullet : MonoBehaviour {
    [Header("Status")]
    public float bulletSpeed = 10f;
    public float zForce;

    [Header("Object")]
    public GameObject Player;

    [Header("Component")]
    public Rigidbody2D rb;
    public TestPlayerControl playerControl;

    void Awake() {
        Player = GameObject.FindWithTag("Player");
        playerControl = Player.GetComponent<TestPlayerControl>();
        zForce = playerControl.zForce;
    }

    void Start() {
        if(playerControl.isFacingRight) {
            rb.velocity = Vector2.right * bulletSpeed;
        }
        else {
            rb.velocity = Vector2.left * bulletSpeed;
        }
        Destroy(gameObject, 2.0f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                Destroy(gameObject);
                break;
        }
    }
}