using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind : MonoBehaviour {
    [Header("Status")]
    public float bindSpeed;
    public float bindTime;
    public float currentSize = 0.25f;
    public Vector3 mousePosition;
    public Vector3 direction;
    public Vector2 movementDirection;
    public float angle;

    [Header("Condition")]
    public bool isHitEnemy = false;

    [Header("Object")]
    public GameObject Player;
    public GameObject Enemy;

    [Header("Bind_Component")]
    public Rigidbody2D rb;
    public CircleCollider2D circle2D;

    [Header("Others_Component")]
    public TestPlayerControl testPlayerControl;

    void Start() {
        circle2D.enabled = false;

        Player = transform.parent.gameObject;
        testPlayerControl = Player.GetComponent<TestPlayerControl>();
        bindSpeed = testPlayerControl.absoluteZeroSpeed;
        bindTime = testPlayerControl.absoluteZeroTime;
    }

    void Update() {
        if(isHitEnemy) {
            currentSize += 0.8f * Time.deltaTime;
            currentSize = Mathf.Min(currentSize, 2.0f);
            circle2D.radius = currentSize;
            if(circle2D.radius == 2.0f) {
                Destroy(gameObject);
            }
        }
    }

    public void Shoot() {
        circle2D.enabled = true;
        transform.parent = null;

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if(angle < 0) {
            angle += 360;
        }

        movementDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        rb.velocity = movementDirection.normalized * bindSpeed;
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                rb.velocity = Vector2.zero;
                isHitEnemy = true;
                break;
            

            case "Ground" :
                if(!isHitEnemy) {
                    Destroy(gameObject, 0.2f);
                }
                break;

            case "Wall" :
                if(!isHitEnemy) {
                    Destroy(gameObject, 0.2f);
                }
                break;

            }
    }
}