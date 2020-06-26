using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pallate", menuName = "SpritePallate", order = 0)]
public class SpritePallate : ScriptableObject
{
    public int ID = 0;
    public Sprite[] sprites;
}
