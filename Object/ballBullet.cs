using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ballBullet : MonoBehaviour {
    public float bulletSpeed = 10f;
    public Rigidbody2D rb;
    public GameObject Player;
    public TestPlayerControl playerControl;

    void Awake() {
        Player = GameObject.FindWithTag("Player");
        playerControl = Player.GetComponent<TestPlayerControl>();
    }

    void Start() {
        if(playerControl.isFacingRight) {
            rb.velocity = Vector2.right * bulletSpeed;
        }
        else {
            rb.velocity = Vector2.left * bulletSpeed;
        }
        Destroy(gameObject, 3.0f);
    }
}