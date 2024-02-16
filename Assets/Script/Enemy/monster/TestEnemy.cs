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
    public Vector2 frontVec;

    [Header("Enemy_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public BoxCollider2D box2D;

    [Header("Others_Component")]
    public Bind bind;
    public TestPlayerControl player_Component;

    [Header("Layer")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("Condition")]
    public bool isFacingRight = false;
    public bool isMoveAllow = true;
    public bool isAttackRange = false;
    public bool isDetectionPlayer = false;
    public bool isBind = false;
    public bool isRoamAllow = true;
    //
    public int roamNext;
    public float nextRaomTime;

    [Header("RayCast")]
    private Ray forwardRay;
    private Ray backwardRay;
    private Ray attackRangeRay;
    private RaycastHit2D forwardHit;
    private RaycastHit2D backwardHit;
    private RaycastHit2D attackRangeHit;
    private RaycastHit2D roamRayHit;

    void Awake() {
        Invoke("Roam", 5);
    }

    void Update() {
        DetectionPlayer();
    }

    void FixedUpdate() {
        if(!isDetectionPlayer && isMoveAllow) {
            rb.velocity = new Vector2(roamNext * moveSpeed, rb.velocity.y);
        }

        frontVec = new Vector2(rb.position.x + roamNext * 0.2f, rb.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color (0, 1, 0));
        roamRayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, groundLayer);
        if(roamRayHit.collider == null) {
            roamNext *= -1;
            CancelInvoke();
            Invoke("Roam", 5);
        }
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
            CancelInvoke();
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
            Invoke("Roam_Delay", 1f);
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
                player_Component = other.gameObject.GetComponent<TestPlayerControl>();
                if(player_Component.isMagneting) {
                    hp--;
                }
                break;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Player" :
                rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                player_Component = other.gameObject.GetComponent<TestPlayerControl>();
                if(player_Component.isMagneting) {
                    hp--;
                }
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

    void Bind_Delay() {
        isMoveAllow = true;
        isBind = false;
    }

    void Roam_Delay() {
        isDetectionPlayer = false;
    }

    void Roam() {
        roamNext = Random.Range(-1, 2);
        nextRaomTime = Random.Range(6f, 9f);

        Invoke("Roam", nextRaomTime);
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





 