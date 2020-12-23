using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionGUI : MonoBehaviour
{
    public static SelectionGUI instance;
    public Text nameText;
    public Image iconStencil;
    public Image baseImage;
    public Button moveButton;
    public CanvasGroup transparency;

    private void Start()
    {
        instance = this;
        Cursor.GotCursorEvent += SetSelected;
    }

    public void SetSelected() => SetSelected(Cursor.unit);

    public void SetSelected(Unit unit)
    {
        if (unit == null)
        {
            transparency.alpha = 0;
        }
        else
        {
            transparency.alpha = 1;
            moveButton.gameObject.SetActive(unit.GetType() == typeof(PlayerUnit));
            baseImage.sprite = unit.implement.baseSprite;
            nameText.text = unit.name;
        }
    }
    public void PlayerMove()
    {
        Debug.Log("The player wants to move you see");
    }

    private void OnDestroy()
    {
        Cursor.GotCursorEvent -= SetSelected;
    }
}
