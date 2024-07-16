using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] CarMovement carMovement;
    [SerializeField] bool useEffects = false;
    [SerializeField] ParticleSystem rlWParticleSystem;
    [SerializeField] ParticleSystem rrWParticleSystem;
    [SerializeField] TrailRenderer rlWTireSkid;
    [SerializeField] TrailRenderer rrWTireSkid;

    private void Start()
    {
        if (!useEffects)
        {
            if (rlWParticleSystem != null)
            {
                rlWParticleSystem.Stop();
            }
            if (rrWParticleSystem != null)
            {
                rrWParticleSystem.Stop();
            }
            if (rlWTireSkid != null)
            {
                rlWTireSkid.emitting = false;
            }
            if (rrWTireSkid != null)
            {
                rrWTireSkid.emitting = false;
            }
        }
    }

    public void DriftCarPS()
    {
        if (useEffects)
        {
            try
            {
                if (carMovement.IsDrifting)
                {
                    rlWParticleSystem.Play();
                    rrWParticleSystem.Play();
                }
                else if (!carMovement.IsDrifting)
                {
                    rlWParticleSystem.Stop();
                    rrWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((carMovement.IsTractionLocked || Mathf.Abs(carMovement.LocalVelocity.x) > 5f) && Mathf.Abs(carMovement.CarSpeed) > 12f)
                {
                    rlWTireSkid.emitting = true;
                    rrWTireSkid.emitting = true;
                }
                else
                {
                    rlWTireSkid.emitting = false;
                    rrWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (rlWParticleSystem != null)
            {
                rlWParticleSystem.Stop();
            }
            if (rrWParticleSystem != null)
            {
                rrWParticleSystem.Stop();
            }
            if (rlWTireSkid != null)
            {
                rlWTireSkid.emitting = false;
            }
            if (rrWTireSkid != null)
            {
                rrWTireSkid.emitting = false;
            }
        }

    }
}
