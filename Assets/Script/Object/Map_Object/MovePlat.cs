using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlat : MonoBehaviour {
    [Header("Component")]
    public Rigidbody2D rb;

    [Header("Status")]
    public float moveSpeed = 5f;
    public int moveNext;

    void Start() {
        StartCoroutine(changeMove());
    }

    void FixedUpdate() {
        if(this.gameObject.tag != "UpPlat") {
            rb.velocity = new Vector2(moveSpeed * moveNext, rb.velocity.y);
        }
        else {
            rb.velocity = new Vector2(rb.velocity.x, moveSpeed * moveNext);
        }
    }

    IEnumerator changeMove() {
        moveNext = -1;

        yield return new WaitForSeconds(2f);

        moveNext = 0;

        yield return new WaitForSeconds(1f);

        moveNext = 1;

        yield return new WaitForSeconds(2f);

        moveNext = 0;

        yield return new WaitForSeconds(1f);

        StartCoroutine(changeMove());
    }
}