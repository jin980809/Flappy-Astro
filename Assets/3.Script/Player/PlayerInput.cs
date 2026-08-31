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

    [SerializeField] private float skillCooldown = 4f;

    // 남은 쿨타임(초). 0 이면 쿨 아님.
    public float leftCooldownRemaining = 0f;
    public float rightCooldownRemaining = 0f;

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
        leftCooldownRemaining = skillCooldown;
        while (leftCooldownRemaining > 0f)
        {
            leftCooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        leftCooldownRemaining = 0f;
        canLeftSkill = true;
    }
    private IEnumerator RightSkillCool()
    {
        canRightSkill = false;
        rightCooldownRemaining = skillCooldown;
        while (rightCooldownRemaining > 0f)
        {
            rightCooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        rightCooldownRemaining = 0f;
        canRightSkill = true;
    }
}
