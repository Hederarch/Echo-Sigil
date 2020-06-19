using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightGUIScript : MonoBehaviour
{
    public Animator animator;
    public Animator playerGUIAnimator;
    static Animator staticAnimator;
    static Animator staticPlayerGUIAnimator;

    public Transform menuGUI;
    static Transform staticMenuGUI;
    public GameObject menuGUIItem;
    static GameObject staticMenuGUIItem;

    public Transform statsGUI;
    static Transform staticStatsGUI;
    public GameObject statsGUIItem;
    static GameObject staticStatsGUIItem;

    private void Start()
    {
        staticAnimator = animator;
        staticPlayerGUIAnimator = playerGUIAnimator;

        staticMenuGUI = menuGUI;
        staticMenuGUIItem = menuGUIItem;

        staticStatsGUI = statsGUI;
        staticStatsGUIItem = statsGUIItem;
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

    public static void SetMenu(JRPGBattle unit)
    {
        foreach(Ability a in unit.abilites)
        {
            GameObject m = Instantiate(staticMenuGUIItem,staticMenuGUI);
            m.name = a.name;
            m.GetComponentInChildren<Text>().text = a.name;
        }
    }

    public static void SetStats()
    {
        List<JRPGBattle> units = Camera.main.GetComponent<JRPGBattleCamera>().units;
        if (staticStatsGUI.childCount == 0)
        {
            foreach (JRPGBattle j in units)
            {
                Instantiate(staticStatsGUIItem, staticStatsGUI);
            }
        }
        for(int i=0; i < staticStatsGUI.childCount; i++)
        {
            Transform statItem = staticStatsGUI.GetChild(i);
            statItem.GetComponentInChildren<Text>().text = units[i].name;
            Transform sliderItem = statItem.GetChild(1);
            sliderItem.GetChild(0).GetComponent<Slider>().value = units[i].GetHealthPercent();
            sliderItem.GetChild(1).GetComponent<Slider>().value = units[i].GetWillPercent();
        }

    }

    public static void ResetMenuStats()
    {
        for(int i= staticMenuGUI.childCount - 1; i  > -1 ; i--)
        {
            Destroy(staticMenuGUI.GetChild(i).gameObject);
        }
        for (int i = staticStatsGUI.childCount -1 ; i > -1; i--)
        {
            Destroy(staticStatsGUI.GetChild(i).gameObject);
        }
    }
}
