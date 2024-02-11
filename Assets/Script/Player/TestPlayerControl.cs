using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestPlayerControl : MonoBehaviour {
    [Header("Player_Status")]
    public float horiaontalInput;
    public float Hp;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    //
    public float absoluteDelayTime = 5.0f;
    public float blackholeDelayTime = 5.0f;
    public float centrifugalDelayTime = 2.0f;
    public float frictionDelayTime = 2.0f;
    public float hitDelayTime = 2.0f;
    public float magneticDelayTime = 2.0f;
    public float repulsiveDelayTime = 2.0f;
    public float surefaceDelayTime = 2.0f;
    public float moveDelayTime = 1.0f;
    //
    public float absoluteRunTime = 2.0f;
    public float blackholeRunTime = 2.0f;
    public float frictionRunTime = 0.5f;
    public float magneticRunTime  = 1.0f;
    public float surefaceRunTime = 5.0f;
    //
    public float absoluteZeroTime = 2.0f;
    public float absoluteZeroSpeed = 5.0f;
    //
    public float currentCentrifugalChargeForce;
    public float maxCentrifugalChargeForce = 3.0f;
    public float CentrifugalChargeSpeed = 2.0f;
    public int centrifugalForce;
    public float centrifugalSpeed = 5.0f;
    //
    public float repulsiveSpeed = 30.0f;


    [Header("Player_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public CapsuleCollider2D capsule2D;

    [Header("Others_Component")]
    public Bind bind;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask springLayer;
    public LayerMask enemyLayer;
    public LayerMask enemyBulletLayer;

    [Header("Player_Condition")]
    public bool[] isGroundeds = new bool[3];
    public bool isGrounded;
    public bool isFacingRight;
    public bool isAttachedToWall;
    //
    public bool isAbsoluteAllow = true;
    public bool isBlackHoleAllow = true;
    public bool isCentrifugalAllow = true;
    public bool isFrictionAllow = true;
    public bool isHitAllow = true;
    public bool isMagneticAllow = true;
    public bool isMoveAllow = true;
    public bool isRepulsiveAllow = true;
    public bool isSurefaceAllow = true;
    //
    public bool isAbsoluting = false;
    public bool isBlackHoling = false;
    public bool isCentrifugaling = false;
    public bool isFrictioning = false;
    public bool isHitting = false;
    public bool isMagneting = false;
    public bool isRepulsiving = false;
    public bool isSurefacing = false;

    [Header("Player_Item")]
    public GameObject centrifugalBullet_prefab;
    public GameObject repulsive_prefab;
    public GameObject absolute_prefab;
    public Text centrifugalForceText;
    //
    public GameObject absoluteBind;

    [Header("RayCast")]
    RaycastHit2D[] groundHits = new RaycastHit2D[3];
    Vector2 groundRay;
    Vector2 wallRay;
    public Vector3 mousePosition;


    void Update() {
        Jump();
        Flip();

        Absolute_Zero();
        BlackHole_Bomb();
        Centrifugal_Force();
        Dry_Friction();
        Repulsive_Push();

        UpdateText();
    }

    void FixedUpdate() {
        isFacingRight = transform.localScale.x > 0;
        horiaontalInput = Input.GetAxis("Horizontal");

        //Move
        if(!isAttachedToWall && isMoveAllow 
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting) {
            Vector2 moveDirection = new Vector2(horiaontalInput, 0);
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }

        //isGround
        float groundRayThickness = -0.4f;

        for(int i = 0; i < 3; i++) {
            groundRay = new Vector2(transform.position.x + groundRayThickness, transform.position.y);
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
        
        wallRay = new Vector2(transform.position.x, transform.position.y - 1.0f);
        isAttachedToWall = Physics2D.Raycast(wallRay, Vector2.right * transform.localScale.x, 0.51f, wallLayer);        
    }

    void UpdateText() {
        centrifugalForceText.text = "centrifugalForce" + (int)(currentCentrifugalChargeForce + 1f);
    }

    #region PlayerMove

    void Jump() {
        if(isGrounded && Input.GetButtonDown("Jump") && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip() {
        if(horiaontalInput > 0 && !isFacingRight && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if(horiaontalInput < 0 && isFacingRight && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting) {
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
                    if(isMoveAllow) {
                        rb.velocity = new Vector2(rb.velocity.x, 20f);
                    }
                    
                }
                break;

            case "enemyBullet" :
                if(!isHitting && isHitAllow && !isFrictioning) {
                    Debug.Log("enemyBullet");
                    
                    if(other.transform.position.x < this.transform.position.x) {
                        rb.velocity = new Vector2(7f, 4.5f);
                    }
                    else {
                        rb.velocity = new Vector2(-7f, 4.5f);
                    }
                    currentCentrifugalChargeForce = 0;
                    StartCoroutine(hitDelay());
                    StartCoroutine(moveDelay());
                }
                else if(isFrictioning) {
                    Debug.Log("Dry_Friction");
                }
                break;
            }
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                if(!isHitting && isHitAllow && !isFrictioning) {
                    Debug.Log("Enemy");
                    if(other.transform.position.x < this.transform.position.x) {
                        rb.velocity = new Vector2(7f, 4.5f);
                    }
                    else {
                        rb.velocity = new Vector2(-7f, 4.5f);
                    }
                    currentCentrifugalChargeForce = 0;
                    StartCoroutine(hitDelay());
                    StartCoroutine(moveDelay());
                }
                
                else if(isFrictioning) {
                    Debug.Log("Dry_Friction");
                }
                break;
            }
    }

    void OnCollisionStay2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "Enemy" :
                if(!isHitting && isHitAllow && !isFrictioning) {
                    Debug.Log("Enemy");
                    if(other.transform.position.x < this.transform.position.x) {
                        rb.velocity = new Vector2(7f, 4.5f);
                    }
                    else {
                        rb.velocity = new Vector2(-7f, 4.5f);
                    }
                    currentCentrifugalChargeForce = 0;
                    StartCoroutine(hitDelay());
                    StartCoroutine(moveDelay());
                }
                else if(isFrictioning) {
                    Debug.Log("Dry_Friction");
                }
                break;
            }
    }

    IEnumerator hitDelay() {
        isHitting = true;
        isHitAllow = false;

        Color color = sp.color;
        color.a = 0.5f;
        sp.color = color;

        yield return new WaitForSeconds(hitDelayTime);

        isHitting = false;
        isHitAllow = true;

        color = sp.color;
        color.a = 1.0f;
        sp.color = color;
    }

    IEnumerator moveDelay() {
        isMoveAllow = false;

        isAbsoluteAllow = false;
        isBlackHoleAllow = false;
        isCentrifugalAllow = false;
        isFrictionAllow = false;
        isMagneticAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;
    
        yield return new WaitForSeconds(moveDelayTime);

        isMoveAllow = true;

        isAbsoluteAllow = true;
        isBlackHoleAllow = true;
        isCentrifugalAllow = true;
        isFrictionAllow = true;
        isMagneticAllow = true;
        isRepulsiveAllow = true;
        isSurefaceAllow = true;
    }

    #endregion

    #region PlayerSkill

    void Absolute_Zero() {
        if(Input.GetKey(KeyCode.E) && isAbsoluteAllow
        && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting && !isSurefacing) {
            Debug.Log("Absolute_Zero_Charging");
            absoluteBind = Instantiate(absolute_prefab, new Vector2(transform.position.x, transform.position.y + 1.3f), Quaternion.identity);
            absoluteBind.transform.parent = this.gameObject.transform;
            bind = absoluteBind.GetComponent<Bind>();
            StartCoroutine(absoluteDelay());
            StartCoroutine(absoluteRunning());
        }
    }

    void BlackHole_Bomb() {
        if(Input.GetKey(KeyCode.Q) && isBlackHoleAllow
        && !isAbsoluting && !isFrictioning && !isHitting && !isMagneting && !isSurefacing) {
            Debug.Log("BlackHole_Charging");
            StartCoroutine(blackholeDelay());
            StartCoroutine(blackholeRunning());
        }
    }

    void Centrifugal_Force() {
        if(Input.GetMouseButton(0) && isCentrifugalAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting && !isSurefacing) {
            isCentrifugaling = true;
            currentCentrifugalChargeForce += Time.deltaTime * CentrifugalChargeSpeed;
            currentCentrifugalChargeForce = Mathf.Min(currentCentrifugalChargeForce, maxCentrifugalChargeForce);
        }

        if(Input.GetMouseButtonUp(0)) {
            isCentrifugaling = false;
            centrifugalForce = (int)(currentCentrifugalChargeForce + 1f);
            
            Debug.Log(centrifugalForce);

            GameObject centrifugalBullet = Instantiate(centrifugalBullet_prefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            
            StartCoroutine(centrifugalDelay());
        }
    }

    void Dry_Friction() {
        if(Input.GetKeyDown(KeyCode.LeftShift) && isFrictionAllow && isGrounded && isMoveAllow
        && !isHitting && !isMagneting) {
            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("none"));
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && isFrictionAllow && isGrounded && isAbsoluting) {
            Before_Absolute();
            StartCoroutine(absoluteDelay());

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("Absolute"));
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && isFrictionAllow && isGrounded && isBlackHoling) {
            Before_BlackHole();
            StartCoroutine(blackholeDelay());

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("BlackHole"));
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && isFrictionAllow && isGrounded && isCentrifugaling) {
            isCentrifugaling = false;

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("none"));
        }
    }

    void Magnetic_Move() {
        
    }

    void Repulsive_Push() {
        if(Input.GetKeyDown(KeyCode.F) && isRepulsiveAllow && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting && !isMagneting) {
            if(isFacingRight) {
                GameObject repulsive = Instantiate(repulsive_prefab, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
            }
            else {
                GameObject repulsive = Instantiate(repulsive_prefab, new Vector2(transform.position.x - 0.4f, transform.position.y), Quaternion.identity);
            }
            StartCoroutine(repulsiveDelay());
        }
    }

    void SureFace_Water() {

    }

    IEnumerator absoluteDelay() {
        isAbsoluteAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(absoluteDelayTime);

        isAbsoluteAllow = true;
    }

    IEnumerator blackholeDelay() {
        isBlackHoleAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(blackholeDelayTime);

        isBlackHoleAllow = true;
    }

    IEnumerator centrifugalDelay() {
        isCentrifugalAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(centrifugalDelayTime);

        isCentrifugalAllow = true;
    }

    IEnumerator frictionDelay() {
        isFrictionAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(frictionDelayTime);

        isFrictionAllow = true;
    }

    // IEnumerator magneticDelay() {
    // }

    IEnumerator repulsiveDelay() {
        isRepulsiveAllow = false;

        yield return new WaitForSeconds(repulsiveDelayTime);

        isRepulsiveAllow = true;
    }

    // IEnumerator surefaceDelay() {
        
    // }

    /// ///////////////////////////////
    /// ///////////////////////////////
    /// ///////////////////////////////

    IEnumerator absoluteRunning() {

        After_Absolute();

        yield return new WaitForSeconds(absoluteRunTime);

        if(isAbsoluting) {
            Debug.Log("Absolute_Zero_Shoot!");
            bind.Shoot();
            Before_Absolute();  
        }
    }

    void After_Absolute() {
        isAbsoluting = true;

        isBlackHoleAllow = false;
        isCentrifugalAllow = false;
        isMagneticAllow = false;
        isMoveAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;

    }

    void Before_Absolute() {
        isAbsoluting = false;

        isBlackHoleAllow = true;
        isCentrifugalAllow = true;
        isMagneticAllow = true;
        isMoveAllow = true;
        isRepulsiveAllow = true;
        isSurefaceAllow = true;
    }


    IEnumerator blackholeRunning() {
        After_BalckHole();

        yield return new WaitForSeconds(blackholeRunTime);

        if(isBlackHoling) {
            Debug.Log("BlackHole_Shoot!!");
            
            Before_BlackHole();
        }

    }

    void After_BalckHole() {
        isBlackHoling = true;

        isAbsoluteAllow = false;
        isCentrifugalAllow = false;
        isMagneticAllow = false;
        isMoveAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;
    }

    void Before_BlackHole() {
        isBlackHoling = false;

        isAbsoluteAllow = true;
        isCentrifugalAllow = true;
        isMagneticAllow = true;
        isMoveAllow = true;
        isRepulsiveAllow = true;
        isSurefaceAllow = true;
    }


    IEnumerator frictionRunning(string skill) {
        isFrictioning = true;
        switch(skill) {
            case "none" :
                isAbsoluteAllow = false;
                isBlackHoleAllow = false;
                isCentrifugalAllow = false;
                isHitAllow = false;
                isMagneticAllow = false;
                isMoveAllow = false;
                isRepulsiveAllow = false;
                isSurefaceAllow = false;

                yield return new WaitForSeconds(frictionRunTime);

                Debug.Log("Friction_End");

                isAbsoluteAllow = true;
                isBlackHoleAllow = true;
                isCentrifugalAllow = true;
                isHitAllow = true;
                isMagneticAllow = true;
                isMoveAllow = true;
                isRepulsiveAllow = true;
                isSurefaceAllow = true;

                break;

            case "Absolute" :
                isBlackHoleAllow = false;
                isCentrifugalAllow = false;
                isHitAllow = false;
                isMagneticAllow = false;
                isMoveAllow = false;
                isRepulsiveAllow = false;
                isSurefaceAllow = false;

                if(absoluteBind != null) {
                    Destroy(absoluteBind);
                }

                yield return new WaitForSeconds(frictionRunTime);

                Debug.Log("Friction_End");

                isBlackHoleAllow = true;
                isCentrifugalAllow = true;
                isHitAllow = true;
                isMagneticAllow = true;
                isMoveAllow = true;
                isRepulsiveAllow = true;
                isSurefaceAllow = true;

                break;

            case "BlackHole" :
                isAbsoluteAllow = false;
                isCentrifugalAllow = false;
                isHitAllow = false;
                isMagneticAllow = false;
                isMoveAllow = false;
                isRepulsiveAllow = false;
                isSurefaceAllow = false;

                yield return new WaitForSeconds(frictionRunTime);

                Debug.Log("Friction_End");

                isAbsoluteAllow = true;
                isCentrifugalAllow = true;
                isHitAllow = true;
                isMagneticAllow = true;
                isMoveAllow = true;
                isRepulsiveAllow = true;
                isSurefaceAllow = true;

                break;

        }
        isFrictioning = false;
    }

    // IEnumerator magneticRunning() {
    // }

    // IEnumerator surefaceRunning() {
    // }

    #endregion
}