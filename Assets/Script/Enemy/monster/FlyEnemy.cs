using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : MonoBehaviour {
    [Header("Status")]
    public int forwardRayCount = 14;
    public int facingIndex;
    public float forwardRayDistance = 3.5f;
    public float backwardRayDistance = 0.8f;
    public float attackRayDistance = 0.8f;
    public float angleDetect;
    public float angleDetectValue = 5f;
    public List<RaycastHit2D> forwardHitResults = new List<RaycastHit2D>();
    public float horiaontalInput;
    public float hp = 10;
    public float moveSpeed = 2.0f;
    public float attackDelayTime = 0.8f;
    public float attackReadyTime = 0.2f;
    public float crashDamage = 1f;
    //
    public Vector2 crashPush;
    public Vector2 roamVec;

    [Header("Enemy_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public CircleCollider2D circle2D;

    [Header("Others_Component")]
    public Bind bind;
    public Bomb bomb;
    public TestPlayerControl player_Component;

    [Header("Condition")]
    public bool isFacingRight = false;
    public bool isMoveAllow = true;
    public bool isBinded = false;
    public bool isDetectPlayer = false;
    public bool isRoamAllow = true;
    public bool isFollowing = false;
    public bool isAttacking = false;
    //
    public int roamNext;
    public float nextRaomTime;
    public float DetectTime = 5f;

    [Header("RayCast")]
    private RaycastHit2D obstacleHit;
    private RaycastHit2D roamHit;
    private RaycastHit2D backwardHit;
    private RaycastHit2D attackHit;

    [Header("GameObject")]
    public GameObject Player;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    void Awake() {
        Roam_Next();
    }

    void Update() {
        if(!isDetectPlayer) {
            Forward_DetectPlayer();
            Backward_DetectPlayer();
        }
        Roam();
        Following_Player();
    }

    #region Collider

    void OnCollisionEnter2D(Collision2D other) {
        if(LayerMask.LayerToName(other.gameObject.layer) == "Player") {    
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            player_Component = other.gameObject.GetComponent<TestPlayerControl>();
            
            if(player_Component.isMagneting) {
                hp--;
            }

            if(!player_Component.isHitting && player_Component.isHitAllow && !player_Component.isFrictioning) {
                player_Component.Hp -= crashDamage;
                player_Component.rb.velocity = Vector2.zero;

                if(other.transform.position.x >= this.transform.position.x) {
                    player_Component.rb.velocity = crashPush;
                    Debug.Log("crash");
                }
                else {
                    player_Component.rb.velocity = new Vector2(crashPush.x * -1, crashPush.y);
                    Debug.Log("crash");
                }
            }
            Debug.Log("player");
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        if(LayerMask.LayerToName(other.gameObject.layer) == "Player") {    
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            player_Component = other.gameObject.GetComponent<TestPlayerControl>();
            
            if(player_Component.isMagneting) {
                hp--;
            }

            if(!player_Component.isHitting && player_Component.isHitAllow && !player_Component.isFrictioning) {
                player_Component.Hp -= crashDamage;
                player_Component.rb.velocity = Vector2.zero;

                if(other.transform.position.x >= this.transform.position.x) {
                    player_Component.rb.velocity = crashPush;
                    Debug.Log("crash");
                }
                else {
                    player_Component.rb.velocity = new Vector2(crashPush.x * -1, crashPush.y);
                    Debug.Log("crash");
                }
            }
            Debug.Log("player");
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
            case "Weapon" :
                switch(other.gameObject.tag) {
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
                        isBinded = true;
                        Debug.Log("Bind");
                        break;

                    case "blackholeBomb" :
                        hp -= other.gameObject.GetComponent<Bomb>().bombDamage;
                        Debug.Log("Bomb");
                        break;
                }
            break;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {
            case "Weapon" :
                switch(other.gameObject.tag) {
                    case "absoluteBind" :
                        bind = other.GetComponent<Bind>();
                        Invoke("Bind_Delay", bind.bindTime);
                        break;

                    case "blackholeBomb" :
                        Debug.Log("Bomb");
                        hp -= 5;
                        break;
                }
            break;
        }
    }

    #endregion

    #region Interaction

    void Bind_Delay() {
        isMoveAllow = true;
        isBinded = false;
    }

    void Roam() {
        if(isMoveAllow && !isDetectPlayer) {
            rb.velocity = new Vector2(roamNext * moveSpeed, rb.velocity.y);
        }
        roamVec = new Vector2(rb.position.x + roamNext * 0.4f, rb.position.y);

        roamHit = Physics2D.Raycast(roamVec, Vector3.down, 1, groundLayer);
        obstacleHit = Physics2D.Raycast(rb.position, Vector2.right * roamNext, 0.52f, groundLayer);


        if(roamHit.collider == null || obstacleHit.collider != null) {
            roamNext *= -1;
            CancelInvoke();
            Invoke("Roam_Next", 5);
        }
    }

    void Roam_Next() {
        roamNext = Random.Range(-1, 2);
        nextRaomTime = Random.Range(5f, 8f);

        if(roamNext < 0) {
            isFacingRight = false;
        }
        else {
            isFacingRight = true;
        }

        Invoke("Roam_Next", nextRaomTime);
    }

    bool Forward_DetectPlayer() {
        if(roamNext == 0) {
            if(isFacingRight) {
                facingIndex = 1;
            }
            else {
                facingIndex = -1;
            }
        }
        else {
            facingIndex = roamNext;
        }

        for(int i = 0; i < forwardRayCount; i++) {
            angleDetect = i * angleDetectValue;
            Vector2 direction = new Vector2(Mathf.Cos(angleDetect * Mathf.Deg2Rad) * facingIndex, Mathf.Sin(angleDetect * Mathf.Deg2Rad));
            RaycastHit2D[] forwardHit = Physics2D.RaycastAll(transform.position, direction, forwardRayDistance, playerLayer);
                
            Debug.DrawRay(transform.position, direction * forwardRayDistance, Color.green, 0.3f);
            forwardHitResults.AddRange(forwardHit);
        }

        foreach (RaycastHit2D forwardHit in forwardHitResults) {
            if(forwardHit.collider != null) {
                Debug.Log("DetectPlayer");
                Player = forwardHit.collider.gameObject;
                StartCoroutine(Follow_Condition());
                return true;
            }
        }

        return false;
    }

    void Backward_DetectPlayer() {
        backwardHit = Physics2D.Raycast(transform.position, -facingIndex * Vector2.right, backwardRayDistance, playerLayer);
        Debug.DrawRay(transform.position, -facingIndex * Vector2.right * backwardRayDistance, Color.green, 0.3f);
        if(backwardHit.collider != null) {
            Player = backwardHit.collider.gameObject;
            StartCoroutine(Follow_Condition());
            roamNext *= -1;
        }
    }

    IEnumerator Follow_Condition() {
        Debug.Log("Follow_Conndition");
        forwardHitResults.Clear();
        isFollowing = true;
        isDetectPlayer = true;

        yield return new WaitForSeconds(DetectTime);

        if(Forward_DetectPlayer()) {
            StartCoroutine(Follow_Condition());
        }
        else {
            isFollowing = false;
            isDetectPlayer = false;
        }
    }

    void Following_Player() {
        if(isFollowing) {
            roamVec = new Vector2(rb.position.x + roamNext * 0.4f, rb.position.y);
            roamHit = Physics2D.Raycast(roamVec, Vector3.down, 1, groundLayer);


            if(roamHit.collider != null && isMoveAllow) {
                Debug.Log("Move");
                if(Player.transform.position.x <= this.transform.position.x) {
                    rb.velocity = new Vector2(moveSpeed * -1.5f, rb.velocity.y);
                    attackHit = Physics2D.Raycast(transform.position, -Vector2.right, attackRayDistance, playerLayer);
                }
                else {
                    rb.velocity = new Vector2(moveSpeed * 1.5f, rb.velocity.y);
                    attackHit = Physics2D.Raycast(transform.position, Vector2.right, attackRayDistance, playerLayer);
                }
            }

            if(attackHit.collider) {
                rb.velocity = Vector2.zero;
                StartCoroutine(Attack_Player());
            }
        }   
    }

    #endregion

    #region Attack

    IEnumerator Attack_Player() {
        isFollowing = false;
        isMoveAllow = false;
        isAttacking = true;

        yield return new WaitForSeconds(attackReadyTime);

        Debug.Log("Attack");

        yield return new WaitForSeconds(attackDelayTime);

        isFollowing = true;
        isMoveAllow = true;
        isAttacking = false;
    }

    #endregion
}





 