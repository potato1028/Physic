using System;
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
        //
        public float attackRayDistance;
        
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
                    Vector2 forwardRoamVec = new Vector2(transform.position.x, transform.position.y + (roamInterval * i));
                    RaycastHit2D forwardHit = Physics2D.Raycast(forwardRoamVec, Vector2.right * facingIndex, rayDistance, playerLayer);
                    Debug.DrawRay(forwardRoamVec, Vector2.right * facingIndex * rayDistance, Color.red, 0.3f); 

                    if(forwardHit.collider != null && !Roam_Obstacle(forwardRoamVec, Vector2.right * facingIndex, forwardHit.point.x)) {
                        Debug.Log("DetectPlayer");
                        Player = forwardHit.collider.gameObject;
                        CancelInvoke("Roam_Next");
                        isDetectPlayer = true;
                        break;
                    }
                    else {
                        Debug.Log("Obstacle");
                        isDetectPlayer = false;
                        continue;
                    }
                }

                Vector2 backRoamVec = new Vector2(transform.position.x, transform.position.y);
                RaycastHit2D backwardHit = Physics2D.Raycast(backRoamVec, Vector2.left * facingIndex, 1.0f, playerLayer);
                Debug.DrawRay(backRoamVec, Vector2.left * facingIndex * 1.0f, Color.blue, 0.3f); 

                if(backwardHit.collider != null && !Roam_Obstacle(backRoamVec, Vector2.left * facingIndex, backwardHit.point.x)) {
                    roamNext *= -1;
                    isFacingRight = !isFacingRight;
                }
            }
        }

        protected virtual void Roam_Next() {
            roamNext = UnityEngine.Random.Range(-1, 2);
            nextRaomTime = UnityEngine.Random.Range(5f, 8f);

            if(roamNext < 0) {
                isFacingRight = false;
            }
            else if(roamNext > 0) {
                isFacingRight = true;
            }

            //here 
            
            Invoke("Roam_Next", nextRaomTime);
        }

        protected virtual bool Roam_Obstacle(Vector2 startPosition, Vector2 rayDirection, float playerXPoint) {
            RaycastHit2D obstacleHit = Physics2D.Raycast(startPosition, rayDirection, rayDistance, obstacleLayer);
            Debug.DrawRay(roamVec, rayDirection * rayDistance, Color.red, 0.3f);

            if(obstacleHit.collider != null) {
                float distanceToPlayer = Mathf.Abs(playerXPoint - startPosition.x);
                float distanceToObstacle = Mathf.Abs(obstacleHit.point.x - startPosition.x);

                if(distanceToPlayer < distanceToObstacle) return false;
                else return true;
            }
            else return false;
                    
        }

        //protected virtual void Death() {}

        //protected virtual void Crash() {}

    }
}