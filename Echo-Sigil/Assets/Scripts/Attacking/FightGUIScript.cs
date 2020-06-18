using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGUIScript : MonoBehaviour
{
    public Animator animator;
    static Animator staticAnimator;
    private void Start()
    {
        staticAnimator = animator;
    }
    public static void SetBattle()
    {
        staticAnimator.SetTrigger("Battle");
    }

}
