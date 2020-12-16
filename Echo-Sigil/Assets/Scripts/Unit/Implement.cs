using UnityEngine;

public class Implement
{
    public string name;
    public int modPathIndex;
    public Artifact artifact;
    public Sprite baseSprite;

    public Implement(string name, Sprite defaultBaseSprite)
    {
        this.name = name;
        baseSprite = defaultBaseSprite;
        modPathIndex = -1;
    }

    public AnimatorOverrideController animationController => SaveSystem.Unit.GetAnimationControler(name, baseSprite); 
}
