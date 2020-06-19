using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGUIScript : MonoBehaviour
{
    public Animator animator;
    public Animator playerGUIAnimator;
    static Animator staticAnimator;
    static Animator staticPlayerGUIAnimator;
    private void Start()
    {
        staticAnimator = animator;
        staticPlayerGUIAnimator = playerGUIAnimator;
    }
    public static void SetBattle()
    {
        staticAnimator.SetBool("Battle",true);
        staticPlayerGUIAnimator.SetBool("Battle",true);
    }
    public static void UnSetBattle()
    {
        staticAnimator.SetBool("Battle", false);
        staticPlayerGUIAnimator.SetBool("Battle", false);
    }

}
