using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPCInput : CarInput
{
    private PlayerController playerController;

    public override void Init(GameObject vehicle)
    {
        base.Init(vehicle);
        playerController = new PlayerController();
    }

    public override void EnableInput()
    {
        base.EnableInput();
        playerController.Enable();
    }

    public override void DisableInput()
    {
        base.DisableInput();

        playerController.Disable();
    }

    protected override Vector2 GetMovementDirection()
    {
        return playerController.Vehicle.Move.ReadValue<Vector2>();
    }
}
