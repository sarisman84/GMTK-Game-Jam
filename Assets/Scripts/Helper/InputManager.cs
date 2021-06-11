using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public static class CustomInput
{
    public const string MouseDelta = "Look";
    public const string DirectionalKeys = "Movement";
    public const string Jump = "Jump";
    public const string Crouch = "Crouch";
    public const string Sprint = "Sprint";

    private static Dictionary<string, InputAction> _registeredActions;

    public static void ImportAsset(InputActionAsset asset, params string[] ids)
    {
        _registeredActions = new Dictionary<string, InputAction>();
        foreach (var id in ids)
        {
            InputAction foundAction = asset.FindAction(id);
            if (foundAction != null)
            {
                _registeredActions.Add(id, foundAction);
            }
        }
    }

    public static void SetInputActive(bool state, params string[] ids)
    {
        if (ids == null)
        {
            foreach (var registeredAction in _registeredActions)
            {
                if (state)
                    registeredAction.Value.Enable();
                else
                    registeredAction.Value.Disable();
            }

            return;
        }

        foreach (var id in ids)
        {
            if (state)
                _registeredActions[id].Enable();
            else
                _registeredActions[id].Disable();
        }
    }

    public static bool GetButton(string button)
    {
        InputAction input = GetAction(button);
        return input?.ReadValue<float>() > 0;
    }

    public static bool GetButtonDown(string button)
    {
        InputAction input = GetAction(button);
    

        return input?.ReadValue<float>() > 0 && input.triggered;
    }


    public static Vector2 GetDirectionalInput(string directionalInput)
    {
        InputAction input = GetAction(directionalInput);
        if (input == null)
            return Vector2.zero;


        return input.ReadValue<Vector2>();
    }


    private static InputAction GetAction(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (!_registeredActions.ContainsKey(id)) return null;
        if (!_registeredActions[id].enabled)
        {
            _registeredActions[id].Enable();
        }

        return _registeredActions[id];
    }
}