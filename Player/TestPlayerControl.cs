using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestPlayerControl : MonoBehaviour {
    [Header("Player_Status")]
    public float horiaontalInput;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float hitDelayTime = 2.0f;
    public float moveDelayTime = 1.0f;
    //
    public int xForce;
    public float xChargeTime = 3.0f;
    public float chargeRate = 1.0f;
    public float xDelayTime = 0.3f;
    private float currentChargeTime;

    [Header("Player_Component")]
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [Header("Player_Condition")]
    public bool isHit = false;
    public bool isMoveAllow = true;
    public bool isXAllow = true;
    public bool[] isGroundeds = new bool[3];
    public bool isGrounded;
    public bool isFacingRight;
    public bool isAttachedToWall;
    public bool isDashing = false;

    [Header("Player_Item")]
    public GameObject Bullet;

    void Update() {
        if(isMoveAllow) {
            Jump();
            Flip();
            Centrifugal_Force();
        }
    }

    void FixedUpdate() {
        isFacingRight = transform.localScale.x > 0;
        horiaontalInput = Input.GetAxis("Horizontal");

        //Move
        if(!isAttachedToWall && isMoveAllow) {
            Vector2 moveDirection = new Vector2(horiaontalInput, 0);
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }

        //isGround
        float groundRayThickness = -0.4f;

        RaycastHit2D[] groundHits = new RaycastHit2D[3];
        for(int i = 0; i < 3; i++) {
            Vector2 groundRay = new Vector2(transform.position.x + groundRayThickness, transform.position.y);
            isGroundeds[i] = Physics2D.Raycast(groundRay, Vector2.down, 1.01f, groundLayer);
            if(isGroundeds[i]) {
                isGrounded = true;
                break;
            }
            else {
                isGrounded = false;
            }
            groundRayThickness += 0.4f;
        }
        
        Vector2 wallRay = new Vector2(transform.position.x, transform.position.y - 1.0f);
        isAttachedToWall = Physics2D.Raycast(wallRay, Vector2.right * transform.localScale.x, 0.51f, wallLayer);
    }

    #region PlayerMove

    void Jump() {
        if(isGrounded && Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip() {
        if(horiaontalInput > 0 && !isFacingRight) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if(horiaontalInput < 0 && isFacingRight) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }

    #endregion


    #region Interaction

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {
            case "LexTertia" :
                Debug.Log("LexTertia");
                rb.velocity = new Vector2(rb.velocity.x, 20f);
                break;
            
            case "Enemy" :
                Debug.Log("Enemy");
                if(other.transform.position.x < this.transform.position.x) {
                    rb.velocity = new Vector2(7f, 4.5f);
                }
                else {
                    rb.velocity = new Vector2(-7f, 4.5f);
                }
                StartCoroutine(hitDelay());
                StartCoroutine(moveDelay());
                break;
        }
    }

    IEnumerator hitDelay() {
        isHit = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), isHit);

        yield return new WaitForSeconds(hitDelayTime);

        isHit = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), isHit);
    }

    IEnumerator moveDelay() {
        isMoveAllow = false;
        isXAllow = false;
        currentChargeTime = 0f;
    
        yield return new WaitForSeconds(moveDelayTime);

        isMoveAllow = true;
        isXAllow = true;
    }

    #endregion

    #region PlayerSkill

    void Centrifugal_Force() {
        if(Input.GetKey(KeyCode.Z) && isXAllow) {
            currentChargeTime += Time.deltaTime * chargeRate;
            currentChargeTime = Mathf.Min(currentChargeTime, xChargeTime);
        }

        if(Input.GetKeyUp(KeyCode.Z) && isXAllow) {
            xForce = (int)(currentChargeTime + 1f);
            
            Debug.Log(xForce);

            if(isFacingRight) {
                GameObject xBullet = Instantiate(Bullet, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
            }
            else {
                GameObject xBullet = Instantiate(Bullet, new Vector2(transform.position.x - 0.4f, transform.position.y), Quaternion.identity);
            }

            currentChargeTime = 0f;

            StartCoroutine(xDelay());
        }

        IEnumerator xDelay() {
            isXAllow = false;

            yield return new WaitForSeconds(xDelayTime);

            isXAllow = true;
        }
    }

    #endregion
}