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
    private GameObject _exit;

    public bool DisableTracker { set; private get; }

    private void Start()
    {
    }

    // private void Update()
    // {
    //     if (!_exit)
    //     {
    //         _exit = LevelManager.GetGameObjectFromActiveSceneWithTag("Level/Exit")?.gameObject;
    //         pointerHUD.enabled = false;
    //     }
    //     else
    //     {
    //         pointerHUD.enabled = _exit.activeSelf && !DisableTracker;
    //
    //
    //         if (pointerHUD.enabled)
    //         {
    //             Vector3 dir = _exit.transform.position - GameMaster.singletonAccess.playerObject.transform.position;
    //             dir = new Vector3(dir.normalized.x, 0, dir.normalized.z);
    //             pointerModel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    //         }
    //     }
    // }
}