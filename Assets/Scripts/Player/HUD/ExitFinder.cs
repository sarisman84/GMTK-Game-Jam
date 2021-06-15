using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ExitFinder : MonoBehaviour
{
    public GameObject pointerModel;
    public RawImage pointerHUD;
    private Vector3 _exit;
    public Func<bool> ExitFinderCondition;


    private void Awake()
    {
        ExitFinderCondition = () => true;
    }

    private void Update()
    {
        if (pointerHUD.enabled)
        {
            Vector3 dir = _exit - transform.position;
            dir = new Vector3(dir.normalized.x, 0, dir.normalized.z);
            pointerModel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    public void SetTarget(Detector foundDetector)
    {
        if (!foundDetector)
        {
            SetActive(false);
            return;
        }

        SetActive(true);
        _exit = foundDetector.transform.position;
    }

    public void SetActive(bool state)
    {
        pointerHUD.enabled = state && ExitFinderCondition != null && ExitFinderCondition.Invoke();
    }
}