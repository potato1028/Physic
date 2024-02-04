using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class zBullet : MonoBehaviour {
    [Header("Status")]
    public float zSpeed;
    public float zForce;

    [Header("Object")]
    public GameObject Player;

    [Header("Component")]
    public Rigidbody2D rb;
    public TestPlayerControl playerControl;

    void Awake() {
        Player = GameObject.FindWithTag("Player");
        playerControl = Player.GetComponent<TestPlayerControl>();
        zForce = playerControl.centrifugalForce;
        zSpeed = playerControl.centrifugalSpeed;
    }

    void Start() {
        if(playerControl.isFacingRight) {
            rb.velocity = Vector2.right * zSpeed;
        }
        else {
            rb.velocity = Vector2.left * zSpeed;
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