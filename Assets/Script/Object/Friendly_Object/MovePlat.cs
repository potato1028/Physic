using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlat : MonoBehaviour {
    [Header("Status")]
    public float moveSpeed;
    public float moveTime;
    public float stopTime;
    public float startDelayTime = 0f;
    public Vector2 moveVector;
    public bool isR = false;
    public bool isU = false;

    [Header("Component")]
    public Rigidbody2D rb;

    void Start() {
        if(isR || isU) {
            moveSpeed *= -1;
        }
        if(this.gameObject.tag == "LRPlat") {
            StartCoroutine(moveLR());
        }
        else {
            StartCoroutine(moveUD());
        }
    }

    IEnumerator moveLR() {
        yield return new WaitForSeconds(startDelayTime);
        startDelayTime = 0f;

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
        yield return new WaitForSeconds(startDelayTime);
        startDelayTime = 0f;
        
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
