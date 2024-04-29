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
        public float forwardRayDistance;
        public float angleDetect;
        public float angleDetectValue;
        public List<RaycastHit2D> forwardHitResults = new List<RaycastHit2D>();
        

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
        public bool isFacingRight;
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
            //PlayerDetect();
            Attack_Range();
        }

        protected abstract void Move();

        protected abstract void Chase();

        protected abstract void Attack_Range();

        protected virtual void PlayerDetect() {

            if(!isDetectPlayer) {
                for(int i = startI; i < forwardRayCount; i++) {
                    angleDetect = i * angleDetectValue;
                    Vector2 direction = new Vector2(Mathf.Cos(angleDetect * Mathf.Deg2Rad) * facingIndex, Mathf.Sin(angleDetect * Mathf.Deg2Rad));
                    RaycastHit2D[] forwardHit = Physics2D.RaycastAll(transform.position, direction, forwardRayDistance, playerLayer);
                
                    Debug.DrawRay(transform.position, direction * forwardRayDistance, Color.red, 0.3f);
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
            randomDirection = Random.insideUnitCircle.normalized;
            nextRaomTime = Random.Range(5f, 8f);
            //here 
            
            Invoke("Roam_Next", nextRaomTime);
        }

        protected void OnCollisionEnter2D(Collision2D other) {
            if((obstacleLayer.value & (1 << other.gameObject.layer)) != 0) {
                Vector2 collisionPoint = transform.InverseTransformPoint(other.contacts[0].point);
                Debug.Log(collisionPoint);

                if(randomDirection.x < 0) { //왼쪽으로 가던 중
                    if(collisionPoint.y < 0) { // 왼쪽 아래 대각선으로
                        if(collisionPoint.x < -0.5f) {
                            randomDirection.x *= -1f;
                        }
                        if(collisionPoint.y < -0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else if(collisionPoint.y > 0) { //왼쪽 위 대각선으로
                        if(collisionPoint.x < -0.5f) {
                            randomDirection.x *= -1f;
                        }
                        if(collisionPoint.y > 0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else { // 왼쪽으로
                        randomDirection.x *= -1f;
                    }
                }
                else if(randomDirection.x > 0) { //오른쪽으로 가던 중
                    if(collisionPoint.y < 0) { //오른쪽 아래 대각선으로
                        if(collisionPoint.x > 0.5f) {
                            randomDirection.x *= -1f;
                        }
                        if(collisionPoint.y < -0.5f) {
                            randomDirection.y *= -1f;
                        }
                    }
                    else if(collisionPoint.y > 0) { //오른쪽 위 대각선으로
                        if(collisionPoint.x > 0.5f) {
                            randomDirection.x *= -1f;
                        }
                        if(collisionPoint.y > 0.5f) {
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