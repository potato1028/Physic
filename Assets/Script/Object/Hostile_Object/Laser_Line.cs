using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Line :  MonoBehaviour {
    [Header("Status")]
    public float destroyTime = 5.0f;
    public float targetingTime = 3.0f;
    public float shootTime = 0.3f;
    public float reloadTime = 0.5f;
    public float laserPower = 1.0f;
    //
    public bool isTargetAllow = false;
    public bool isShootAllow = false;
    //
    public Color targetColor = Color.green;
    public Color shootColor = Color.red;
    //
    public Vector2[] laserPoint = new Vector2[2];
    public Vector2 laserPush;

    [Header("GameObject")]
    public GameObject Player;

    [Header("Component")]
    public LineRenderer lineR;
    public EdgeCollider2D edge2D;

    [Header("Other_Component")]
    public TestPlayerControl testPlayerControl;

    public void Start() {
        Player = GameObject.FindWithTag("Player");
        testPlayerControl = Player.GetComponent<TestPlayerControl>();
        edge2D.enabled = false;

        lineR.SetPosition(0, new Vector2(0, 0));
        lineR.SetPosition(1, new Vector2(0, 0));

        laserPoint[0] =  new Vector2(0, 0);
        laserPoint[1] = new Vector2(0, 0);
        edge2D.points = laserPoint;

        lineR.startColor = targetColor; 
        lineR.endColor = targetColor;

        StartCoroutine(Laser_Position());
    }

    public void Update() {
        Laser_Size();
    }

    IEnumerator Laser_Position() {
        lineR.startColor = targetColor; 
        lineR.endColor = targetColor;

        isTargetAllow = true;

        yield return new WaitForSeconds(targetingTime);
        
        isShootAllow = true;

        yield return new WaitForSeconds(0.2f);

        edge2D.enabled = true;
        lineR.startColor = shootColor;
        lineR.endColor = shootColor;

        yield return new WaitForSeconds(shootTime);

        edge2D.enabled = false;
        isTargetAllow = false;
        isShootAllow = false;

        yield return new WaitForSeconds(reloadTime);

        StartCoroutine(Laser_Position());
    }

    public void Laser_Size() {
        if(!isShootAllow) {
            if(isTargetAllow) {
                laserPoint[1] = new Vector2(-10f * (this.transform.position.x - Player.transform.position.x), -10f * (this.transform.position.y - Player.transform.position.y));
                lineR.SetPosition(1, laserPoint[1]);
            }
            else {
                laserPoint[1] = new Vector2(0, 0);
                lineR.SetPosition(1, laserPoint[1]);
            }
            edge2D.points = laserPoint;
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(LayerMask.LayerToName(other.gameObject.layer) == "Player") {
            if(!testPlayerControl.isHitting && testPlayerControl.isHitAllow && !testPlayerControl.isFrictioning) {
                testPlayerControl.Hp -= laserPower;
                testPlayerControl.rb.velocity = Vector2.zero;

                if(other.transform.position.x >= this.transform.position.x) {
                    testPlayerControl.rb.velocity = laserPush;
                }
                else {
                    testPlayerControl.rb.velocity = new Vector2(laserPush.x * -1, laserPush.y);
                }
            }
        }
    }
}