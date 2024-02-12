using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [Header("Status")]
    public float bombSpeed;
    public float bombTime;
    public float maxHeight = 50.0f;
    public Vector3 mousePosition;
    public Vector3 worldMousePosition;
    public float distanceX;

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
        bombSpeed = testPlayerControl.blackholeBombSpeed;
        bombTime = testPlayerControl.blackholeBombTime;
    }

    public void Shoot() {
        circle2D.enabled = true;
        transform.parent = null;

        mousePosition = Input.mousePosition;
        worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        distanceX = Mathf.Abs(transform.position.x - worldMousePosition.x);

        rb.bodyType = RigidbodyType2D.Dynamic;

        distanceX = (transform.position.x - worldMousePosition.x) < 0 ? distanceX : (distanceX *= -1);

        rb.AddForce(new Vector2(distanceX, maxHeight / distanceX), ForceMode2D.Impulse);

        Debug.Log(distanceX);
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                rb.velocity = Vector2.zero;
                circle2D.radius = 3.0f;
                Destroy(gameObject, 0.2f);
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