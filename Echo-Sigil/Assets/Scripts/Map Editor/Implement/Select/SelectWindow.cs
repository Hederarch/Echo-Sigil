using System;
using System.Collections;
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
        public static event Action<ImplementList, int> SelectionEvent;

        private void UnsubscribeSelectionElements()
        {
            foreach (Transform toDelete in implementHolderTransform)
            {
                Destroy(toDelete.gameObject);
            }
        }

        public override void Initalize(Implement implement) => Initalize();

        public void Initalize()
        {
            string[] modPaths = SaveSystem.GetModPaths();

            UnsubscribeSelectionElements();
            foreach (string modPath in modPaths)
            {
                Transform modHolder = Instantiate(implementModHolderObject, implementHolderTransform).transform;
                ImplementList implementList = SaveSystem.LoadImplementList(modPath);
                modHolder.GetChild(0).GetChild(0).GetComponent<Text>().text = implementList.modName;

                if (implementList.Implements != null)
                {
                    for (int i = 0; i < implementList.implements.Length; i++)
                    {
                        if (implementList[i].index == i)
                        {
                            GameObject unitObject = Instantiate(implementSelectionObject, modHolder);
                            Button unitButton = unitObject.GetComponent<Button>();
                            int index = i;
                            unitButton.onClick.AddListener(delegate { SelectionEvent?.Invoke(implementList, index); });
                            implementButtons.Add(unitButton);
                            unitObject.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                            unitObject.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().text = implementList.Implements[i].name;
                            Sprite sprite = implementList[i].BaseSprite;
                            Image image = unitObject.transform.GetChild(1).GetComponent<Image>();
                            image.color = sprite == null ? Color.clear : Color.white;
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
            ImplementList implementList = SaveSystem.LoadImplementList(modPath);
            int length = implementList.implements.Length;
            new Implement("temp", implementList);
            implementList = SaveSystem.SaveImplmentList(implementList);
            SelectionEvent?.Invoke(implementList, length);
        }

        public override Implement Save(Implement implement)
        {
            return implement;
        }

        protected void OnDestroy()
        {
            UnsubscribeSelectionElements();
        }
    }
}
