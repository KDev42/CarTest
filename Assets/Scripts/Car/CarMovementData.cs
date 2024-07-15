using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementData : ScriptableObject
{
    [Range(20, 450)]
    [SerializeField] int maxSpeed = 90; 
    [Range(10, 120)]
    [SerializeField] int maxReverseSpeed = 45;
    [Range(1, 10)]
    [SerializeField] int acceleration = 2;
    [Space(10)]
    [Range(10, 45)]
    [SerializeField] int maxSteeringAngle = 27;
    [Range(0.1f, 1f)]
    [SerializeField] float steeringSpeed = 0.5f;
    [Space(10)]
    [Range(100, 600)]
    [SerializeField] int brakeForce = 350;
    [Range(1, 10)]
    [SerializeField] int decelerationMultiplier = 2;
    [Range(1, 10)]
    [SerializeField] int handbrakeDriftMultiplier = 5;

    public int MaxSpeed => maxSpeed;
    public int MaxReverseSpeed => maxReverseSpeed;
    public int Acceleration => acceleration;
    public int MaxSteeringAngle => maxSteeringAngle;
    public float SteeringSpeed => steeringSpeed;
    public int BrakeForce => brakeForce;
    public int DecelerationMultiplier => decelerationMultiplier;
    public int HandbrakeDriftMultiplier => handbrakeDriftMultiplier;
}
