using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public bool isJump = false;

    public bool leftSkill = false;
    public bool canLeftSkill = true;

    public bool rightSkill = false;
    public bool canRightSkill = true;

    private WaitForSeconds wfs = new WaitForSeconds(4f);

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
            if (canLeftSkill)
            {
                leftSkill = true;
            }
            
        }
        else if (context.phase.Equals(InputActionPhase.Canceled))
        {
            leftSkill = false;
            if (canLeftSkill)
            {
                StartCoroutine(LeftSkillCool());
            }
            
        }
    }

    public void EventRightMouse(InputAction.CallbackContext context)
    {
        if (context.phase.Equals(InputActionPhase.Performed))
        {
            if (canRightSkill)
            {
                rightSkill = true;
            }
        }
        else if (context.phase.Equals(InputActionPhase.Canceled))
        {
            rightSkill = false;
            if (canRightSkill)
            {
                StartCoroutine(RightSkillCool());
            }
        }
    }

    private IEnumerator LeftSkillCool()
    {
        canLeftSkill = false;
        yield return wfs;
        canLeftSkill = true;
    }
    private IEnumerator RightSkillCool()
    {
        canRightSkill = false;
        yield return wfs;
        canRightSkill = true;
    }
}
