using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] CarMovement carMovement;

    private void Awake()
    {
        inputManager.Init();
        inputManager.InitCarInput(carMovement.gameObject);
        InputEventsHolder.Instance.EnableInput();
    }
}
