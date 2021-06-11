using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public InputActionAsset input;

    // Start is called before the first frame update
    void Awake()
    {
        CustomInput.ImportAsset(input, CustomInput.DirectionalKeys, CustomInput.MouseDelta);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(CustomInput.GetDirectionalInput(CustomInput.DirectionalKeys));
    }

    private void OnDisable()
    {
        CustomInput.SetInputActive(false);
    }
}