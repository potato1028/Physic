using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnemySystem {

    public abstract class BasicEnemyClass : MonoBehaviour {
        [Header("Enemy_Status")]
        public int Hp;
        public float moveSpeed;
        public int facingIndex;
        public Vector2 roamVec;
        /////
        public float rayDistance;
        public float roamInterval;
        public List<RaycastHit2D> forwardHitResults = new List<RaycastHit2D>();
        

        [Header("Enemy_Component")]
        public Rigidbody2D rb;

        [Header("Enemy_Layer")]
        public LayerMask obstacleLayer;
        public LayerMask playerLayer;
        public LayerMask groundLayer;

        [Header("Enemy_Condition")]
        public bool isMoveAllow;
        public bool isDetectPlayer;
        public bool isFacingRight;
        public bool isAttacking;
        ///
        public int roamNext;
        public float nextRaomTime;

        [Header("Other_Object")]
        public GameObject Player;

        [Header("Enemy_ray")]
        public RaycastHit2D obstacleHit;
        public RaycastHit2D roamHit;
        
        public void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start() {
            isMoveAllow = true;
            isAttacking = false;
            Roam_Next();
        }

        public void Update() {
            Move();
            Chase();
            PlayerDetect();
            Attack_Range();
        }

        protected abstract void Move();

        protected abstract void Chase();

        protected abstract void Attack_Range();

        protected virtual void PlayerDetect() {
            if(roamNext == 0) {
                if(isFacingRight) {
                    facingIndex = 1;
                }
                else {
                    facingIndex = -1;
                }
            }
            else {
                facingIndex = roamNext;
            }

            if(!isDetectPlayer) {
                for(int i = -1; i < 2; i++) {
                    Vector2 roamVec = new Vector2(transform.position.x - (facingIndex * 2.0f), transform.position.y + (roamInterval * i));
                    RaycastHit2D[] forwardHit = Physics2D.RaycastAll(roamVec, Vector2.right * facingIndex, rayDistance, playerLayer);

                    Debug.DrawRay(roamVec, Vector2.right * facingIndex * rayDistance, Color.red, 0.3f);
                    forwardHitResults.AddRange(forwardHit);
                }

                foreach (RaycastHit2D forwardHit in forwardHitResults) {
                    if(forwardHit.collider != null) {
                        Debug.Log("DetectPlayer");
                        Player = forwardHit.collider.gameObject;
                        CancelInvoke("Roam_Next");
                        isDetectPlayer = true;
                        break;
                    }
                }
            }
        }

        protected virtual void Roam_Next() {
            roamNext = Random.Range(-1, 2);
            nextRaomTime = Random.Range(5f, 8f);

            if(roamNext < 0) {
                isFacingRight = false;
            }
            else if(roamNext > 0) {
                isFacingRight = true;
            }

            //here 
            
            Invoke("Roam_Next", nextRaomTime);
        }
        //protected virtual void Death() {}

        //protected virtual void Crash() {}
    }
}