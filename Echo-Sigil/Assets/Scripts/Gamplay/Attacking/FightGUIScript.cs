using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static void SetBattleAnimations()
    {
        staticAnimator.SetTrigger("Battle");
        staticPlayerGUIAnimator.SetBool("Battle",true);
    }

    public static void UnSetBattleAnimations()
    {
        staticAnimator.SetTrigger("Battle");
        staticPlayerGUIAnimator.SetBool("Battle", false);
    }

    public static void SetMenu()
    {
        foreach (JRPGBattle j in BattleData.leftCombatants)
        {
            foreach (Ability a in j.abilites.Keys.ToArray())
            {
                GameObject m = Instantiate(staticMenuGUIItem, staticMenuGUI);
                m.name = a.name;
                m.GetComponentInChildren<Text>().text = a.name;
                m.GetComponent<Button>().onClick.AddListener(() => a.ActivateAbility());
                m.GetComponent<Button>().onClick.AddListener(() => SetStats());
            }
        }
    }

    public static void SetStats()
    {
        if (staticStatsGUI.childCount == 0)
        {
            for (int i = 0; i < BattleData.leftCombatants.Count; i++)
            {
                Instantiate(staticStatsGUIItem, staticStatsGUI);
            }
        }
        for(int i=0; i < staticStatsGUI.childCount; i++)
        {
            Transform statItem = staticStatsGUI.GetChild(i);
            statItem.GetComponentInChildren<Text>().text = BattleData.leftCombatants[i].name;
            Transform sliderItem = statItem.GetChild(1);
            sliderItem.GetChild(0).GetComponent<Slider>().value = BattleData.leftCombatants[i].GetHealthPercent();
            sliderItem.GetChild(1).GetComponent<Slider>().value = BattleData.leftCombatants[i].GetWillPercent();
        }

    }

    public static void ResetMenuAndStats()
    {
        for(int i= staticMenuGUI.childCount - 1; i  > -1 ; i--)
        {
            GameObject m = staticMenuGUI.GetChild(i).gameObject;
            m.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(m);
        }
        for (int i = staticStatsGUI.childCount -1 ; i > -1; i--)
        {
            Destroy(staticStatsGUI.GetChild(i).gameObject);
        }
    }
}
