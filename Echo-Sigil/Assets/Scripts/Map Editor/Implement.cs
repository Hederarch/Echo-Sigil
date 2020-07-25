using System;
using UnityEngine;

[Serializable]
public struct Implement
{
    public string name;
    public string path;
    public string description;
    public float[] primaryColor;
    public float[] secondaryColor;
    public Animation[] animations;

    public Color PrimaryColor { get => new Color(primaryColor[0], primaryColor[1], primaryColor[2]); set => SetUnitColors(value, SecondaryColor); }
    public Color SecondaryColor { get => new Color(secondaryColor[0], secondaryColor[1], secondaryColor[2]); set => SetUnitColors(PrimaryColor, value); }

    public Implement(string name)
    {
        primaryColor = new float[3];
        secondaryColor = new float[3];
        this.name = name;
        if(SaveSystem.developerMode)
        {
            path = Application.dataPath + "/Implements/" + name;
        } else
        {
            path = SaveSystem.curModPath + "/Implements/" + name;
        }

        description = "";
        animations = new Animation[0];
    }

    [Serializable]
    public struct Animation
    {
        public string name;
        public string path;
    }

    public static Implement SetUnitColors(Implement unit, Color primaryColor, Color secondaryColor)
    {
        unit.primaryColor[0] = primaryColor.r;
        unit.primaryColor[1] = primaryColor.g;
        unit.primaryColor[2] = primaryColor.b;
        unit.secondaryColor[0] = secondaryColor.r;
        unit.secondaryColor[1] = secondaryColor.g;
        unit.secondaryColor[2] = secondaryColor.b;
        return unit;
    }

    public Implement SetUnitColors(Color primaryColor, Color secondaryColor)
    {
        primaryColor[0] = primaryColor.r;
        primaryColor[1] = primaryColor.g;
        primaryColor[2] = primaryColor.b;
        secondaryColor[0] = secondaryColor.r;
        secondaryColor[1] = secondaryColor.g;
        secondaryColor[2] = secondaryColor.b;
        return this;
    }

    internal void SetPrimaryColor(Color obj) => PrimaryColor = obj;

    internal void SetSecondayColor(Color obj) => SecondaryColor = obj;
}