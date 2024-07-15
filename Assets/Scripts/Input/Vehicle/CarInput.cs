using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarInput : VehicleInput
{
    private GameObject vehicle;

    public override void Init(GameObject vehicle)
    {
        this.vehicle = vehicle;
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
        //if ((movementSchemeComponents & MovementSchemeComponents.Movement) != 0)
        //{
        //    characterMove.SetMoveDirection(isSprint, GetMovementDirection());
        //}
    }

    protected abstract Vector2 GetMovementDirection();
}
