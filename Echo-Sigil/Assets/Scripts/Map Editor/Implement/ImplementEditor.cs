using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImplementEditor : MonoBehaviour
{
    private int selectedImplementIndex;
    private ImplementList selectedImplementList;
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
    /// <summary>
    /// Holds all implements for selection
    /// </summary>
    public Transform implementHolderTransform;
    /// <summary>
    /// Holds all implments of a mod for selection
    /// </summary>
    public GameObject implementModHolderObject;
    public GameObject implementSelectionObject;
    public GameObject newImplementSelectionObject;
    public Image splashScreenProfile;
    public Button addProfileButton;
    private List<Button> buttons = new List<Button>();
    public event Action<ImplementList, int> SelectionEvent;

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
        foreach (Button toDelete in buttons)
        {
            toDelete.onClick.RemoveAllListeners();
        }
        foreach (Transform toDelete in implementHolderTransform)
        {
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

    private void DeafualtSelectionEvent(ImplementList implementList, int index)
    {
        selectedImplementIndex = index;
        selectedImplementList = implementList;
        ChangeWindow(3);
    }

    private void ChangeWindow(int arg0)
    {
        DisableAllWindows();
        switch (arg0)
        {
            case 1:
                PopulateSelection();
                SelectionEvent += DeafualtSelectionEvent;
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

    private void Save()
    {
        if (selectedImplementList != null)
        {
            selectedImplementList.implements[selectedImplementIndex].PrimaryColor = primaryColor.Color;
            selectedImplementList.implements[selectedImplementIndex].SecondaryColor = secondaryColor.Color;
            SaveSystem.SaveImplmentList(selectedImplementList);
        }
    }
    /// <summary>
    /// Turns off all windows and saves game
    /// </summary>
    private void DisableAllWindows()
    {
        UnsubscribeSelectionElements();
        UnsubscribeSplashElements();
        Save();
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
        nameField.text = selectedImplementList.implements[selectedImplementIndex].name;
        descriptionField.text = selectedImplementList.implements[selectedImplementIndex].description;
        primaryColor.Color = selectedImplementList.implements[selectedImplementIndex].PrimaryColor;
        secondaryColor.Color = selectedImplementList.implements[selectedImplementIndex].SecondaryColor;
        if (selectedImplementList.implements[selectedImplementIndex].BaseSprite(selectedImplementList.modPath) == null)
        {
            splashScreenProfile.color = Color.clear;
        }
        else
        {
            splashScreenProfile.color = Color.white;
        }
        splashScreenProfile.sprite = selectedImplementList.implements[selectedImplementIndex].BaseSprite(selectedImplementList.modPath);

        //subscribe
        nameField.onEndEdit.AddListener(delegate { selectedImplementList.implements[selectedImplementIndex].name = nameField.text; SaveSystem.SaveImplmentList(selectedImplementList); });
        descriptionField.onEndEdit.AddListener(delegate { selectedImplementList.implements[selectedImplementIndex].description = descriptionField.text; SaveSystem.SaveImplmentList(selectedImplementList); });
        primaryColor.ColorChangedEvent += selectedImplementList.implements[selectedImplementIndex].SetPrimaryColor;
        secondaryColor.ColorChangedEvent += selectedImplementList.implements[selectedImplementIndex].SetSecondayColor;
        primaryColor.ColorChangedEvent += delegate { SaveSystem.SaveImplmentList(selectedImplementList); };
        secondaryColor.ColorChangedEvent += delegate { SaveSystem.SaveImplmentList(selectedImplementList); };
        addProfileButton.onClick.AddListener(call: AddProfile);

        //activate
        splashObject.SetActive(true);
    }

    private void AddProfile()
    {
        Sprite texture = SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Set Profile", Application.persistentDataPath, "png"), Vector2.one); 
        SaveSystem.SavePNG(selectedImplementList.modPath + "/" + selectedImplementList.implements[selectedImplementIndex].name + "/Base.png", texture.texture); 
        splashScreenProfile.color = Color.white; 
        splashScreenProfile.sprite = texture;
    }

    private void UnsubscribeSplashElements()
    {
        nameField.onEndEdit.RemoveAllListeners();
        descriptionField.onEndEdit.RemoveAllListeners();
        primaryColor.Unsubscribe();
        secondaryColor.Unsubscribe();
        addProfileButton.onClick.RemoveAllListeners();
    }

    public void CloseWindow()
    {
        DisableAllWindows();
        windowObject.SetActive(false);
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

    private void PopulateSelection(string[] modPaths = null)
    {
        if (modPaths == null)
        {
            modPaths = new string[1];
            modPaths[0] = SaveSystem.SetDefualtModPath(modPaths[0]);
        }

        UnsubscribeSelectionElements();
        foreach (string modPath in modPaths)
        {
            Transform modHolder = Instantiate(implementModHolderObject, implementHolderTransform).transform;
            ImplementList implementList = SaveSystem.LoadImplementList(modPath);
            modHolder.GetChild(0).GetChild(0).GetComponent<Text>().text = implementList.modName;

            if (implementList.implements != null)
            {
                for (int i = 0; i < implementList.implements.Length; i++)
                {
                    if (implementList.implements[i].index == i)
                    {
                        GameObject unitObject = Instantiate(implementSelectionObject, modHolder);
                        Button unitButton = unitObject.GetComponent<Button>();
                        int index = i;
                        unitButton.onClick.AddListener(delegate { SelectionEvent?.Invoke(implementList, index); });
                        buttons.Add(unitButton);
                        unitObject.GetComponentInChildren<Text>().text = i.ToString() + ": " + implementList.implements[i].name;
                        Sprite sprite = implementList.BaseSprite(i);
                        Image image = unitObject.transform.GetChild(0).GetComponent<Image>();
                        if (sprite == null)
                        {

                            image.color = Color.clear;
                        }
                        image.sprite = sprite;
                    }
                }
            }
            Button addButton = Instantiate(newImplementSelectionObject, modHolder).GetComponent<Button>();
            addButton.onClick.AddListener(delegate { CreateNewImplement(modPath); });
            buttons.Add(addButton);
        }

    }

    private void CreateNewImplement(string modPath = null)
    {
        modPath = SaveSystem.SetDefualtModPath(modPath);
        ImplementList implementList = SaveSystem.LoadImplementList();
        int length = 0;
        if (implementList.implements != null)
        {
            length = implementList.implements.Length;
        }

        Implement[] implements = new Implement[length + 1];
        implementList.implements.CopyTo(implements, 0);
        implements[length] = new Implement("temp", -1);
        implementList = SaveSystem.SaveImplmentList(new ImplementList(implements, modPath));
        selectedImplementList = implementList;
        selectedImplementIndex = length;
        ChangeWindow(3);
        nameField.onEndEdit.AddListener(delegate { implements[length].index = length; SaveSystem.SaveImplmentList(new ImplementList(implements, modPath, implementList.modName)); });
    }
}
