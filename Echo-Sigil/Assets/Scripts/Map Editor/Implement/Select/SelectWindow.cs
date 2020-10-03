using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Windows
{
    public class SelectWindow : Window
    {
        public Transform implementHolderTransform;

        //prefabs
        public GameObject implementModHolderObject;
        public GameObject implementSelectionObject;
        public GameObject newImplementSelectionObject;

        private static List<Button> implementButtons = new List<Button>();
        public static event Action<Implement,int> SelectionEvent;

        private void UnsubscribeSelectionElements()
        {
            foreach (Transform toDelete in implementHolderTransform)
            {
                Destroy(toDelete.gameObject);
            }
        }

        public override void Initalize(Implement implement, Unit unit = null) => Initalize();

        public void Initalize()
        {
            ModPath[] modPaths = SaveSystem.GetModPaths(true);

            UnsubscribeSelectionElements();
            for (int i = 0; i < modPaths.Length; i++)
            {
                ModPath modPath = modPaths[i];
                int index = i;

                Transform modHolder = Instantiate(implementModHolderObject, implementHolderTransform).transform;
                modHolder.GetChild(0).GetChild(0).GetComponent<Text>().text = modPath.modName;

                Implement[] implements = SaveSystem.LoadImplements(i);

                for (int implemntIndex = 0; implemntIndex < implements.Length; implemntIndex++)
                {
                    Implement implement = implements[implemntIndex];
                    GameObject unitObject = Instantiate(implementSelectionObject, modHolder);
                    Button unitButton = unitObject.GetComponent<Button>();
                    unitButton.onClick.AddListener(delegate { SelectionEvent?.Invoke(implement,index); });
                    implementButtons.Add(unitButton);
                    unitObject.transform.GetChild(0).GetComponent<Text>().text = implemntIndex.ToString();
                    unitObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().text = implement.splashInfo.name;
                    Sprite sprite = implement.baseSprite;
                    Image image = unitObject.transform.GetChild(1).GetComponent<Image>();
                    image.color = sprite == null ? Color.clear : Color.white;
                    image.sprite = sprite;
                }

                Button addButton = Instantiate(newImplementSelectionObject, modHolder).GetComponent<Button>();
                
                addButton.onClick.AddListener(delegate { CreateNewImplement(index,implements.Length); });
                implementButtons.Add(addButton);
            }
        }
        private void CreateNewImplement(int modPathIndex, int index)
        {
            SelectionEvent?.Invoke(new Implement(modPathIndex, index),modPathIndex);
        }

    }
}
