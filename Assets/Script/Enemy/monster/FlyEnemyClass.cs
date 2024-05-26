using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnemySystem {

    public abstract class FlyEnemyClass : MonoBehaviour {
        [Header("Enemy_Status")]
        public int startI;
        public int Hp;
        public float moveSpeed;
        public int facingIndex;
        public Vector2 randomDirection;
        /////
        public int forwardRayCount;
        public float rayDistance;
        public Vector2 rayDirection;
        public float angleDetect;
        public float angleDetectValue;
        //
        public float attackRayDistance;

        [Header("Enemy_Component")]
        public Rigidbody2D rb;
        public BoxCollider2D box2D;

        [Header("Enemy_Layer")]
        public LayerMask obstacleLayer;
        public LayerMask playerLayer;
        public LayerMask groundLayer;

        [Header("Enemy_Condition")]
        public bool isMoveAllow;
        public bool isDetectPlayer;
        public bool isAttacking;
        ///
        public float nextRaomTime;

        [Header("Other_Object")]
        public GameObject Player;

        public void Awake() {
            rb = GetComponent<Rigidbody2D>();
            box2D = GetComponent<BoxCollider2D>();
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
            if(randomDirection.x <= 0) {
                facingIndex = -1;
            }
            else {
                facingIndex = 1;
            }

            if(!isDetectPlayer) {
                for(int i = startI; i < forwardRayCount; i++) {
                    angleDetect = i * angleDetectValue;
                    Vector2 direction = new Vector2(Mathf.Cos(angleDetect * Mathf.Deg2Rad) * facingIndex, Mathf.Sin(angleDetect * Mathf.Deg2Rad));
                    RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, direction, rayDistance, playerLayer);
                    Debug.DrawRay(transform.position, direction * rayDistance, Color.red, 0.3f);

                    if(forwardHit.collider != null) {
                        if(forwardHit.collider.gameObject.CompareTag("Player") && !Roam_Obstacle(this.transform.position, direction, forwardHit.point.x)) {
                            Debug.Log("DetectPlayer");
                            Player = forwardHit.collider.gameObject;
                            CancelInvoke("Roam_Next");
                            isDetectPlayer = true;
                            break;
                        }
                        else {
                            isDetectPlayer = false;
                            continue;
                        }
                    }
                }

                Vector2 backRoamVec = new Vector2(transform.position.x, transform.position.y);
                RaycastHit2D backwardHit = Physics2D.Raycast(backRoamVec, Vector2.left * facingIndex, 1.0f, playerLayer);
                Debug.DrawRay(backRoamVec, Vector2.left * facingIndex * 1.0f, Color.blue, 0.3f); 

                if(backwardHit.collider != null && !Roam_Obstacle(backRoamVec, Vector2.left * facingIndex, backwardHit.point.x)) {
                    rb.velocity = Vector2.zero;
                    randomDirection.x *= -1;
                }
            }

            return;
        }

        protected virtual void Roam_Next() {
            randomDirection = Random.insideUnitCircle.normalized;
            nextRaomTime = Random.Range(5f, 8f);
            
            Invoke("Roam_Next", nextRaomTime);
        }

        protected virtual bool Roam_Obstacle(Vector2 startPosition, Vector2 rayDirection, float playerXPoint) {
            RaycastHit2D obstacleHit = Physics2D.Raycast(startPosition, rayDirection, rayDistance, obstacleLayer);
            Debug.DrawRay(startPosition, rayDirection * rayDistance, Color.red, 0.3f);

            if(obstacleHit.collider != null) {
                float distanceToPlayer = Mathf.Abs(playerXPoint - startPosition.x);
                float distanceToObstacle = Mathf.Abs(obstacleHit.point.x - startPosition.x);

                if(distanceToPlayer < distanceToObstacle) return false;
                else return true;
            }

            return false;
            
        }

        protected void OnCollisionEnter2D(Collision2D other) {
            if((obstacleLayer.value & (1 << other.gameObject.layer)) != 0 && !isDetectPlayer) {
                CancelInvoke("Roam_Next");
                Invoke("Roam_Next", nextRaomTime);
                Vector2 collisionPoint = transform.InverseTransformPoint(other.contacts[0].point);
                Debug.Log(collisionPoint);

                if(randomDirection.x < 0) { //왼쪽으로 가던 중
                    if(collisionPoint.y < 0) { // 왼쪽 아래 대각선으로
                        if(collisionPoint.x <= -0.5f) {
                            randomDirection.x *= -1f;
                        }
                        else if(collisionPoint.y < -0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else if(collisionPoint.y > 0) { //왼쪽 위 대각선으로
                        if(collisionPoint.x <= -0.5f) {
                            randomDirection.x *= -1f;
                        }
                        else if(collisionPoint.y > 0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else { // 왼쪽으로
                        randomDirection.x *= -1f;
                    }
                }
                else if(randomDirection.x > 0) { //오른쪽으로 가던 중
                    if(collisionPoint.y < 0) { //오른쪽 아래 대각선으로
                        if(collisionPoint.x >= 0.5f) {
                            randomDirection.x *= -1f;
                        }
                        else if(collisionPoint.y < -0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else if(collisionPoint.y > 0) { //오른쪽 위 대각선으로
                        if(collisionPoint.x >= 0.5f) {
                            randomDirection.x *= -1f;
                        }
                        else if(collisionPoint.y >= 0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else { // 오른쪽으로
                        randomDirection.x *= -1f;
                    }
                }
                else { // 위 아래로 움직일 때
                    randomDirection.y *= -1f;
                }


            }
        }
        //protected virtual void Death() {}

        //protected virtual void Crash() {}
    }
}