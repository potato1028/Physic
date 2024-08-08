using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EnemySystem {

    public class FlyEnemy : FlyEnemyClass {

        [Header("Basic_Enemy_Status")]
        public float attackSpeed = 8f;

        protected override void Start() {
            base.Start();
            Hp = 7;
            moveSpeed = 5;
            forwardRayCount = 4;
            rayDistance = 5.0f;
            angleDetectValue = 20f;
            startI = -1;
            attackRayDistance = 2.5f;
        }

        protected override void Move() {
            if(isMoveAllow && !isDetectPlayer) {
                rb.velocity = randomDirection * moveSpeed;
            }
        }

        protected override void Chase() {
            if(isDetectPlayer && isMoveAllow && Player != null) {
                float chaseRayDistance = 7.0f;
                rayDirection = Player.gameObject.transform.position - this.transform.position;
                RaycastHit2D chaseHit = Physics2D.Raycast(transform.position, rayDirection, chaseRayDistance, playerLayer);
                Debug.DrawRay(transform.position, rayDirection * rayDistance, Color.green, 0.3f);

                if(chaseHit.collider != null && chaseHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !Roam_Obstacle(this.transform.position, rayDirection, Player.transform.position.x)) {
                    rb.velocity = rayDirection * moveSpeed / 10f;
                }
                else {
                    rb.velocity = Vector2.zero;
                    Player = null;
                    isDetectPlayer = false;
                    if(!IsInvoking("Roam_Next")) {
                        Invoke("Roam_Next", 2f);
                    }
                }
            }
        }    

        protected override void Attack_Range() {
            if(isDetectPlayer && !isAttacking) {
                RaycastHit2D attackHit = Physics2D.Raycast(transform.position, rayDirection, attackRayDistance, playerLayer);
                Debug.DrawRay(transform.position, rayDirection * attackRayDistance, Color.blue, 0.3f);

                if(attackHit.collider != null && attackHit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    StartCoroutine(Attack(rayDirection));
                }
                else {
                    isMoveAllow = true;
                }
            }
        }

        IEnumerator Attack(Vector2 playerPosition) {
            rb.velocity = Vector2.zero;
            isMoveAllow = false;
            isAttacking = true;
            Debug.Log("Attack Ready");
            yield return new WaitForSeconds(1.0f);

            rb.velocity = rayDirection * attackSpeed;

            yield return new WaitForSeconds(2.0f);

            isAttacking = false;
            rb.velocity = Vector2.zero;

            yield return new WaitForSeconds(3f);
        }

        protected override void OnCollisionEnter2D(Collision2D other) {
            base.OnCollisionEnter2D(other);
            if(isAttacking && other.gameObject.layer == obstacleLayer) {
                Debug.Log("Stop");
                StopCoroutine(Attack(rayDirection));
                rb.velocity = Vector2.zero;
                isAttacking = false;
            }
        }
    }
}