using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WASD : CustomBehaviour
{
    public InputActionAsset potatis;
    // Start is called before the first frame update
    void Awake()
    {
        CustomInput.ImportAsset(potatis, "Movement");
        CustomInput.GetDirectionalInput("Movement");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(CustomInput.GetDirectionalInput("Movement")); 
    }
}