using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool current;
    public bool target;
    public bool selectable;

    //BFS stuff
    public bool visited;
    public Tile parent;
    public int distance;

}
