using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet_Blue : MonoBehaviour {
    [Header("Player")]
    public GameObject Player;
    public TestPlayerControl testPlayerControl;

    void Start() {
        Player = GameObject.FindWithTag("Player");
        testPlayerControl = Player.GetComponent<TestPlayerControl>();
    }

    void Update() {
        if(Input.GetKey(KeyCode.R) && !testPlayerControl.isMagneting) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if(hit.collider != null && hit.collider.gameObject == gameObject) {
                if(testPlayerControl.isRedCondition) {
                    testPlayerControl.Magnetic_Move((Vector2)transform.position);
                }
                else {
                    Debug.Log("Player is not Blue");
                }
            } 
        }
    }
}