using MapEditor.Windows;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class ImplementEditor : MonoBehaviour
    {
        private int selectedImplementIndex;
        private ImplementList selectedImplementList;
        public Unit selectedUnit = null;
        public UnitDisplay unitDisplay;
        public RectTransform contentArea;

        public GameObject[] windowObjects;
        public GameObject selectWindowObject;
        private Window curWindow;

        public void ChangeWindow(int arg0)
        {
            Save();
            DisableAllWindows();
            gameObject.SetActive(true);
            SelectWindow.SelectionEvent += DefualtSelectionEvent;

            if (selectedImplementList != null && selectedImplementList.Implements.Length > selectedImplementIndex)
            {
                unitDisplay.DisplayUnit(selectedImplementList, selectedImplementIndex);
                Implement implement = selectedImplementList[selectedImplementIndex];
                curWindow = Instantiate(windowObjects[arg0], contentArea).GetComponent<Window>();
                curWindow.Initalize(implement, selectedUnit);
            }
            else
            {
                SelectWindow selectWindow = Instantiate(selectWindowObject, contentArea).GetComponent<SelectWindow>();
                curWindow = selectWindow;
                selectWindow.Initalize();
            }
        }

        public void Save()
        {
            if (selectedImplementList != null && selectedImplementList.Implements.Length > selectedImplementIndex)
            {
                Implement implement = selectedImplementList.Implements[selectedImplementIndex];
                implement = curWindow.Save(implement);
                selectedImplementList.Implements[selectedImplementIndex] = implement;
                unitDisplay.DisplayUnit(implement, selectedUnit);
                SaveSystem.SaveImplmentList(selectedImplementList);
                unitDisplay.Saved();
            }
        }

        private void DisableAllWindows()
        {
            foreach(Transform transform in contentArea)
            {
                Destroy(transform.gameObject);
            }
        }

        public void CloseWindow()
        {
            Save();
            DisableAllWindows();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            SelectWindow.SelectionEvent -= DefualtSelectionEvent;
            DisableAllWindows();
        }

        public void DefualtSelectionEvent(ImplementList implementList, int index)
        {
            selectedImplementIndex = index;
            selectedImplementList = implementList;
            unitDisplay.DisplayUnit(implementList, index);
            ChangeWindow(1);
        }
    }

}