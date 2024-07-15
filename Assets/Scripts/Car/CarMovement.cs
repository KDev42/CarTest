using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CarMovement : MonoBehaviour
{
    [SerializeField] CarMovementData carMovementData;

    [SerializeField] GameObject frontLeftMesh;
    [SerializeField] WheelCollider frontLeftCollider;
    [Space(10)]
    [SerializeField] GameObject frontRightMesh;
    [SerializeField] WheelCollider frontRightCollider;
    [Space(10)]
    [SerializeField] GameObject rearLeftMesh;
    [SerializeField] WheelCollider rearLeftCollider;
    [Space(10)]
    [SerializeField] GameObject rearRightMesh;
    [SerializeField] WheelCollider rearRightCollider; 
    [Space(10)]
    [SerializeField] Vector3 bodyMassCenter = new Vector3(0,1.5f,0); 

    private Rigidbody carRigidbody; // Stores the car's rigidbody.
    float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
    float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool touchControlsSetup = false;

    private float flWextremumSlip;
    private float rrWextremumSlip;
    private float frWextremumSlip;
    private float rlWextremumSlip;
    private WheelFrictionCurve flWheelFriction;
    private WheelFrictionCurve frWheelFriction;
    private WheelFrictionCurve rlWheelFriction;
    private WheelFrictionCurve rrWheelFriction;

    [HideInInspector]
    public float carSpeed; // Used to store the speed of the car.

    void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        InitWheele(frontLeftCollider, flWheelFriction, ref flWextremumSlip);
        InitWheele(frontRightCollider, frWheelFriction, ref frWextremumSlip);
        InitWheele(rearLeftCollider, rlWheelFriction, ref rlWextremumSlip);
        InitWheele(rearRightCollider, rrWheelFriction, ref rrWextremumSlip);
    }

    private void InitWheele(WheelCollider wheelCollider, WheelFrictionCurve wheelFriction, ref float wextremumSlip)
    {
        wheelFriction = new WheelFrictionCurve();
        wheelFriction.extremumSlip = wheelCollider.sidewaysFriction.extremumSlip;
        wextremumSlip = wheelCollider.sidewaysFriction.extremumSlip;
        wheelFriction.extremumValue = wheelCollider.sidewaysFriction.extremumValue;
        wheelFriction.asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip;
        wheelFriction.asymptoteValue = wheelCollider.sidewaysFriction.asymptoteValue;
        wheelFriction.stiffness = wheelCollider.sidewaysFriction.stiffness;
    }

    private void Turn(float horizantaleInput)
    {
        if (horizantaleInput != 0)
        {
            float sign = Math.Sign(horizantaleInput);

            steeringAxis = steeringAxis + sign * (Time.deltaTime * 10f * carMovementData.SteeringSpeed);

            if (steeringAxis < -1f)
            {
                steeringAxis = -1f;
            }
            if (steeringAxis > 1f)
            {
                steeringAxis = 1f;
            }

            var steeringAngle = steeringAxis * carMovementData.MaxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, carMovementData.SteeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, carMovementData.SteeringSpeed);
        }
    }

    //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.
    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * carMovementData.SteeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * carMovementData.SteeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * carMovementData.MaxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, carMovementData.SteeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, carMovementData.SteeringSpeed);
    }

    private void Move()
    {
        //If the forces aplied to the rigidbody in the 'x' asis are greater than
        //3f, it means that the car is losing traction, then the car will start emitting particle systems.
        //if (Mathf.Abs(localVelocityX) > 2.5f)
        //{
        //    isDrifting = true;
        //    DriftCarPS();
        //}
        //else
        //{
        //    isDrifting = false;
        //    DriftCarPS();
        //}

        // The following part sets the throttle power to 1 smoothly.
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }
        //If the car is going backwards, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
        //is safe to apply positive torque to go forward.
        if (localVelocityZ < -1f)
        {
            //Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < carMovementData.MaxSpeed)
            {
                //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (carMovementData.Acceleration * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (carMovementData.Acceleration * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (carMovementData.Acceleration * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (carMovementData.Acceleration * 50f) * throttleAxis;
            }
            else
            {
                // If the maxSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    //public void GoForward()
    //{
    //    //If the forces aplied to the rigidbody in the 'x' asis are greater than
    //    //3f, it means that the car is losing traction, then the car will start emitting particle systems.
    //    if (Mathf.Abs(localVelocityX) > 2.5f)
    //    {
    //        isDrifting = true;
    //        DriftCarPS();
    //    }
    //    else
    //    {
    //        isDrifting = false;
    //        DriftCarPS();
    //    }
    //    // The following part sets the throttle power to 1 smoothly.
    //    throttleAxis = throttleAxis + (Time.deltaTime * 3f);
    //    if (throttleAxis > 1f)
    //    {
    //        throttleAxis = 1f;
    //    }
    //    //If the car is going backwards, then apply brakes in order to avoid strange
    //    //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
    //    //is safe to apply positive torque to go forward.
    //    if (localVelocityZ < -1f)
    //    {
    //        Brakes();
    //    }
    //    else
    //    {
    //        if (Mathf.RoundToInt(carSpeed) < maxSpeed)
    //        {
    //            //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
    //            frontLeftCollider.brakeTorque = 0;
    //            frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            frontRightCollider.brakeTorque = 0;
    //            frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            rearLeftCollider.brakeTorque = 0;
    //            rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            rearRightCollider.brakeTorque = 0;
    //            rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //        }
    //        else
    //        {
    //            // If the maxSpeed has been reached, then stop applying torque to the wheels.
    //            // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
    //            // could be a bit higher than expected.
    //            frontLeftCollider.motorTorque = 0;
    //            frontRightCollider.motorTorque = 0;
    //            rearLeftCollider.motorTorque = 0;
    //            rearRightCollider.motorTorque = 0;
    //        }
    //    }
    //}

    //// This method apply negative torque to the wheels in order to go backwards.
    //public void GoReverse()
    //{
    //    //If the forces aplied to the rigidbody in the 'x' asis are greater than
    //    //3f, it means that the car is losing traction, then the car will start emitting particle systems.
    //    if (Mathf.Abs(localVelocityX) > 2.5f)
    //    {
    //        isDrifting = true;
    //        DriftCarPS();
    //    }
    //    else
    //    {
    //        isDrifting = false;
    //        DriftCarPS();
    //    }
    //    // The following part sets the throttle power to -1 smoothly.
    //    throttleAxis = throttleAxis - (Time.deltaTime * 3f);
    //    if (throttleAxis < -1f)
    //    {
    //        throttleAxis = -1f;
    //    }
    //    //If the car is still going forward, then apply brakes in order to avoid strange
    //    //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
    //    //is safe to apply negative torque to go reverse.
    //    if (localVelocityZ > 1f)
    //    {
    //        Brakes();
    //    }
    //    else
    //    {
    //        if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
    //        {
    //            //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
    //            frontLeftCollider.brakeTorque = 0;
    //            frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            frontRightCollider.brakeTorque = 0;
    //            frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            rearLeftCollider.brakeTorque = 0;
    //            rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //            rearRightCollider.brakeTorque = 0;
    //            rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
    //        }
    //        else
    //        {
    //            //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
    //            // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
    //            // could be a bit higher than expected.
    //            frontLeftCollider.motorTorque = 0;
    //            frontRightCollider.motorTorque = 0;
    //            rearLeftCollider.motorTorque = 0;
    //            rearRightCollider.motorTorque = 0;
    //        }
    //    }
    //}
}
