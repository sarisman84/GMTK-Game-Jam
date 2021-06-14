using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> ONTriggerEnter;
    public event Action ONUpdateEvent;
    public int additionalPossessionsRequired = 1;
    internal int currentPossessionRequired;
    private void OnTriggerEnter(Collider other)
    {
        ONTriggerEnter?.Invoke(other);
    }

    private void Update()
    {
        ONUpdateEvent?.Invoke();
    }

    public void ResetEvents()
    {
        ONTriggerEnter?.RemoveAllListeners();
        ONUpdateEvent = null;
    }

    
}