using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class wave : MonoBehaviour {
    [Header("Status")]
    public float waveSpeed;

    [Header("Object")]
    public GameObject Player;

    [Header("Component")]
    public Rigidbody2D rb;
    public TestPlayerControl playerControl;

    void Awake() {
        Player = GameObject.FindWithTag("Player");
        playerControl = Player.GetComponent<TestPlayerControl>();
        waveSpeed = playerControl.waveSpeed;
    }

    void Start() {
        if(playerControl.isFacingRight) {
            rb.velocity = Vector2.right * waveSpeed;
        }
        else {
            rb.velocity = Vector2.left * waveSpeed;
        }
        Destroy(gameObject, 0.1f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                Destroy(gameObject);
                break;
        }
    }
}