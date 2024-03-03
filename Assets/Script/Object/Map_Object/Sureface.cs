using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sureface : MonoBehaviour {
    [Header("GameObject")]
    public GameObject Player;

    [Header("Other_Component")]
    public TestPlayerControl testPlayerControl;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Player = other.gameObject;
            testPlayerControl = Player.GetComponent<TestPlayerControl>();

            testPlayerControl.isSurefacing = true;
            testPlayerControl.SureFace_Water();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && testPlayerControl != null) {
            testPlayerControl.isSurefacing = false;
        }
    }
}