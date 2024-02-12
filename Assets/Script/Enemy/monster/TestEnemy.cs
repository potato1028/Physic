using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
    [Header("Status")]
    public float horiaontalInput;
    public float hp = 10;
    public float moveSpeed = 2.0f;
    public float attckSpeed = 0.8f;
    public float forwardDistance = 3.0f;
    public float backwardDistance = 1.5f;
    public float attackRangeDistance = 0.8f;
    //
    public Vector3 newScale;
    public Vector3 direction;
    public Vector2 currentPosition;

    [Header("Enemy_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public BoxCollider2D box2D;

    [Header("Others_Component")]
    public Bind bind;

    [Header("Layer")]
    public LayerMask playerLayer;

    [Header("Condition")]
    public bool isFacingRight = false;
    public bool isMoveAllow = true;
    public bool isAttackRange = false;
    public bool isDetectionPlayer = false;
    public bool isBind = false;

    [Header("RayCast")]
    private Ray forwardRay;
    private Ray backwardRay;
    private Ray attackRangeRay;
    private RaycastHit2D forwardHit;
    private RaycastHit2D backwardHit;
    private RaycastHit2D attackRangeHit;

    [Header("Floor")]
    public GameObject moveFloor;
    public Bounds bFloor;

    void Start() {
        bFloor = moveFloor.GetComponent<SpriteRenderer>().bounds;
    }

    void Update() {
        DetectionPlayer();
    }

    #region Collider

    void DetectionPlayer() {
        if(isFacingRight && !isBind) {
            forwardRay = new Ray(transform.position, transform.right);
            backwardRay = new Ray(transform.position, -transform.right);
            attackRangeRay = new Ray(transform.position, transform.right);
        }
        else if(!isFacingRight && !isBind) {
            forwardRay = new Ray(transform.position, -transform.right);
            backwardRay = new Ray(transform.position, transform.right);
            attackRangeRay = new Ray(transform.position, -transform.right);
        }
        

        forwardHit = Physics2D.Raycast(forwardRay.origin, forwardRay.direction, forwardDistance, playerLayer);
        backwardHit = Physics2D.Raycast(backwardRay.origin, backwardRay.direction, backwardDistance, playerLayer);
        attackRangeHit = Physics2D.Raycast(attackRangeRay.origin, attackRangeRay.direction, attackRangeDistance, playerLayer);

        if(forwardHit.collider && !isBind) {
            Follow(forwardHit.collider.gameObject);
            isDetectionPlayer = true;
            if(forwardDistance <= 3.0f) {
                forwardDistance *= 1.5f;
                backwardDistance *= 1.5f;
            }
        }
        else if(backwardHit.collider && !isBind) {
            Flip();
        }
        else {
            isDetectionPlayer = false;
        }

        if(!forwardHit.collider && !isDetectionPlayer && !isBind) {

            if(forwardDistance > 3.0f) {
                forwardDistance /= 1.5f;
                backwardDistance /= 1.5f;
            }
        }
        
        if(attackRangeHit.collider && !isBind) {
            StartCoroutine(Attack());
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                break;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                break;
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX & ~RigidbodyConstraints2D.FreezePositionY;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "zBullet" :
                hp -= other.gameObject.GetComponent<zBullet>().zForce;
                if(hp <= 0) {
                    Destroy(gameObject);
                }
                break;

            case "RepulsiveForce" :
                if(other.transform.position.x < this.transform.position.x) {
                    rb.velocity = new Vector2(20f, rb.velocity.y);
                }
                else {
                    rb.velocity = new Vector2(-20f, rb.velocity.y);
                }
                break;

            case "absoluteBind" :
                isMoveAllow = false;
                isBind = true;
                Debug.Log("Bind");
                break;

            case "blackholeBomb" :
                Debug.Log("Bomb");
                hp -= 5;
                break;

        }
    }

    void OnTriggerExit2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {
            case "absoluteBind" :
                bind = other.GetComponent<Bind>();
                Invoke("Bind_Delay", bind.bindTime);
                break;

            case "blackholeBomb" :
                Debug.Log("Bomb");
                hp -= 5;
                break;
        }
    }

    #endregion

    #region Interaction

    void Flip() {
        if(!isFacingRight && isMoveAllow) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
            isFacingRight = !isFacingRight;
        }
        else if(isFacingRight && isMoveAllow) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
            isFacingRight = !isFacingRight;
        }
    }
    
    void Follow(GameObject player) {
        if(isMoveAllow) {
            direction = (player.transform.position - this.transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    void Roam() {

    }

    void Bind_Delay() {
        isMoveAllow = true;
        isBind = false;
    }

    #endregion

    #region Attack

    IEnumerator Attack() {
        isMoveAllow = false;
        Debug.Log("Attack");

        yield return new WaitForSeconds(attckSpeed);

        if(attackRangeHit.collider != null) {
            StartCoroutine(Attack());
        }
        else {
            isMoveAllow = true;
        }
    }

    #endregion
}





 