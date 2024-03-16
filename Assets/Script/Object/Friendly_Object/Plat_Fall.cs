using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plat_Fall : MonoBehaviour {
    [Header("Status")]
    public float fallTime;
    public bool isFallAllow = false;
    public bool isDetectPlayer = true;

    [Header("Component")]
    public BoxCollider2D box2D;
    public SpriteRenderer sp;
    public Color color;

    [Header("Layer")]
    public LayerMask playerLayer;

    [Header("Ray")]
    private RaycastHit2D[] onRay = new RaycastHit2D[11];

    void Awake() {
        color = sp.color;
        playerLayer = LayerMask.GetMask("Player");
    }

    void FixedUpdate() {
        Collider_Disabled();
        onPlayer();
    }

    void onPlayer() {
        if(isDetectPlayer) {
            float groundRayThickness = -1.8f;
        
            for(int i = 0; i < 11; i++) {
                onRay[i] = Physics2D.Raycast(new Vector2(transform.position.x + groundRayThickness, transform.position.y + 0.25f), Vector2.up, 0.01f, playerLayer);
                Debug.DrawRay(new Vector2(transform.position.x + groundRayThickness, transform.position.y + 0.25f), Vector2.up, Color.green);
                if(onRay[i].collider != null) {
                    isFallAllow = true;
                    isDetectPlayer = false;
                    break;
                }
                else {
                    isFallAllow = false;
                }
                groundRayThickness += 0.36f;
            }
        }
    }

    void Collider_Disabled() {
        if(isFallAllow) {
            color.a -= (1f / fallTime); 
            sp.color = color;
        }

        if(color.a <= 0) {
            isFallAllow = false;
            box2D.enabled = false;
            StartCoroutine(Delay_Collider());
        }
    }   

    IEnumerator Delay_Collider() {
        yield return new WaitForSeconds(2.0f);

        box2D.enabled = true;
        color.a = 1f;
        sp.color = color;
        isDetectPlayer = true;
    } 
}
