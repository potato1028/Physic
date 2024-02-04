using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spring : MonoBehaviour {
    [Header("Component")]
    public BoxCollider2D boxCollider2D;
    public SpriteRenderer sp;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay() {
        Color currentColor = sp.color;

        currentColor.a = 0.5f;
        sp.color = currentColor;
        boxCollider2D.enabled = false;

        yield return new WaitForSeconds(1.0f);

        currentColor.a = 1.0f;
        sp.color = currentColor;
        boxCollider2D.enabled = true;
    }
}