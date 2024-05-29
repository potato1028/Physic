using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TestPlayerControl : MonoBehaviour {
    private static TestPlayerControl instance;

    [Header("Player_Status")]
    public float horiaontalInput;
    public float verticalInput;
    public float Hp;
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashForce = 30f;
    public int groundRayCount = 9;
    public float groundRayThickness;
    public float wallRayThickness;
    //
    public float absoluteDelayTime = 5.0f;
    public float blackholeDelayTime = 5.0f;
    public float centrifugalDelayTime = 2.0f;
    public float dashDelayTime = 0.5f;
    public float frictionDelayTime = 2.0f;
    public float hitDelayTime = 2.0f;
    public float magneticDelayTime = 2.0f;
    public float repulsiveDelayTime = 2.0f;
    public float moveDelayTime = 1.0f;
    //
    public float absoluteRunTime = 2.0f;
    public float blackholeRunTime = 2.0f;
    public float dashRunTime = 0.2f;
    public float frictionRunTime = 0.5f;
    public float magneticRunTime  = 1.0f;
    //
    public float absoluteZeroTime = 2.0f;
    public float absoluteZeroSpeed = 5.0f;
    //
    public float blackholeBombTime = 2.0f;
    public float blackholeBombSpeed = 5.0f;
    //
    public float currentCentrifugalChargeForce;
    public float maxCentrifugalChargeForce = 3.0f;
    public float CentrifugalChargeSpeed = 2.0f;
    public int centrifugalForce;
    public float centrifugalSpeed = 5.0f;
    //
    public float repulsiveSpeed = 30.0f;
    //
    public float magnetMaxTime = 0.3f;
    public Vector2 magnetDistance;
    public float magnetMoveTime;
    //
    public float attractionFace = 10f;
    public Vector2 surefaceDirection;


    [Header("Player_Component")]
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public CapsuleCollider2D capsule2D;

    [Header("Others_Component")]
    public Bind bind;
    public Bomb bomb;

    [Header("Player_Condition")]
    public RaycastHit2D[] isGroundeds = new RaycastHit2D[10];
    public bool[] isLeftWalls = new bool[10];
    public bool[] isRightWalls = new bool[10];
    public bool isGrounded;
    public bool isCoyoteLeft;
    public bool isCoyoteRight;
    public bool isFacingRight;
    public bool isAttachedToLeftWall;
    public bool isAttachedToRightWall;
    public bool isOnPlat = false;
    public bool isAttachedMagnet = false;
    //
    public bool isAbsoluteAllow = true;
    public bool isBlackHoleAllow = true;
    public bool isCentrifugalAllow = true;
    public bool isDashAllow = true;
    public bool isFrictionAllow = true;
    public bool isHitAllow = true;
    public bool isMagneticAllow = true;
    public bool isMoveAllow = true;
    public bool isRedCondition = false;
    public bool isBlueCondition = true;
    public bool isRepulsiveAllow = true;
    public bool isSurefaceAllow = true;
    //
    public bool isAbsoluting = false;
    public bool isBlackHoling = false;
    public bool isCentrifugaling = false;
    public bool isDashing = false;
    public bool isFrictioning = false;
    public bool isHitting = false;
    public bool isMagneting = false;
    public bool isRepulsiving = false;
    public bool isSurefacing = false;

    [Header("Player_Item")]
    public GameObject centrifugalBullet_prefab;
    public GameObject repulsive_prefab;
    public GameObject absolute_prefab;
    public GameObject blackhole_prefab;
    public Text centrifugalForceText;
    //
    public GameObject absoluteBind;
    public GameObject blackholeBomb;
    public GameObject onPlat;

    [Header("RayCast")]
    Vector2 moveDirection;
    Vector2 groundRayVec;
    Vector2 wallRayVec;
    Vector2 CoyoteLeftVec;
    Vector2 CoyoteRightVec;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private bool isPaused = false;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this) {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        Debug.Log("Scene Change");
        groundLayer = LayerMask.GetMask("Ground", "Wall", "Plat");
        wallLayer = LayerMask.GetMask("Ground", "Wall", "Plat");
    }

    void Update() {
        Jump();
        Flip();

        // Absolute_Zero();
        // BlackHole_Bomb();
        // Centrifugal_Force();
        Dash();
        // Dry_Friction();
        // Repulsive_Push();
        // Magnetic_Change();

        UpdateText();

        if(Input.GetKeyDown(KeyCode.M)) {
            GamePuased();
        }
    }

    void FixedUpdate() {
        isPlayerGround();
        
        isFacingRight = transform.localScale.x > 0;
        horiaontalInput = Input.GetAxis("Horizontal");

        //Move
        if(isMoveAllow 
            && !isAbsoluting && !isBlackHoling && !isDashing&& !isFrictioning && !isMagneting && !isSurefacing) {
            if(isAttachedToLeftWall && horiaontalInput < 0) {
                moveDirection.x = 0f;
            }
            else if(isAttachedToRightWall && horiaontalInput > 0) {
                moveDirection.x = 0f;
            }
            else {
                moveDirection = new Vector2(horiaontalInput, 0);
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            }
        }
        else if(isMoveAllow 
            && !isAbsoluting && !isBlackHoling && !isDashAllow && !isFrictioning && !isMagneting && isSurefacing) {
            verticalInput = Input.GetAxis("Vertical");
            if(isAttachedToLeftWall && horiaontalInput < 0) {
                moveDirection.x = 0f;
            }
            else if(isAttachedToRightWall && horiaontalInput > 0) {
                moveDirection.x = 0f;
            }
            else {
                moveDirection = new Vector2(horiaontalInput, verticalInput);
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            }
        }

        //isGround && isWall &&isOnPlat
    }

    void isPlayerGround() {
        wallRayThickness = -0.8f;
        groundRayThickness = -0.4f;
        for(int i = 0; i < groundRayCount; i++) {
            groundRayVec = new Vector2(transform.position.x + groundRayThickness, transform.position.y - 1.0f);
            isGroundeds[i] = Physics2D.Raycast(groundRayVec, Vector2.down, 0.01f, groundLayer);
            Debug.DrawRay(groundRayVec, Vector2.down * 0.01f, Color.green);
            if(isGroundeds[i].collider != null) {
                isGrounded = true;
                break;
            }
            else {
                isGrounded = false;
            }
            groundRayThickness += 0.1f;
        }

        for(int i = 0; i < 8; i++) {
            wallRayVec = new Vector2(transform.position.x, transform.position.y + wallRayThickness);
            isLeftWalls[i] = Physics2D.Raycast(wallRayVec, Vector2.left, 0.52f, wallLayer);
            isRightWalls[i] = Physics2D.Raycast(wallRayVec, Vector2.right, 0.52f, wallLayer);
            Debug.DrawRay(wallRayVec, Vector2.left * 0.52f, Color.green);
            Debug.DrawRay(wallRayVec, Vector2.right * 0.52f, Color.green);
            if(isLeftWalls[i]) {
                isAttachedToLeftWall = true;
                break;
            }
            else {
                isAttachedToLeftWall = false;
            }

            if(isRightWalls[i]) {
                isAttachedToRightWall = true;
                break;
            }
            else {
                isAttachedToRightWall = false;
            }

            wallRayThickness += 0.2f;
        }

        if(!isAttachedToLeftWall) {
            CoyoteLeftVec = new Vector2(this.transform.position.x - 0.52f, this.transform.position.y);
            isCoyoteLeft = Physics2D.Raycast(CoyoteLeftVec, Vector2.down, 1.01f, groundLayer);
            Debug.DrawRay(CoyoteLeftVec, Vector2.down * 1.01f, Color.green);
            if(isCoyoteLeft) {
                isGrounded = true;
            }
            else {
                isGrounded = false;
            }
        }
        if(!isAttachedToRightWall) {
            CoyoteRightVec = new Vector2(this.transform.position.x + 0.52f, this.transform.position.y);
            isCoyoteRight = Physics2D.Raycast(CoyoteRightVec, Vector2.down, 1.01f, groundLayer);
            Debug.DrawRay(CoyoteRightVec, Vector2.down * 1.01f, Color.green);
            if(isCoyoteRight) {
                isGrounded = true;
            }
            else {
                isGrounded = false;
            }
        }
    }

    void UpdateText() {
        centrifugalForceText.text = "centrifugalForce" + (int)(currentCentrifugalChargeForce + 1f);
    }

    #region PlayerMove

    void Jump() {
        if(isGrounded && Input.GetButtonDown("Jump") && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip() {
        if(horiaontalInput > 0 && !isFacingRight && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if(horiaontalInput < 0 && isFacingRight && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting) {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }

    #endregion


    #region Interaction

    void OnCollisionEnter2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "hostileOrg" :
                switch(other.gameObject.tag) {
                    case "BasicEnemy" :
                        Hit();
                        break;
                    case "FlyingEnemy" :
                        Hit();
                        break;
                }
                break;

            case "Ground" :
                if(isDashing) {
                    Before_Dash();
                }
                break;

            case "Plat" :
                switch(other.gameObject.tag) {
                    case "LRPlat" :
                        Debug.Log("On" + other.gameObject.tag);
                        break;
                    
                    case "UDPlat" :
                        Debug.Log("On" + other.gameObject.tag);
                        break;
                }
                break;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "hostileOrg" :
                switch(other.gameObject.tag) {
                    case "BasicEnemy" :
                        Hit();
                        break;
                    case "FlyingEnemy" :
                        Hit();
                        break;
                }
                break;
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {
            case "Ground" :
                switch(other.gameObject.tag) {
                    case "Plat" :
                        isOnPlat = false;
                        break;
                }
                break;
            }
    }

    void OnTriggerEnter2D(Collider2D other) {
        switch(LayerMask.LayerToName(other.gameObject.layer)) {    
            case "hostileObj" :
                switch(other.gameObject.tag) {
                    case "Laser" :
                        Hit();
                        break;
                    case "Lava" :
                        rb.velocity = Vector2.zero;
                        StartCoroutine(moveDelay());
                        break;
                }
                break;
        }
    }

    void Hit() {
        if(isDashing) {
            isDashing = false;
            rb.gravityScale = 2f;
            Invoke("Before_Dash", moveDelayTime);
        }

        if(!isHitting && isHitAllow && !isFrictioning) {
            currentCentrifugalChargeForce = 0;
            StartCoroutine(hitDelay());
            StartCoroutine(moveDelay());
        }
        else if(isFrictioning) {
            Debug.Log("Dry_Friction");
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
        && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting && !isSurefacing) {
            Debug.Log("Absolute_Zero_Charging");
            rb.velocity = Vector2.zero;
            absoluteBind = Instantiate(absolute_prefab, new Vector2(transform.position.x, transform.position.y + 1.3f), Quaternion.identity);
            absoluteBind.transform.parent = this.gameObject.transform;
            bind = absoluteBind.GetComponent<Bind>();
            StartCoroutine(absoluteDelay());
            StartCoroutine(absoluteRunning());
        }
    }

    void BlackHole_Bomb() {
        if(Input.GetKey(KeyCode.Q) && isBlackHoleAllow
        && !isAbsoluting && !isDashing && !isFrictioning && !isHitting && !isMagneting && !isSurefacing) {
            Debug.Log("BlackHole_Charging");
            rb.velocity = Vector2.zero;
            blackholeBomb = Instantiate(blackhole_prefab, new Vector2(transform.position.x, transform.position.y + 1.3f), Quaternion.identity);
            blackholeBomb.transform.parent = this.gameObject.transform;
            bomb = blackholeBomb.GetComponent<Bomb>();
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

    void Dash() {
        if(Input.GetKeyDown(KeyCode.LeftShift) && isDashAllow
            && !isAbsoluting && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting) {

                float xDash = 0f, yDash = 0f;

                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0f;

                    if(Input.GetKey(KeyCode.W)) {
                        if(Input.GetKey(KeyCode.D)) {
                            xDash = (dashForce / 2f);
                            yDash = (dashForce / 2f);
                        }
                        else if(Input.GetKey(KeyCode.A)) {
                            xDash = -(dashForce / 2f);
                            yDash = (dashForce / 2f);
                        }
                        else {
                            xDash = 0f;
                            yDash = dashForce;
                        }
                    }
                    else if(Input.GetKey(KeyCode.S)) {
                        if(Input.GetKey(KeyCode.D)) {
                            xDash = (dashForce / 2f);
                            yDash = -(dashForce / 2f);
                        }
                        else if(Input.GetKey(KeyCode.A)) {
                            xDash = -(dashForce / 2f);
                            yDash = -(dashForce / 2f);
                        }
                        else {
                            xDash = 0f;
                            yDash = -dashForce;
                        }
                    }
                    else {
                        if(Input.GetKey(KeyCode.A)) {
                            xDash = -dashForce;
                            yDash = 0f;
                        }
                        else if(Input.GetKey(KeyCode.D)) {
                            xDash = dashForce;
                            yDash = 0f;
                        }
                        else {
                            return;
                        }
                    }

                    rb.velocity = new Vector2(xDash, yDash);

                    StartCoroutine(dashDelay());
                    StartCoroutine(dashRunning());
                }
            }
    }

    void Dry_Friction() {
        if(Input.GetKeyDown(KeyCode.R) && isFrictionAllow && isGrounded && isMoveAllow
        && !isDashing && !isHitting && !isMagneting) {
            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("none"));
        }
        else if(Input.GetKeyDown(KeyCode.R) && isFrictionAllow && isGrounded && isAbsoluting && !isDashing) {
            Before_Absolute();
            StartCoroutine(absoluteDelay());

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("Absolute"));
        }
        else if(Input.GetKeyDown(KeyCode.R) && isFrictionAllow && isGrounded && isBlackHoling && !isDashing) {
            Before_BlackHole();
            StartCoroutine(blackholeDelay());

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("BlackHole"));
        }
        else if(Input.GetKeyDown(KeyCode.R) && isFrictionAllow && isGrounded && isCentrifugaling && !isDashing) {
            isCentrifugaling = false;

            Debug.Log("Friction!");
            StartCoroutine(frictionDelay());
            StartCoroutine(frictionRunning("none"));
        }
    }

    public void Magnetic_Move(Vector2 MagnetPosition) {
        if(isMagneticAllow && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isFrictioning && !isHitting) {
            magnetDistance = MagnetPosition - rb.position;
            magnetMoveTime = magnetDistance.magnitude / rb.velocity.magnitude;

            if(magnetMoveTime > magnetMaxTime) {
                magnetMoveTime = 1f;
            }

            rb.velocity = magnetDistance.normalized * (magnetDistance.magnitude / magnetMaxTime);

            StartCoroutine(magneticDelay());
            StartCoroutine(magneticRunning());
        }
    }

    void Magnetic_Change() {
        if(Input.GetKeyDown(KeyCode.X)) {
            isRedCondition = !isRedCondition;
            isBlueCondition = !isBlueCondition;
        }
    }

    void Repulsive_Push() {
        if(Input.GetKeyDown(KeyCode.F) && isRepulsiveAllow && isMoveAllow
            && !isAbsoluting && !isBlackHoling && !isDashing && !isFrictioning && !isHitting && !isMagneting) {
            if(isFacingRight) {
                GameObject repulsive = Instantiate(repulsive_prefab, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
            }
            else {
                GameObject repulsive = Instantiate(repulsive_prefab, new Vector2(transform.position.x - 0.4f, transform.position.y), Quaternion.identity);
            }
            StartCoroutine(repulsiveDelay());
        }
    }

     public void SureFace_Water() {
        if(isSurefacing && !isMagneting) {
            Debug.Log("Gravity 0");
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.gravityScale = 4f;
        }
        else if(!isSurefacing && !isMagneting){
            rb.velocity = new Vector2(rb.velocity.x * 5f, rb.velocity.y * 3f);
            rb.gravityScale = 2f;
        }
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

    IEnumerator dashDelay() {
        isDashAllow = false;

        yield return new WaitForSeconds(dashDelayTime);

        isDashAllow = true;
    }

    IEnumerator frictionDelay() {
        isFrictionAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(frictionDelayTime);

        isFrictionAllow = true;
    }

    IEnumerator magneticDelay() {
        isMagneticAllow = false;
        currentCentrifugalChargeForce = 0f;

        yield return new WaitForSeconds(magneticDelayTime);

        isMagneticAllow = true;
    }

    IEnumerator repulsiveDelay() {
        isRepulsiveAllow = false;

        yield return new WaitForSeconds(repulsiveDelayTime);

        isRepulsiveAllow = true;
    }

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

    public void After_Absolute() {
        isAbsoluting = true;

        isBlackHoleAllow = false;
        isCentrifugalAllow = false;
        isMagneticAllow = false;
        isMoveAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;

    }

    public void Before_Absolute() {
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
            bomb.Shoot();
            Before_BlackHole();
        }
    }

    public void After_BalckHole() {
        isBlackHoling = true;

        isAbsoluteAllow = false;
        isCentrifugalAllow = false;
        isMagneticAllow = false;
        isMoveAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;
    }

    public void Before_BlackHole() {
        isBlackHoling = false;

        isAbsoluteAllow = true;
        isCentrifugalAllow = true;
        isMagneticAllow = true;
        isMoveAllow = true;
        isRepulsiveAllow = true;
        isSurefaceAllow = true;
    }

    IEnumerator dashRunning() {
        After_Dash();

        yield return new WaitForSeconds(dashRunTime);

        if(isDashing) {
            Before_Dash();
        }
    }

    public void After_Dash() {
        isDashing = true;

        isAbsoluteAllow = false;
        isBlackHoleAllow = false;
        isCentrifugalAllow = true;
        isFrictionAllow = false;
        isMagneticAllow = true;
        isMoveAllow = true;
        isRepulsiveAllow = false;
        isSurefaceAllow = true;
    }

    public void Before_Dash() {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        isDashing = false;

        isAbsoluteAllow = true;
        isBlackHoleAllow = true;
        isCentrifugalAllow = true;
        isFrictionAllow = true;
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

                if(blackholeBomb != null) {
                    Destroy(blackholeBomb);
                }

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

    IEnumerator magneticRunning() {
        After_Magnet();
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(magneticRunTime);

        Before_Magnet();
        rb.gravityScale = 2f;
    }

    public void After_Magnet() {
        isMagneting = true;
        capsule2D.isTrigger = true;

        isAbsoluteAllow = false;
        isBlackHoleAllow = false;
        isCentrifugalAllow = false;
        isHitAllow = false;
        isMoveAllow = false;
        isRepulsiveAllow = false;
        isSurefaceAllow = false;
    }

    public void Before_Magnet() {
        isMagneting = false;
        capsule2D.isTrigger = false;

        isAbsoluteAllow = true;
        isBlackHoleAllow = true;
        isCentrifugalAllow = true;
        isHitAllow = true;
        isMoveAllow = true;
        isRepulsiveAllow = true;
        isSurefaceAllow = true;
    }

    #endregion

    public void GamePuased() {
        if(isPaused) {
            Time.timeScale = 1f;
            isPaused = false;
        }
        else {
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}