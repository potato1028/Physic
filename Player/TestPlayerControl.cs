using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestPlayerControl : MonoBehaviour {
    [Header("Player_Status")]
    public float horiaontalInput;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float hitDelayTime = 2.0f;
    public float moveDelayTime = 1.0f;
    public float zDelayTime = 0.3f;
    //
    public float currentZChargeForce;
    public float maxZChargeForce = 3.0f;
    public float ZChargeSpeed = 2.0f;
    public int zForce;

    [Header("Player_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask springLayer;
    public LayerMask enemyLayer;
    public LayerMask enemyBulletLayer;

    [Header("Player_Condition")]
    public bool isHit = false;
    public bool isMoveAllow = true;
    public bool isZAllow = true;
    public bool[] isGroundeds = new bool[3];
    public bool isGrounded;
    public bool isFacingRight;
    public bool isAttachedToWall;

    [Header("Player_Item")]
    public GameObject zBullet_prefab;
    public Text zForceText;

    void Update() {
        Jump();
        Flip();
        Centrifugal_Force();
        UpdateText();
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

    void UpdateText() {
        zForceText.text = "zForce" + (int)(currentZChargeForce + 1f);
    }

    #region PlayerMove

    void Jump() {
        if(isGrounded && Input.GetButtonDown("Jump") && isMoveAllow) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip() {
        if(horiaontalInput > 0 && !isFacingRight && isMoveAllow) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if(horiaontalInput < 0 && isFacingRight && isMoveAllow) {
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
                if(isMoveAllow) {
                    Debug.Log("LexTertia");
                    rb.velocity = new Vector2(rb.velocity.x, 20f);
                }
                break;

            case "enemyBullet" :
                if(!isHit) {
                    Debug.Log("enemyBullet");
                    if(other.transform.position.x < this.transform.position.x) {
                        rb.velocity = new Vector2(7f, 4.5f);
                    }
                    else {
                        rb.velocity = new Vector2(-7f, 4.5f);
                    }
                    StartCoroutine(hitDelay());
                    StartCoroutine(moveDelay());
                }
                break;
            }
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                if(!isHit) {
                    Debug.Log("Enemy");
                    if(other.transform.position.x < this.transform.position.x) {
                        rb.velocity = new Vector2(7f, 4.5f);
                    }
                    else {
                        rb.velocity = new Vector2(-7f, 4.5f);
                    }
                    StartCoroutine(hitDelay());
                    StartCoroutine(moveDelay());
                }
                break;
            }
    }

    IEnumerator hitDelay() {
        isHit = true;

        Color color = sp.color;
        color.a = 0.5f;
        sp.color = color;

        yield return new WaitForSeconds(hitDelayTime);

        isHit = false;

        color = sp.color;
        color.a = 1.0f;
        sp.color = color;
    }

    IEnumerator moveDelay() {
        isMoveAllow = false;
        isZAllow = false;
        currentZChargeForce = 0;
    
        yield return new WaitForSeconds(moveDelayTime);

        isMoveAllow = true;
        isZAllow = true;
    }

    #endregion

    #region PlayerSkill

    void Centrifugal_Force() {
        if(Input.GetKey(KeyCode.Z) && isZAllow) {
            currentZChargeForce += Time.deltaTime * ZChargeSpeed;
            currentZChargeForce = Mathf.Min(currentZChargeForce, maxZChargeForce);
        }

        if(Input.GetKeyUp(KeyCode.Z)) {
            zForce = (int)(currentZChargeForce + 1f);
            
            Debug.Log(zForce);

            if(isFacingRight) {
                GameObject zBullet = Instantiate(zBullet_prefab, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
            }
            else {
                GameObject zBullet = Instantiate(zBullet_prefab, new Vector2(transform.position.x - 0.4f, transform.position.y), Quaternion.identity);
            }

            StartCoroutine(zDelay());
        }
    }

    IEnumerator zDelay() {
        isZAllow = false;
        currentZChargeForce = 0;

        yield return new WaitForSeconds(zDelayTime);

        isZAllow = true;
    }

    #endregion
}