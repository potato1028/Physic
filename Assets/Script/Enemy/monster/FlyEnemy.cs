using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : MonoBehaviour {
    [Header("Status")]
    public int forwardRayCount = 9;
    public int facingIndex;
    public float forwardRayDistance = 3.5f;
    public float backwardRayDistance = 2.0f;
    public float attackRayDistance = 3.0f;
    public float detectRayDistance = 7.5f;
    public float angleDetect;
    public float angleDetectValue = 5f;
    public List<RaycastHit2D> forwardHitResults = new List<RaycastHit2D>();
    //
    public float hp = 10;
    public float moveSpeed = 2.0f;
    public float attackDelayTime = 1.5f;
    public float attackReadyTime = 0.5f;
    public float crashDamage = 1f;
    //
    public Vector2 crashPush;

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
    public float roamAnlge;
    public float nextRaomTime;
    public float DetectTime = 5f;
    public float moveX;
    public float moveY;

    [Header("RayCast")]
    private RaycastHit2D obstacleHit;
    private RaycastHit2D backwardHit;

    [Header("GameObject")]
    public GameObject Player;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    void Awake() {
        Roam_Next();
    }

    void Update() {
        if (!isDetectPlayer) {
            Forward_DetectPlayer();
            Backward_DetectPlayer();
            Roam();
        }
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
            
        }
        obstacleHit = Physics2D.Raycast(rb.position, Vector2.right * roamNext, 0.52f, groundLayer);


        if(obstacleHit.collider != null) {
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
        else if(roamNext > 0) {
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
        forwardHitResults.Clear();
        yield return new WaitForSeconds(0.0f);
    }

    #endregion

    #region Attack

    #endregion
}