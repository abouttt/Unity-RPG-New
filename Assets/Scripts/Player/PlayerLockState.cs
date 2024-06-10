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

    [Header("[Combat]")]
    public bool Attack;
    public bool Parry;
    public bool Defense;

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

        Player.Combat.CanAttack = Attack;
        Player.Combat.CanParry = Parry;
        Player.Combat.CanDefense = Defense;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        s_currentStateCount--;

        if (s_currentStateCount == 0 && stateInfo.normalizedTime >= UnlockTime)
        {
            Player.Movement.Enabled = true;
            Player.Movement.Clear();

            Player.Combat.Enabled = true;
            Player.Combat.Clear();
        }
    }

    private void OnDestroy()
    {
        s_currentStateCount = 0;
    }
}
