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
        ExtremumSlip � ��� � ���� ���������� ������. ��� ���� ��� ��������, ��� ������ ���� ������ ��������� �� �����������, ��� �������� �� ��(�.�. � ������� �������� ��� �������������).
        ExtremumValue � ��� ����, ������� �������������� ����������. ��� ���� ��� ��������, ��� ������ ����� ��������� ���, ��� �� ������� ������ �� ��.

        ����� ����������� ����������� �����, ������ ��������� � �� �� ��������� �������������
        AsymptoteSlip � ��� ���������� ������, �� ����� ���.
        AsymptoteValue � ��� ���� ������� �������������� ���������� �� ����� ���
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