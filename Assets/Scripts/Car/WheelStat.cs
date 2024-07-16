using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WheelData")]
public class WheelStat : ScriptableObject
{
    [SerializeField] WheelFriction sidewaysFriction;

    public WheelFriction SidewaysFriction => sidewaysFriction;
}

[Serializable] 
public class WheelFriction
{
    /*
        ExtremumSlip Ч это и есть скольжение колеса. „ем выше это значение, тем больше шина сможет скользить по поверхности, без срывани€ на юз(т.е. к резкому снижению сил сопротивлени€).
        ExtremumValue Ч это силы, которые сопротивл€ютс€ скольжению. „ем выше это значение, тем больше нужно приложить сил, что бы сорвать колесо на юз.

        ѕосле прохождени€ критической точки, колеса срываютс€ в юз со снижением сопротивлени€
        AsymptoteSlip Ч это скольжение колеса, во врем€ юза.
        AsymptoteValue Ч это силы которые сопротивл€ютс€ скольжению во врем€ юза
     */

    [Range(0.1f, 10)]
    public float ExtremumSlip = 0.2f;
    [Range(0.1f, 10)]
    public float ExtremumValue = 1;
    [Range(0.1f, 10)]
    public float AsymptoteSlip = 0.5f;
    [Range(0.1f, 10)]
    public float AsymptoteValue = 0.75f;
    [Range(0, 1)]
    public float Stiffness = 1;
}