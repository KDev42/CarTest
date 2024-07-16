using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InputEventsHolder
{
    private static readonly Lazy<InputEventsHolder> lazyInstance = new Lazy<InputEventsHolder>(() => new InputEventsHolder());

    public static InputEventsHolder Instance { get { return lazyInstance.Value; } }

    public Action EnableInput { get; set; }
    public Action DisableInput { get; set; }

    public Action<GameObject> EnableVehicleInput { get; set; }
}
