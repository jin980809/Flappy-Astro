using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public bool isJump = false;

    public bool leftSkill = false;

    public bool rightSkill = false;

    public Vector2 mousePosition = Vector2.zero;

    public void EventJump(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            isJump = true;
        }
        else if (context.phase.Equals(InputActionPhase.Canceled))
        {
            isJump = false;
        }
    }

    public void Event_MousePosition(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void EventLeftMouse(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            
            leftSkill = true;
            
        }
        else if (context.phase.Equals(InputActionPhase.Canceled))
        {
            leftSkill = false;
            
        }
    }

    public void EventRightMouse(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            rightSkill = true;
        }
        else if (context.phase.Equals(InputActionPhase.Canceled))
        {
            rightSkill = false;
        }
    }
}
