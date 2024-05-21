using System;
using UnityEngine;

public class PlayerLockState : StateMachineBehaviour
{
    [Header("Can Behaviour")]
    [Header("[Movement]")]
    public bool Move;
    public bool Rotation;
    public bool Sprint;
    public bool Jump;
    public bool Roll;

    [Range(0f, 1f)]
    public float UnlockTime = 0f;

    private static int s_currentStateCount = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        s_currentStateCount++;

        Player.Movement.CanMove = Move;
        Player.Movement.CanRotation = Rotation;
        Player.Movement.CanSprint = Sprint;
        Player.Movement.CanJump = Jump;
        Player.Movement.CanRoll = Roll;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        s_currentStateCount--;

        if (s_currentStateCount == 0 && stateInfo.normalizedTime >= UnlockTime)
        {
            Player.Movement.Enabled = true;
            Player.Movement.Clear();
        }
    }

    private void OnDestroy()
    {
        s_currentStateCount = 0;
    }
}
