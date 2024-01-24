using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Click_Shoot : MonoBehaviour {
    public GameObject enemyBullet;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            GameObject TestBullet = Instantiate(enemyBullet, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
        }
    }
}