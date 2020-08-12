using mapEditor.animations;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace mapEditor
{
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

        private void ChangeWindow(int arg0)
        {
            Save();
            DisableAllWindows();
            switch (arg0)
            {
                case 1:
                    PopulateSelection();
                    SelectionEvent += DefualtSelectionEvent;
                    selectionObject.SetActive(true);
                    break;
                case 2:
                    PopulateAnimation();
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
            if (selectedImplementList != null && selectedImplementList.implements.Length > selectedImplementIndex)
            {
                Implement implement = selectedImplementList.implements[selectedImplementIndex];
                if (selectionObject.activeInHierarchy)
                {

                }
                else if (animationsObject.activeInHierarchy)
                {
                    implement.animations = SaveAnimations(selectedImplementList.ImplementPath(selectedImplementIndex));
                    implement = SetAnimationIdexes(implement);
                }
                else if (splashObject.activeInHierarchy)
                {
                    implement.name = splashNameField.text;
                    implement.fragment = splashFragmentField.text;
                    implement.power = splashPowerField.text;
                    implement.type = splashType;
                    implement.description = splashDescriptionField.text;
                    implement.SetBaseSprite(selectedImplementList.modPath, splashScreenProfile.sprite);
                    implement.PrimaryColor = primaryColor.Color;
                    implement.SecondaryColor = secondaryColor.Color;
                }
                else if (abilityObject.activeInHierarchy)
                {

                }
                selectedImplementList.implements[selectedImplementIndex] = implement;
                SaveSystem.SaveImplmentList(selectedImplementList);
            }
        }



        /// <summary>
        /// Turns off all windows and saves game
        /// </summary>
        private void DisableAllWindows()
        {
            Save();
            UnsubscribeSelectionElements();

            unitMenu.value = 0;
            windowObject.SetActive(true);
            selectionObject.SetActive(false);
            animationsObject.SetActive(false);
            splashObject.SetActive(false);
            abilityObject.SetActive(false);
        }
        public void CloseWindow()
        {
            DisableAllWindows();
            windowObject.SetActive(false);
        }

        //Tabs
        public Button selectionButton;
        public Button splashButton;
        public Button animationButton;
        public Button abilityButton;

        //toolbox
        public Dropdown viewMenu;
        public Dropdown unitMenu;

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
        private void UnsubscribeTabToolbarElements()
        {
            viewMenu.onValueChanged.RemoveAllListeners();
            unitMenu.onValueChanged.RemoveAllListeners();
            selectionButton.onClick.RemoveAllListeners();
            splashButton.onClick.RemoveAllListeners();
            animationButton.onClick.RemoveAllListeners();
            abilityButton.onClick.RemoveAllListeners();
        }


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
        private static List<Button> implementButtons = new List<Button>();
        public static event Action<ImplementList, int> SelectionEvent;

        private void UnsubscribeSelectionElements()
        {
            foreach (Button toDelete in implementButtons)
            {
                toDelete.onClick.RemoveAllListeners();
            }
            foreach (Transform toDelete in implementHolderTransform)
            {
                Destroy(toDelete.gameObject);
            }
            if (implementHolderTransform.TryGetComponent(out ContentSizeFitter c))
            {
                Destroy(c);
            }
        }
        private void DefualtSelectionEvent(ImplementList implementList, int index)
        {
            selectedImplementIndex = index;
            selectedImplementList = implementList;
            AnimationElement.ImplementPath = implementList.ImplementPath(index);
            ChangeWindow(3);
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
                            implementButtons.Add(unitButton);
                            unitObject.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                            unitObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().text = implementList.implements[i].name;
                            Sprite sprite = implementList.BaseSprite(i);
                            Image image = unitObject.transform.GetChild(1).GetComponent<Image>();
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
                implementButtons.Add(addButton);
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
            implements[length] = new Implement("temp", length);
            implementList.implements = implements;
            implementList = SaveSystem.SaveImplmentList(implementList);
            selectedImplementList = implementList;
            selectedImplementIndex = length;
            ChangeWindow(3);
        }


        //Splash
        public ColorSlider primaryColor;
        public ColorSlider secondaryColor;
        public Image primaryColorImage;
        public Image secondayColorImage;
        public Image splashScreenProfile;
        public InputField splashNameField;
        public InputField splashFragmentField;
        public InputField splashPowerField;
        public int splashType;
        public Button[] splashButtons;
        public InputField splashDescriptionField;

        private void EnableSplashScreen()
        {
            //set
            splashNameField.text = selectedImplementList.implements[selectedImplementIndex].name;
            splashFragmentField.text = selectedImplementList.implements[selectedImplementIndex].fragment;
            splashPowerField.text = selectedImplementList.implements[selectedImplementIndex].power;
            ChangeType(selectedImplementList.implements[selectedImplementIndex].type);
            splashDescriptionField.text = selectedImplementList.implements[selectedImplementIndex].description;
            primaryColor.Color = selectedImplementList.implements[selectedImplementIndex].PrimaryColor;
            secondaryColor.Color = selectedImplementList.implements[selectedImplementIndex].SecondaryColor;
            if (selectedImplementList.implements[selectedImplementIndex].GetBaseSprite(selectedImplementList.modPath) == null)
            {
                splashScreenProfile.color = Color.clear;
            }
            else
            {
                splashScreenProfile.color = Color.white;
            }
            splashScreenProfile.sprite = selectedImplementList.implements[selectedImplementIndex].GetBaseSprite(selectedImplementList.modPath);

            //activate
            splashObject.SetActive(true);
        }
        public void AddProfile()
        {
            Sprite texture = SaveSystem.LoadPNG(EditorUtility.OpenFilePanel("Set Profile", Application.persistentDataPath, "png"), Vector2.one, 1);
            SaveSystem.SavePNG(selectedImplementList.ImplementPath(selectedImplementIndex) + "/Base.png", texture.texture);
            splashScreenProfile.color = Color.white;
            splashScreenProfile.sprite = texture;
        }
        public void SplashPreview()
        {
            throw new NotImplementedException();
        }
        public void ChangeType(int i)
        {
            foreach (Button butt in splashButtons)
            {
                ColorBlock colorBlock = butt.colors;
                colorBlock.normalColor = Color.black;
                butt.colors = colorBlock;
            }
            ColorBlock altcolorBlock = splashButtons[i].colors;
            altcolorBlock.normalColor = Color.white;
            splashButtons[i].colors = altcolorBlock;
            splashType = i;
        }

        //Animation
        public Transform animationHolderTransform;
        public GameObject animationSelectionObject;
        public GameObject animationAddObject;
        public GameObject animationAttachmentObject;
        public static List<AnimationElement> AnimationElements => animationList;
        private static List<AnimationElement> animationList = new List<AnimationElement>();
        private static List<Attachment> attachments = new List<Attachment>();

        private void PopulateAnimation()
        {
            UnsubsubscribeAnimation();
            if (selectedImplementList != null && selectedImplementList.implements[selectedImplementIndex].animations != null)
            {
                IAnimation[] animations = selectedImplementList.implements[selectedImplementIndex].animations;

                for (int i = 0; i < animations.Length; i++)
                {
                    AnimationElement animationElement = Instantiate(animationSelectionObject, animationHolderTransform).GetComponent<AnimationElement>();
                    animationList.Add(animationElement);
                    animationElement.index = i;
                    animationElement.DestroyEvent += DestroyAnimation;
                    animationElement.Initalize((animations.Animation)animations[i]);
                    PopulateAnimationAttachments(i, animationElement);
                }
            }
            Instantiate(animationAddObject, animationHolderTransform).GetComponent<Button>().onClick.AddListener(call: AddAnimation);
            animationsObject.SetActive(true);
        }

        private void PopulateAnimationAttachments(int index, AnimationElement animation)
        {
            if (selectedImplementList.implements[selectedImplementIndex].idelIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Idel", index);
            }
            if (selectedImplementList.implements[selectedImplementIndex].attackIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Attack", index);
            }
            if (selectedImplementList.implements[selectedImplementIndex].fidgetIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Fidget", index);
            }
            if (selectedImplementList.implements[selectedImplementIndex].walkIndex == index)
            {
                InstantiateAnimationAttachment(animation, "Walk", index).Directional = true;
            }
            if (selectedUnit != null && false)
            {
                foreach (KeyValuePair<Ability, int> AKey in (selectedUnit.battle as JRPGBattle).abilites)
                {
                    if (AKey.Value == index)
                    {
                        InstantiateAnimationAttachment(animation, AKey.Key.name, index);
                        return;
                    }
                }
            }
        }

        private Attachment InstantiateAnimationAttachment(AnimationElement animation, string name, int index)
        {
            GameObject attachmentObject = Instantiate(animationAttachmentObject, animation.attachmentHolderTransform);
            Attachment attachment = attachmentObject.GetComponent<Attachment>();
            attachment.Name = name;
            attachment.index = index;
            attachments.Add(attachment);
            return attachment;
        }

        private void AddAnimation()
        {
            DisableAllWindows();
            IAnimation[] selectedAnimations = selectedImplementList.implements[selectedImplementIndex].animations;
            int length = selectedAnimations.Length;
            IAnimation[] animations = new IAnimation[length + 1];
            selectedAnimations.CopyTo(animations, 0);
            animations[length] = new animations.Animation(SaveSystem.LoadPNG(Vector2.one / 2f), length, selectedImplementList.ImplementPath(selectedImplementIndex));
            selectedImplementList.implements[selectedImplementIndex].animations = animations;
            PopulateAnimation();
        }
        private void DestroyAnimation(int index)
        {
            animationList.Remove(animationList[index]);
            ChangeWindow(2);
        }
        private IAnimation[] SaveAnimations(string implmentPath)
        {
            List<IAnimation> animations = new List<IAnimation>();
            foreach (AnimationElement a in animationList)
            {
                animations.Add(a.CurrentSpritesAsAnimation());
            }
            return animations.ToArray();
        }
        private void UnsubsubscribeAnimation()
        {
            animationList.Clear();
            attachments.Clear();
            foreach (Transform t in animationHolderTransform)
            {
                if (TryGetComponent(out AnimationElement a))
                {
                    a.DeInitalize();
                }
                else if (TryGetComponent(out Button b))
                {
                    b.onClick.RemoveAllListeners();
                }

                Destroy(t.gameObject);
            }
        }
        private Implement SetAnimationIdexes(Implement implement)
        {
            foreach(Attachment attachment in attachments)
            {
                if (attachment.Name == "Idel")
                {
                    implement.idelIndex = attachment.index;
                }
                else if (attachment.Name == "Attack")
                {
                    implement.attackIndex = attachment.index;
                }
                else if (attachment.Name == "Fidget")
                {
                    implement.fidgetIndex = attachment.index;
                }
                else if (attachment.Name == "Walk")
                {
                    implement.walkIndex = attachment.index;
                }
                else if (selectedUnit != null && false)
                {
                    Dictionary<Ability, int> abilites = (selectedUnit.battle as JRPGBattle).abilites;
                    foreach (KeyValuePair<Ability, int> AKey in abilites)
                    {
                        if (attachment.Name == AKey.Key.name)
                        {
                            abilites[AKey.Key] = attachment.index;
                            break;
                        }
                    }
                }

            }
            return implement;
        }

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
            DisableAllWindows();
            UnsubscribeTabToolbarElements();
        }

        private void Update()
        {
            if (windowObject.activeInHierarchy)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) 
                {
                    Save();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
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


    }

}