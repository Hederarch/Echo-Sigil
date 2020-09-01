using UnityEngine;

namespace MapEditor.Windows
{
    public class Window : MonoBehaviour
    {
        public virtual bool Active { get => gameObject.activeInHierarchy; set => gameObject.SetActive(value); }
        public virtual void Initalize(Implement implement)
        {

        }
        public virtual Implement Save(Implement implement)
        {
            return implement;
        }
    } 
}
