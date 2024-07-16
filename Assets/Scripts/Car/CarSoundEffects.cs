using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CarSoundEffects : MonoBehaviour
{
    [SerializeField] CarMovement carMovement;
    [SerializeField] bool useSounds = false;
    [SerializeField] AudioSource carEngineSound;
    [SerializeField] AudioSource tireScreechSound;

    private float initialCarEngineSoundPitch;

    private void Start()
    {
        if (carEngineSound != null)
        {
            initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        if (useSounds)
        {
            InvokeRepeating("CarSounds", 0f, 0.1f);
        }
        else if (!useSounds)
        {
            if (carEngineSound != null)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null)
            {
                tireScreechSound.Stop();
            }
        }
    }

    public void CarSounds()
    {

        if (useSounds)
        {
            try
            {
                if (carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carMovement.LocalVelocity.magnitude) / 25f);
                    carEngineSound.pitch = engineSoundPitch;
                }
                if ((carMovement.IsDrifting) || (carMovement.IsTractionLocked && Mathf.Abs(carMovement.CarSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Play();
                    }
                }
                else if ((!carMovement.IsDrifting) && (!carMovement.IsTractionLocked || Mathf.Abs(carMovement.CarSpeed) < 12f))
                {
                    tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }

    }
}
