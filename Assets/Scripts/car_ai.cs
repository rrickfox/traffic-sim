using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTypes;


public class car_ai
{
    
    public float speed = 0f; // Längeneinheiten pro Zeiteinheit
    public car Car;
    

    void fixedUpdate()
    {
        EarlyfixedUpdate();
        LatefixedUpdate();
    }
    private void EarlyfixedUpdate()
    {
        //get Distance
    }

    void LatefixedUpdate()
    {
        move();
    }

    void move()
    {
        car.positionOnRoad += speed;
    }

    void accelerate(float acceleation)
    {
        speed += acceleation;
    } 
}
