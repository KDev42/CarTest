using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CarMovement : MonoBehaviour
{
    [SerializeField] CarData carData;
    [SerializeField] CarEffects carEffects;

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

    public Vector3 LocalVelocity { get; private set; }
    public float CarSpeed { get; private set; }
    public bool IsDrifting { get; private set; }
    public bool IsTractionLocked { get; private set; }

    private float SteeringAxis {
        get => steeringAxis;
        set {
            if (value > 1) steeringAxis = 1;
            else if (value < -1) steeringAxis = -1;
            else
                steeringAxis = value;
        }
    }
    private float ThrottleAxis
    {
        get => throttleAxis;
        set
        {
            if (value > 1) throttleAxis = 1;
            else if (value < -1) throttleAxis = -1;
            else
                throttleAxis = value;
        }
    }

    private float steeringAxis;
    private float throttleAxis;
    private float driftingAxis;
    private bool deceleratingCar;

    private Rigidbody carRigidbody;

    private float flWextremumSlip;
    private float rrWextremumSlip;
    private float frWextremumSlip;
    private float rlWextremumSlip;
    private WheelFrictionCurve flWheelFriction;
    private WheelFrictionCurve frWheelFriction;
    private WheelFrictionCurve rlWheelFriction;
    private WheelFrictionCurve rrWheelFriction;

    void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        InitWheele(frontLeftCollider, flWheelFriction, ref flWextremumSlip);
        InitWheele(frontRightCollider, frWheelFriction, ref frWextremumSlip);
        InitWheele(rearLeftCollider, rlWheelFriction, ref rlWextremumSlip);
        InitWheele(rearRightCollider, rrWheelFriction, ref rrWextremumSlip);
    }

    private Vector2 moveDirection;

    private void Update()
    {
        CarSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

        LocalVelocity = new Vector3(transform.InverseTransformDirection(carRigidbody.velocity).x, LocalVelocity.y, transform.InverseTransformDirection(carRigidbody.velocity).z); 

        if (moveDirection.y !=0)
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            Move(moveDirection.y);
        }

        if (moveDirection.x != 0)
        {
            Turn(moveDirection.x);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            Handbrake();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RecoverTraction();
        }
        if (moveDirection.y == 0)
        {
            ThrottleOff();
        }
        if (moveDirection.y == 0  && !deceleratingCar)// && !Input.GetKey(KeyCode.Space))
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }
        if (moveDirection.x == 0 && SteeringAxis != 0f)
        {
            ResetSteeringAngle();
        }

        AnimateWheelMeshes();
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection;
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
        float sign = Math.Sign(horizantaleInput);

        SteeringAxis = SteeringAxis + sign * (Time.deltaTime * 10f * carData.SteeringSpeed);

        Debug.Log("SteeringAxis " + SteeringAxis);

        float steeringAngle = SteeringAxis * carData.MaxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, carData.SteeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, carData.SteeringSpeed);
    }

    private void ResetSteeringAngle()
    {
        if (SteeringAxis < 0f)
        {
            SteeringAxis = SteeringAxis + (Time.deltaTime * 10f * carData.SteeringSpeed);
        }
        else if (SteeringAxis > 0f)
        {
            SteeringAxis = SteeringAxis - (Time.deltaTime * 10f * carData.SteeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            SteeringAxis = 0f;
        }
        var steeringAngle = SteeringAxis * carData.MaxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, carData.SteeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, carData.SteeringSpeed);
    }

    private void Move(float verticleInput)
    {
        if (Mathf.Abs(LocalVelocity.x) > 2.5f)
        {
            IsDrifting = true;
            carEffects.DriftCarPS();
        }
        else
        {
            IsDrifting = false;
            carEffects.DriftCarPS();
        }

        float sign = Math.Sign(verticleInput);
        float limitedSpeed = sign > 0 ? carData.MaxSpeed : carData.MaxReverseSpeed;

        ThrottleAxis = ThrottleAxis + sign*(Time.deltaTime * 3f);
        if (ThrottleAxis > 1f)
        {
            ThrottleAxis = 1f;
        }
        if (ThrottleAxis < -1f)
        {
            ThrottleAxis = -1f;
        }

        if ((sign > 0 && LocalVelocity.z < -1f) || (sign < 0 && LocalVelocity.z > 1f))
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(CarSpeed) < limitedSpeed)
            {
                Acceleration();
            }
            else
            {
                ThrottleOff();
            }
        }
    }

    private void Acceleration()
    {
        float force = carData.Acceleration * carData.Mass;
        float motorTorque = frontLeftCollider.radius * force;
        motorTorque /= 4f;

        Action<WheelCollider> addMotorTorque = (wheelCollider) => {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = (motorTorque) * ThrottleAxis;
        };

        addMotorTorque(frontLeftCollider);
        addMotorTorque(frontRightCollider);
        addMotorTorque(rearLeftCollider);
        addMotorTorque(rearRightCollider);
    }

    private void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    private void Brakes()
    {
        frontLeftCollider.brakeTorque = carData.BrakeForce;
        frontRightCollider.brakeTorque = carData.BrakeForce;
        rearLeftCollider.brakeTorque = carData.BrakeForce;
        rearRightCollider.brakeTorque = carData.BrakeForce;
    }

    private void Handbrake()
    {
        CancelInvoke("RecoverTraction");
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * flWextremumSlip * carData.HandbrakeDriftMultiplier;

        if (secureStartingPoint < flWextremumSlip)
        {
            driftingAxis = flWextremumSlip / (flWextremumSlip * carData.HandbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }
        if (Mathf.Abs(LocalVelocity.x) > 2.5f)
        {
            IsDrifting = true;
        }
        else
        {
            IsDrifting = false;
        }
        if (driftingAxis < 1f)
        {
            SetExtimumSlip(flWheelFriction, frontLeftCollider, flWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(frWheelFriction, frontRightCollider, frWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(rlWheelFriction, rearLeftCollider, rlWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(rrWheelFriction, rearRightCollider, rrWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
        }

        IsTractionLocked = true;
        carEffects.DriftCarPS();

    }

    private void RecoverTraction()
    {
        IsTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }

        if (flWheelFriction.extremumSlip > flWextremumSlip)
        {
            SetExtimumSlip(flWheelFriction, frontLeftCollider, flWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(frWheelFriction, frontRightCollider, frWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(rlWheelFriction, rearLeftCollider, rlWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);
            SetExtimumSlip(rrWheelFriction, rearRightCollider, rrWextremumSlip * carData.HandbrakeDriftMultiplier * driftingAxis);

            Invoke("RecoverTraction", Time.deltaTime);

        }
        else if (flWheelFriction.extremumSlip < flWextremumSlip)
        {
            SetExtimumSlip(flWheelFriction, frontLeftCollider, flWextremumSlip);
            SetExtimumSlip(frWheelFriction, frontRightCollider, frWextremumSlip);
            SetExtimumSlip(rlWheelFriction, rearLeftCollider, rlWextremumSlip);
            SetExtimumSlip(rrWheelFriction, rearRightCollider, rrWextremumSlip);

            driftingAxis = 0f;
        }
    }

    private void DecelerateCar()
    {
        if (Mathf.Abs(LocalVelocity.x) > 2.5f)
        {
            IsDrifting = true;
            carEffects.DriftCarPS();
        }
        else
        {
            IsDrifting = false;
            carEffects.DriftCarPS();
        }

        if (ThrottleAxis != 0f)
        {
            if (ThrottleAxis > 0f)
            {
                ThrottleAxis = ThrottleAxis - (Time.deltaTime * 10f);
            }
            else if (ThrottleAxis < 0f)
            {
                ThrottleAxis = ThrottleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(ThrottleAxis) < 0.15f)
            {
                ThrottleAxis = 0f;
            }
        }
        carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * carData.DecelerationMultiplier)));

        ThrottleOff();

        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    private void AnimateWheelMeshes()
    {
        Action<GameObject, WheelCollider> rotateWheel = (meshGO, wheelCollider) =>
        {
            Quaternion wheelRotation;
            Vector3 wheelPosition;
            wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
            meshGO.transform.position = wheelPosition;
            meshGO.transform.rotation = wheelRotation;
        };

        try
        {
            rotateWheel(frontLeftMesh, frontLeftCollider);
            rotateWheel(frontRightMesh, frontRightCollider);
            rotateWheel(rearLeftMesh, rearLeftCollider);
            rotateWheel(rearRightMesh, rearRightCollider);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private Action<WheelFrictionCurve, WheelCollider, float> SetExtimumSlip = (wheelFrictionCurve, wheelCollider, wextremumSlip) =>
    {
        wheelFrictionCurve.extremumSlip = wextremumSlip;
        wheelCollider.sidewaysFriction = wheelFrictionCurve;
    };
}
