using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EnemySystem {

    public class BasicEnemy : BasicEnemyClass {

        public BasicEnemy() {
            Hp = 10;
            moveSpeed = 4;
            forwardRayCount = 14;
            forwardRayDistance = 3.5f;
        }

        protected override void Move() {
            if(isMoveAllow && !isDetectPlayer) {
                rb.velocity = new Vector2(roamNext * moveSpeed, rb.velocity.y);
            }
            roamVec = new Vector2(rb.position.x + roamNext * 0.4f, rb.position.y);

            roamHit = Physics2D.Raycast(roamVec, Vector3.down, 1, groundLayer);
            obstacleHit = Physics2D.Raycast(rb.position, Vector2.right * roamNext, 0.52f, groundLayer);


            if(roamHit.collider == null || obstacleHit.collider != null) {
                roamNext *= -1;
                CancelInvoke();
                Debug.Log("Roam_Next");
            }
        }

        protected override void Chase() {

        }    

        protected override void Attack() {
            
        }
    }
}