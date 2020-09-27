using MapEditor.Windows;
using UnityEngine;

namespace MapEditor
{
    public class ImplementEditor : MonoBehaviour
    {
        public static int selectedImplementIndex;
        public static ImplementList selectedImplementList;
        public static Implement selectedImplement { get => selectedImplementList[selectedImplementIndex]; set => selectedImplementList[selectedImplementIndex] = value; }
        public static Unit selectedUnit = null;
        [SerializeField] private UnitDisplay unitDisplay;
        [SerializeField] private RectTransform contentArea;

        [SerializeField] private GameObject[] windowObjects;
        [SerializeField] private GameObject selectWindowObject;
        public static Window curWindow;

        public void ChangeWindow(int arg0)
        {
            Save();
            DisableAllWindows();
            gameObject.SetActive(true);
            SelectWindow.SelectionEvent += DefualtSelectionEvent;

            if (selectedImplementList != null && selectedImplementList.Implements.Length > selectedImplementIndex)
            {
                unitDisplay.DisplayUnit(selectedImplementList, selectedImplementIndex);
                Implement implement = selectedImplement;
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
                Implement implement = selectedImplement;
                if (curWindow != null)
                {
                    implement = curWindow.Save(implement);
                }
                selectedImplement = implement;
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
            SelectWindow.SelectionEvent -= DefualtSelectionEvent;
            selectedImplementIndex = index;
            selectedImplementList = implementList;
            unitDisplay.DisplayUnit(implementList, index);
            ChangeWindow(1);
        }
    }

}