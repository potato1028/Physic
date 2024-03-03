using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {
    [Header("Status")]
    public float springX;
    public float springY;

    [Header("GameObject")]
    public GameObject Player;

    [Header("Other_Component")]
    public TestPlayerControl testPlayerControl;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Player = other.gameObject;
            testPlayerControl = Player.GetComponent<TestPlayerControl>();
            if(testPlayerControl.isMoveAllow && testPlayerControl.isGrounded) {
                testPlayerControl.rb.velocity = new Vector2(springX, springY);
            }        
        }
    }
}