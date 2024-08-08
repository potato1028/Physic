using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EnemySystem {

    public class BasicEnemy : BasicEnemyClass {
        [Header("Basic_Enemy_Status")]
        public float attackSpeed = 8f;

        protected override void Start() {
            base.Start();
            Hp = 5;
            moveSpeed = 3;
            rayDistance = 3.5f;
            roamInterval = transform.localScale.y / 4f;
            attackRayDistance = 2f;
        }

        protected override void Move() {
            if(isMoveAllow && !isDetectPlayer) {
                rb.velocity = new Vector2(roamNext * moveSpeed, rb.velocity.y);
            }
            roamVec = new Vector2(rb.position.x + roamNext * 0.4f, rb.position.y);

            roamHit = Physics2D.Raycast(roamVec, Vector3.down, 1.5f, groundLayer);
            Debug.DrawRay(roamVec, Vector3.down * 1.5f, Color.yellow);
            obstacleHit = Physics2D.Raycast(rb.position, Vector2.right * roamNext, 0.52f, groundLayer);


            if(roamHit.collider == null || obstacleHit.collider != null) {
                roamNext *= -1;
                facingIndex = -1;
            }
        }

        protected override void Chase() {
            if(isDetectPlayer && isMoveAllow && Player != null) {
                float chaseRayDistance = 7.0f;
                Vector2 rayDirection = Player.gameObject.transform.position - this.transform.position;
                RaycastHit2D chaseHit = Physics2D.Raycast(transform.position, rayDirection, chaseRayDistance, playerLayer);
                Debug.DrawRay(transform.position, rayDirection * rayDistance, Color.green, 0.3f);

                if(chaseHit.collider != null && chaseHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !Roam_Obstacle(this.transform.position, rayDirection, Player.transform.position.x)) {
                    if(Player.transform.position.x <= this.transform.position.x) {
                        StartCoroutine(delayChase(-1));
                        facingIndex = -1;
                    }
                    else {
                        StartCoroutine(delayChase(1));
                        facingIndex = 1;
                    }
                    rb.velocity = new Vector2(moveSpeed * facingIndex * 1.5f, rb.velocity.y);
                    
                }
                else {
                    Player = null;
                    isDetectPlayer = false;
                    roamNext = 0;
                    if(!IsInvoking("Roam_Next")) {
                        Invoke("Roam_Next", 2f);
                    }
                }
            }
        }    

        protected override void Attack_Range() {
            if(isDetectPlayer && !isAttacking) {
                RaycastHit2D attackHit = Physics2D.Raycast(transform.position, new Vector2(facingIndex, 0), attackRayDistance, playerLayer);
                Debug.DrawRay(transform.position, new Vector2(facingIndex, 0) * attackRayDistance, Color.blue, 0.3f);

                if(attackHit.collider != null && attackHit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    StartCoroutine(Attack());
                }
                else {
                    isMoveAllow = true;
                }
            }
        }

        void OnCollisionEnter2D(Collision2D other) {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
                rb.velocity = Vector2.zero;
                rb.velocity = new Vector2(5f * facingIndex * -1f, 2f); //delay
            }
        }

        IEnumerator Attack() {
            rb.velocity = Vector2.zero;
            isMoveAllow = false;
            isAttacking = true;
            Debug.Log("Attack Ready");
            yield return new WaitForSeconds(1.0f);

            rb.velocity = new Vector2(facingIndex * attackSpeed, rb.velocity.y);

            yield return new WaitForSeconds(1.0f);

            isAttacking = false;
            rb.velocity = Vector2.zero;

            yield return new WaitForSeconds(3f);
        }

        IEnumerator delayChase(int currentIndex) {
            if(facingIndex == currentIndex) {
                yield break;
            }

            float currentMoveSpeed = moveSpeed;
            moveSpeed = 0f;

            yield return new WaitForSeconds(0.5f);

            moveSpeed = currentMoveSpeed;
        }
    }
}