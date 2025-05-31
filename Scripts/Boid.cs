using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    float bound = 0.1f;
    public bool isMain = false;

    void Start(){
        velocity = new Vector3(UnityEngine.Random.Range(-bound, bound),
                    UnityEngine.Random.Range(-bound, bound), 0f);
    }

    void Update(){

        float yaw = Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;
        
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -yaw);
        transform.rotation = targetRotation;
    }
}