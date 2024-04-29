using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EnemySystem {

    public class FlyEnemy : FlyEnemyClass {

        protected override void Start() {
            base.Start();
            Hp = 7;
            moveSpeed = 5;
            forwardRayCount = 4;
            forwardRayDistance = 3.5f;
            angleDetectValue = 20f;
            startI = -1;
        }

        protected override void Move() {
            if(isMoveAllow && !isDetectPlayer) {
                rb.velocity = randomDirection * moveSpeed;
            }
        }

        protected override void Chase() {
            // if(isDetectPlayer && isMoveAllow && Player != null) {
            //     float chaseRayDistance = 7.0f;
            //     Vector2 rayDirection = Player.gameObject.transform.position - this.transform.position;
            //     RaycastHit2D chaseHit = Physics2D.Raycast(transform.position, rayDirection, chaseRayDistance, playerLayer);
            //     Debug.DrawRay(transform.position, rayDirection * forwardRayDistance, Color.green, 0.3f);

            //     if(chaseHit.collider != null && chaseHit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            //         if(Player.transform.position.x <= this.transform.position.x) {
            //             rb.velocity = new Vector2(moveSpeed * -1.5f, rb.velocity.y);
            //         }
            //         else {
            //             rb.velocity = new Vector2(moveSpeed * 1.5f, rb.velocity.y);
            //         }
            //     }
            //     else {
            //         Player = null;
            //         isDetectPlayer = false;
            //         roamNext = 0;
            //         if(!IsInvoking("Roam_Next")) {
            //             Invoke("Roam_Next", 2f);
            //         }

            //         forwardHitResults.Clear();
            //     }
            // }
        }    

        protected override void Attack_Range() {
            // if(isDetectPlayer && !isAttacking) {
            //     float attackRayDistance = 1.5f;
            //     RaycastHit2D attackHit = Physics2D.Raycast(transform.position, new Vector2(facingIndex, 0), attackRayDistance, playerLayer);
            //     Debug.DrawRay(transform.position, new Vector2(facingIndex, 0) * attackRayDistance, Color.blue, 0.3f);

            //     if(attackHit.collider != null && attackHit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            //         StartCoroutine(Attack());
            //     }
            //     else {
            //         isMoveAllow = true;
            //     }
            // }
        }

        // IEnumerator Attack() {
        //     this.rb.velocity = Vector2.zero;
        //     isMoveAllow = false;
        //     isAttacking = true;
        //     Debug.Log("Attack Ready");
        //     yield return new WaitForSeconds(1.0f);

        //     Debug.Log("Attack");
        //     yield return new WaitForSeconds(1.0f);

        //     isAttacking = false;
        // }

        protected override void Roam_Next() {
            base.Roam_Next();
        }
    }
}