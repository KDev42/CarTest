using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, ISwitchableInput
{
    [SerializeField] CarPCInput carPCInput;

    private CarInput carInput;

    private void OnEnable()
    {
        InputEventsHolder.Instance.EnableInput += EnableInput;
        InputEventsHolder.Instance.DisableInput += DisableInput;
    }

    private void OnDisable()
    {
        InputEventsHolder.Instance.EnableInput -= EnableInput;
        InputEventsHolder.Instance.DisableInput -= DisableInput;
    }

    public void Init()
    {
        SwitchPlatform();
    }

    public void InitCarInput(GameObject car)
    {
        carInput.Init(car);
    }

    public void DisableInput()
    {
        carInput.DisableInput();
    }

    public void EnableInput()
    {
        carInput.EnableInput();
    }


    private void SwitchPlatform()
    {
        carInput = carPCInput;
    }
}
