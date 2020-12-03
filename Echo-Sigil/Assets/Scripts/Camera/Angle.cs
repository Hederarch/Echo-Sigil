using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct Angle
{
    public float angleInRadians;
    public float AngleInDegrees { get => angleInRadians * Mathf.Rad2Deg; set => angleInRadians = value * Mathf.Deg2Rad; }
    public float X => Mathf.Sin(angleInRadians);
    public float Y => Mathf.Cos(angleInRadians);
    public Vector2 Vector => new Vector2(X, Y);

    public Angle(float angleInRadians)
    {
        this.angleInRadians = angleInRadians;
    }

    public override string ToString()
    {
        return (angleInRadians * Mathf.Rad2Deg).ToString();
    }

    /// <summary>
    /// Is it faster to add to a to get to b?
    /// </summary>
    /// <param name="a">Sourse</param>
    /// <param name="b">Destination</param>
    /// <returns></returns>
    public static bool Sign(Angle a, Angle b)
    {
        float minusAmount;
        float plusAmount;
        if (a.angleInRadians > b.angleInRadians)
        {
            minusAmount = a.angleInRadians - b.angleInRadians;
            plusAmount = 2 * Mathf.PI - minusAmount;
        }
        else
        {
            plusAmount = b.angleInRadians - a.angleInRadians;
            minusAmount = 2 * Mathf.PI - plusAmount;

        }
        return plusAmount < minusAmount;
    }

    public static implicit operator float(Angle a) => a.angleInRadians;
    public static implicit operator Vector2(Angle a) => a.Vector;

    public static implicit operator Angle(float f) => new Angle(f);
    public static explicit operator Angle(Vector2 v)
    {
        v.Normalize();
        return new Angle(Mathf.Atan2(v.y, v.x));
    }

    public static Angle operator +(Angle a, float f)
    {
        a.angleInRadians += f;
        //clamp between 0 and 360
        if (a.angleInRadians > Mathf.PI * 2)
        {
            a.angleInRadians -= Mathf.PI * 2;
        }
        else if (a.angleInRadians < 0)
        {
            a.angleInRadians += Mathf.PI * 2;
        }
        //just in case the top bit dosent work;
        a.angleInRadians = Mathf.Clamp(a.angleInRadians, 0, Mathf.PI * 2);
        return a;
    }
    public static Angle operator -(Angle a, float f) => a + (-f);
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Angle))]
public class AngleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * (property.isExpanded ? 3 : 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty angleInRadians = property.FindPropertyRelative("angleInRadians");
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, false);
        position = EditorGUI.PrefixLabel(position, new GUIContent(" "));
        if (property.isExpanded)
        {
            Rect drawSpace = new Rect(position.x + (position.width - GetPropertyHeight(property, label)), position.y, GetPropertyHeight(property, label), GetPropertyHeight(property, label));
            EditorGUI.DrawRect(drawSpace, Color.black * .2f);
            position.width -= GetPropertyHeight(property, label);

            Vector2 angleVector = new Vector2(Mathf.Sin(angleInRadians.floatValue), Mathf.Cos(angleInRadians.floatValue));
            Vector2 centerDrawSpace = drawSpace.position;
            float halfHeight = GetPropertyHeight(property, label) * .5f;
            centerDrawSpace.x += halfHeight - 1;
            centerDrawSpace.y += halfHeight - 1;
            Rect anglePoint = new Rect(centerDrawSpace + (angleVector * (halfHeight - 4)), Vector2.one * 3);
            EditorGUI.DrawRect(anglePoint, GUI.enabled ? Color.green : Color.green * .5f);

            position.height = GetPropertyHeight(property, label) / 3;
            position.y += GetPropertyHeight(property, label) / 3;
        }
        angleInRadians.floatValue = EditorGUI.Slider(position, angleInRadians.floatValue * Mathf.Rad2Deg, 0, 360) * Mathf.Deg2Rad;
    }
}
#endif