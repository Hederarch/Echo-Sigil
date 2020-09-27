using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class Toolbar : MonoBehaviour
    {
        public Dropdown view;
        public Dropdown implement;
        public ImplementEditor ImplementEditor;

        public void Start()
        {
            view.onValueChanged.AddListener(delegate { HandleVeiw(view.value); });
            implement.onValueChanged.AddListener(delegate { HandleImplent(implement.value); });
        }

        public void HandleVeiw(int arg0)
        {
            view.value = 0;
            switch (arg0)
            {
                case 1:
                    throw new NotImplementedException();
                case 2:
                    throw new NotImplementedException();
                case 3:
                    throw new NotImplementedException();
            }
        }
        public void HandleImplent(int arg0)
        {
            implement.value = 0;
            ImplementEditor.ChangeWindow(arg0);
        }

        public void OnDestroy()
        {
            view.onValueChanged.RemoveAllListeners();
            implement.onValueChanged.RemoveAllListeners();
        }
    }
}
