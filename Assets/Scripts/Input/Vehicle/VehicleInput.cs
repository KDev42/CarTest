using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleInput : MonoBehaviour, ISwitchableInput
{
    public abstract void Init(GameObject vehicle);

    public abstract void EnableInput();

    public abstract void DisableInput();

    protected abstract void Exit();
}
