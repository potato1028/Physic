using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnemySystem {

    public abstract class BasicEnemyClass : MonoBehaviour {
        [Header("Enemy_Status")]
        public int Hp;
        public int attackPower;
        public float attackDelay;
        public float moveSpeed;
        public float chaseSpeed;
        public int facingIndex;
        public Vector2 roamVec;
        /////
        public int forwardRayCount;
        public float forwardRayDistance;
        public float angleDetect;
        public float angleDetectValue;
        public List<RaycastHit2D> forwardHitResults = new List<RaycastHit2D>();

        [Header("Enemy_Component")]
        public Rigidbody2D rb;

        [Header("Enemy_Layer")]
        public LayerMask obstacleLayer;
        public LayerMask playerLayer;
        public LayerMask groundLayer;

        [Header("Enemy_Condition")]
        public bool isMoveAllow;
        public bool isMoving;
        public bool isDetectPlayer;
        public bool isFacingRight;
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

        public void Start() {
            Roam_Next();
        }

        public void Update() {
            Move();
            PlayerDetect();
        }

        protected abstract void Move();

        protected abstract void Chase();

        protected abstract void Attack();

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

            for(int i = 0; i < forwardRayCount; i++) {
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
                    isDetectPlayer = true;
                    break;
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

            Invoke("Roam_Next", nextRaomTime);
        }

        protected virtual void Death() {}

        protected virtual void Crash() {}
    }

}