using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarInput : VehicleInput
{
    private GameObject vehicle;
    private CarMovement carMovement;

    public override void Init(GameObject vehicle)
    {
        this.vehicle = vehicle;
        vehicle.TryGetComponent(out carMovement);
    }

    public override void EnableInput()
    {

    }

    public override void DisableInput()
    {

    }

    protected override void Exit()
    {

    }
    private void Update()
    {
        carMovement.SetMoveDirection(GetMovementDirection());
    }

    protected abstract Vector2 GetMovementDirection();
}
