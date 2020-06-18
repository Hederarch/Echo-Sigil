using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUIScript : MonoBehaviour
{
    public static event Action MoveEvent;
    public static event Action AttackEvent;

    public Animator animator;

    public Image guiCharacterImageRenderer;
    public Slider healthSlider;
    public Slider willSlider;

    private void Start()
    {
        Implement.IsTurnEvent += SetAsCurrentUnit;
    }

    private void Update()
    {
        animator.SetBool("isPlayerTurn", TurnManager.isPlayerTurn);
    }

    public void Move()
    {
        if (TurnManager.isPlayerTurn)
        {
            MoveEvent?.Invoke();
        }
    }

    public void Attack()
    {
        if (TurnManager.isPlayerTurn)
        {
            AttackEvent?.Invoke();
        }
    }

    public void Skip()
    {
        if (TurnManager.isPlayerTurn)
        {
            TurnManager.EndTurn();
        }
    }

    void SetAsCurrentUnit(Implement unit)
    {
        guiCharacterImageRenderer.sprite = unit.unitSprite.sprite;
        SetSliders(unit.battle);
    }

    void SetSliders(IBattle unit)
    {
        healthSlider.value = unit.GetHealthPercent();
        willSlider.value = unit.GetWillPercent();
    }

}
