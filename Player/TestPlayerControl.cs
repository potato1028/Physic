using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestPlayerControl : MonoBehaviour {
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    public Rigidbody2D rb;
    public LayerMask groundLayer;

    public bool isGrounded;
    public bool isFacingRight;
    public bool isDashing = false;

    public float horiaontalInput;

    void Update() {
        isFacingRight = transform.localScale.x > 0;
        horiaontalInput = Input.GetAxis("Horizontal");

        //Move
        Vector2 moveDirection = new Vector2(horiaontalInput, 0);
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        //Jump
        Jump();

        //Dash
        Dash();

        //Condition
        Flip();
    }

    void FixedUpdate() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.01f, groundLayer);
    }

    #region PlayerMove

    void Jump() {
        if(isGrounded && Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Dash() {
    }

    #endregion



    #region PlayerCondition

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
}