﻿using System;
using UnityEngine;

[Serializable]
public struct Implement
{
    public string name;
    public int index;
    public string description;
    public float[] primaryColor;
    public float[] secondaryColor;
    public Animation[] animations;

    public Color PrimaryColor { get => new Color(primaryColor[0], primaryColor[1], primaryColor[2]); set => SetUnitColors(value, SecondaryColor); }
    public Color SecondaryColor { get => new Color(secondaryColor[0], secondaryColor[1], secondaryColor[2]); set => SetUnitColors(PrimaryColor, value); }
    public Sprite BaseSprite(string modPath) => SaveSystem.LoadPNG(modPath + "/" + name + "/Base.png", Vector2.one / 2f);

    public Implement(string name,int index)
    {
        primaryColor = new float[3];
        secondaryColor = new float[3];
        this.index = index;
        this.name = name;
        description = "";
        animations = new Animation[0];
    }

    [Serializable]
    public struct Animation
    {
        public string name;
        public int index;
        public int framerate;
        public bool directional;
        public bool variant;
        public bool multitile;
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
        this.primaryColor[0] = primaryColor.r;
        this.primaryColor[1] = primaryColor.g;
        this.primaryColor[2] = primaryColor.b;
        this.secondaryColor[0] = secondaryColor.r;
        this.secondaryColor[1] = secondaryColor.g;
        this.secondaryColor[2] = secondaryColor.b;
        return this;
    }

    internal void SetPrimaryColor(Color obj) => PrimaryColor = obj;

    internal void SetSecondayColor(Color obj) => SecondaryColor = obj;
}
[Serializable]
public class ImplementList
{
    public string modPath;
    public string modName;
    public Implement[] implements;
    public Sprite BaseSprite(int index) => implements[index].BaseSprite(modPath);

    public static implicit operator Implement[](ImplementList i) => i.implements;

    public ImplementList(int Length, string modPath = null, string modName = "Defualt")
    {
        this.modPath = SaveSystem.SetDefualtModPath(modPath);
        this.modName = modName;
        implements = new Implement[Length];
    }

    public ImplementList(Implement[] implements, string modPath = null, string modName = "Defualt")
    {
        this.modPath = SaveSystem.SetDefualtModPath(modPath);
        this.modName = modName;
        this.implements = implements;
    }
}