using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cameramove : MonoBehaviour {
    [Header("Object")]
    public Transform target;

    public float smoothSpeed = 1.0f;

    void LateUpdate() {
        if(target != null) {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + 6f, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}