using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> ONTriggerEnter;
    public int additionalPossessionsRequired = 1;

    private void OnTriggerEnter(Collider other)
    {
        ONTriggerEnter?.Invoke(other);
    }
}