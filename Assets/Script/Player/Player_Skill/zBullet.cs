using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class zBullet : MonoBehaviour {
    [Header("Status")]
    public float zSpeed;
    public float zForce;
    public Vector3 mousePosition;
    public Vector3 direction;
    public Vector2 movementDirection;
    public float angle;

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
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if(angle < 0) {
            angle += 360;
        }

        rb.rotation = angle;

        movementDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        rb.velocity = movementDirection.normalized * zSpeed * zForce;

        Debug.Log("Angle: " + angle);

        Destroy(gameObject, 2.0f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "hostileOrg" :
                Destroy(gameObject, 0.2f);
                break;
            

            case "Ground" :
                Destroy(gameObject, 0.2f);
                break;
            }
    }
}