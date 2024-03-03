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
        StartCoroutine(moveLR());
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


}
