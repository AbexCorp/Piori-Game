using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IMouseInteractable
{
    public void OnHoverEnter(InputAction.CallbackContext context);
    public void OnHoveExit(InputAction.CallbackContext context);
    public void OnClick(InputAction.CallbackContext context);
}
