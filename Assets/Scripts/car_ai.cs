using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_ai : MonoBehaviour
{
    // Längeneinheiten pro Zeiteinheit
    float speed = 0f; 
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
