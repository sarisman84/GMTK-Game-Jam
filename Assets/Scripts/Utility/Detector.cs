using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> ONTriggerEnter;
    private void OnTriggerEnter(Collider other)
    {
        ONTriggerEnter?.Invoke(other);
    }
}
