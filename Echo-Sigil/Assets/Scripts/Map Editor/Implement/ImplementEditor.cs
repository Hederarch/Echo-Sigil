using MapEditor.Windows;
using UnityEngine;

namespace MapEditor
{
    public class ImplementEditor : MonoBehaviour
    {
        public static Implement selectedImplement = null;
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
            
            if (selectedImplement != null)
            {
                unitDisplay.DisplayUnit(selectedImplement);
                Implement implement = selectedImplement;
                curWindow = Instantiate(windowObjects[arg0], contentArea).GetComponent<Window>();
                curWindow.Initalize(implement, selectedUnit);
            }
            else
            {
                SelectWindow.SelectionEvent += DefualtSelectionEvent;
                SelectWindow selectWindow = Instantiate(selectWindowObject, contentArea).GetComponent<SelectWindow>();
                curWindow = selectWindow;
                selectWindow.Initalize();
            }
        }

        public void Save()
        {
            if (selectedImplement != null)
            {
                Implement implement = selectedImplement;
                if (curWindow != null)
                {
                    implement = curWindow.Save(implement);
                }
                selectedImplement = implement;
                unitDisplay.DisplayUnit(implement, selectedUnit);
                SaveSystem.SaveImplement(selectedImplement.modPathIndex,selectedImplement);
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

        public void DefualtSelectionEvent(Implement implement, int modPathIndex)
        {
            SelectWindow.SelectionEvent -= DefualtSelectionEvent;
            unitDisplay.DisplayUnit(implement);
            selectedImplement = implement;
            implement.modPathIndex = modPathIndex;
            ChangeWindow(1);
        }
    }

}