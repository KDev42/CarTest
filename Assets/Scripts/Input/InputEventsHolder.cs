using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InputEventsHolder
{
    private static readonly Lazy<InputEventsHolder> lazyInstance = new Lazy<InputEventsHolder>(() => new InputEventsHolder());

    public static InputEventsHolder Instance { get { return lazyInstance.Value; } }

    public Action EnableInput;
    public Action DisableInput;

    public Action EnableCharacterInput;
    public Action DisableCharacterInput;
    public Action<bool> SetActiveCharacterLookAround { get; set; }
    public Action<bool> SetActiveCharacterMove;

    public Action<GameObject> EnableVehicleInput { get; set; }
    public Action<GameObject> EnableStaticVehicleInput { get; set; }
}
