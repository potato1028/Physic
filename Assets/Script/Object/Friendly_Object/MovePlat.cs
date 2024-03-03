using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlat : MonoBehaviour {
    [Header("Status")]
    public float moveSpeed;
    public float moveTime;
    public float stopTime;
    public Vector2 moveVector;

    [Header("Component")]
    public Rigidbody2D rb;

    void Start() {
        if(this.gameObject.tag == "LRPlat") {
            StartCoroutine(moveLR());
        }
        else {
            StartCoroutine(moveUD());
        }
    }

    IEnumerator moveLR() {
        moveVector = new Vector2(-moveSpeed, 0);
        rb.velocity = moveVector;

        yield return new WaitForSeconds(moveTime);

        moveVector = Vector2.zero;
        rb.velocity = moveVector;

        yield return new WaitForSeconds(stopTime);

        moveVector = new Vector2(moveSpeed, 0);
        rb.velocity = moveVector;

        yield return new WaitForSeconds(moveTime);
        
        moveVector = Vector2.zero;
        rb.velocity = moveVector;

        yield return new WaitForSeconds(stopTime);

        StartCoroutine(moveLR());
    }

    IEnumerator moveUD() {
        moveVector = new Vector2(0, -moveSpeed);
        rb.velocity = moveVector;

        yield return new WaitForSeconds(moveTime);

        moveVector = Vector2.zero;
        rb.velocity = moveVector;

        yield return new WaitForSeconds(stopTime);

        moveVector = new Vector2(0, moveSpeed);
        rb.velocity = moveVector;

        yield return new WaitForSeconds(moveTime);
        
        moveVector = Vector2.zero;
        rb.velocity = moveVector;

        yield return new WaitForSeconds(stopTime);

        StartCoroutine(moveUD());
    }
}
