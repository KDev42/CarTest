using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] WheelCollider wheelCollider;
    [SerializeField] GameObject wheelMesh;
    [SerializeField] WheelStat wheelStat;

    public WheelCollider WheelCollider => wheelCollider;
    public GameObject WheelMesh => wheelMesh;

    public void InitWheel()
    {
        WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        wheelFrictionCurve.extremumSlip = wheelStat.SidewaysFriction.ExtremumSlip;
        wheelFrictionCurve.extremumValue = wheelStat.SidewaysFriction.ExtremumValue;
        wheelFrictionCurve.asymptoteSlip = wheelStat.SidewaysFriction.AsymptoteSlip;
        wheelFrictionCurve.asymptoteValue = wheelStat.SidewaysFriction.AsymptoteValue;
        wheelFrictionCurve.stiffness = wheelStat.SidewaysFriction.Stiffness;

        wheelCollider.sidewaysFriction = wheelFrictionCurve;
    }
}