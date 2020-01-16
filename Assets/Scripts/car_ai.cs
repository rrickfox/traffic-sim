using System.Collections;
using System.Collections.Generic;
using UnityEngine;

float speed = 0f; 

public class car_ai : MonoBehaviour
 // Längeneinheiten pro Zeiteinheit


{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void fixedUpdate()
    {
        move();
    }


    void move()
    {
        
    }

    void accelerate(float acceleation)
    {
        speed = speed + acceleation;
    } 
}
