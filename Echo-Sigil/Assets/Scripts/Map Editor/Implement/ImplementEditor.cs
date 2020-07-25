using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImplementEditor : MonoBehaviour
{
    private Implement selectedImplement;
    public Unit selectedUnit;

    //Windows
    public GameObject windowObject;
    public GameObject selectionObject;
    public GameObject splashObject;
    public GameObject animationsObject;
    public GameObject abilityObject;

    //Tabs
    public Button selectionButton;
    public Button splashButton;
    public Button animationButton;
    public Button abilityButton;

    //toolbox
    public Dropdown viewMenu;
    public Dropdown unitMenu;

    //Selection
    public GameObject implementHolderObject;
    public GameObject implementSelectionObject;
    public GameObject newImplementSelectionObject;
    public event Action<Implement> SelectionEvent;

    //Splash
    public ColorSlider primaryColor;
    public ColorSlider secondaryColor;
    public Image primaryColorImage;
    public Image secondayColorImage;
    public InputField nameField;
    public InputField descriptionField;

    //Animation
    public GameObject animationSelectionObject;

    //Ability
    public GameObject abilitySelectionObject;


    private void Start()
    {
        viewMenu.onValueChanged.AddListener(HandleVeiw);
        unitMenu.onValueChanged.AddListener(ChangeWindow);
        selectionButton.onClick.AddListener(delegate { ChangeWindow(1); });
        animationButton.onClick.AddListener(delegate { ChangeWindow(2); });
        splashButton.onClick.AddListener(delegate { ChangeWindow(3); });
        abilityButton.onClick.AddListener(delegate { ChangeWindow(4); });
    }

    private void OnDestroy()
    {
        UnsubscribeSelectionElements();
        UnsubscribeTabToolbarElements();
        UnsubscribeSplashElements();
    }

    private void UnsubscribeTabToolbarElements()
    {
        viewMenu.onValueChanged.RemoveAllListeners();
        unitMenu.onValueChanged.RemoveAllListeners();
        selectionButton.onClick.RemoveAllListeners();
        splashButton.onClick.RemoveAllListeners();
        animationButton.onClick.RemoveAllListeners();
        abilityButton.onClick.RemoveAllListeners();
    }

    private void UnsubscribeSelectionElements()
    {
        foreach (Transform toDelete in implementHolderObject.transform)
        {
            toDelete.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(toDelete.gameObject);
        }
    }

    private void HandleVeiw(int arg0)
    {
        viewMenu.value = 0;
        switch (arg0)
        {
            case 1:
                throw new NotImplementedException();
            case 2:
                CloseWindow();
                break;
            case 3:
                ChangeWindow(1);
                break;
        }
    }

    private void ChangeWindow(int arg0)
    {
        UnsubscribeSplashElements();
        DisableAllWindows();
        switch (arg0)
        {
            case 1:
                selectedImplement = SaveSystem.SaveUnitJson(selectedImplement);
                PopulateSelection();
                selectionObject.SetActive(true);
                break;
            case 2:
                animationsObject.SetActive(true);
                break;
            case 3:
                EnableSplashScreen();
                break;
            case 4:
                abilityObject.SetActive(true);
                break;
        }
    }

    private void DisableAllWindows()
    {
        unitMenu.value = 0;
        windowObject.SetActive(true);
        selectionObject.SetActive(false);
        animationsObject.SetActive(false);
        splashObject.SetActive(false);
        abilityObject.SetActive(false);
    }

    private void EnableSplashScreen()
    {
        //set
        nameField.text = selectedImplement.name;
        descriptionField.text = selectedImplement.description;
        primaryColor.Color = selectedImplement.PrimaryColor;
        secondaryColor.Color = selectedImplement.SecondaryColor;
        //subscribe
        nameField.onEndEdit.AddListener(delegate { selectedImplement.name = nameField.text; });
        descriptionField.onEndEdit.AddListener(delegate { selectedImplement.description = descriptionField.text; });
        primaryColor.ColorChangedEvent += selectedImplement.SetPrimaryColor;
        secondaryColor.ColorChangedEvent += selectedImplement.SetSecondayColor;
        //activate
        splashObject.SetActive(true);
    }

    private void UnsubscribeSplashElements()
    {
        nameField.onEndEdit.RemoveAllListeners();
        descriptionField.onEndEdit.RemoveAllListeners();
        primaryColor.ColorChangedEvent -= selectedImplement.SetPrimaryColor;
        secondaryColor.ColorChangedEvent -= selectedImplement.SetSecondayColor;
    }

    public void CloseWindow()
    {
        windowObject.SetActive(false);
        UnsubscribeSplashElements();
        UnsubscribeSelectionElements();
        selectedImplement = SaveSystem.SaveUnitJson(selectedImplement);
    }

    private void Update()
    {
        if (windowObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }

            if (selectionObject.activeInHierarchy)
            {

            }
            else if (animationsObject.activeInHierarchy)
            {

            }
            else if (splashObject.activeInHierarchy)
            {
                primaryColorImage.color = primaryColor.Color;
                secondayColorImage.color = secondaryColor.Color;
            }
            else if (abilityObject.activeInHierarchy)
            {

            }
        }

    }

    private void PopulateSelection()
    {
        UnsubscribeSelectionElements();
        string[] unitFolders = Directory.GetDirectories(Application.dataPath + "/Implements");
        foreach (string unitFolder in unitFolders)
        {
            if (File.Exists(unitFolder + "/ImplentData.json"))
            {
                Implement unit = SaveSystem.LoadUnitJson(unitFolder);
                GameObject unitObject = Instantiate(implementSelectionObject, implementHolderObject.transform);
                Button unitButton = unitObject.GetComponent<Button>();
                unitButton.onClick.AddListener(delegate { selectedImplement = SaveSystem.LoadUnitJson(unitFolder); ChangeWindow(3); });
                unitObject.GetComponentInChildren<Text>().text = unit.name;
                unitObject.GetComponentInChildren<Image>().sprite = SaveSystem.LoadPNG(unitFolder + "/Base.png", Vector2.one / 2f);
            }
        }
        Instantiate(newImplementSelectionObject, implementHolderObject.transform).GetComponent<Button>().onClick.AddListener(delegate { CreateNewImplement(); });
    }

    private Implement CreateNewImplement()
    {
        selectedImplement = new Implement("temp");
        ChangeWindow(3);
        return selectedImplement;
    }
}
