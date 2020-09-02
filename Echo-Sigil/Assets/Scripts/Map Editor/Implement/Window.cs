using System;
using UnityEngine;

namespace MapEditor.Windows
{
    public class Window : MonoBehaviour
    {
        public virtual void Initalize(Implement implement, Unit unit = null)
        {
            throw new NotImplementedException("Called Window.Initalize.Base");
        }
        public virtual Implement Save(Implement implement, Unit unit = null)
        {
            return implement;
        }
    } 
}
