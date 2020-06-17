using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGBattle : MonoBehaviour
{
    public readonly bool leftSide;

    public int health =5;
    public int maxHealth = 5;

    public int will =5;
    public int maxWill = 5;

    public float HealthPercent { get => health/maxHealth; }
    public float WillPercent { get => will / maxWill; }

    public List<Ability> abilites = new List<Ability>();
}
