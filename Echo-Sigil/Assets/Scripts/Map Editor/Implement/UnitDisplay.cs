using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor.Windows
{
    public class UnitDisplay : MonoBehaviour
    {
        public float secondsOfSave = .5f;
        public Text modText;
        public Text implmentText;
        public Image baseImage;
        public Text unitPosText;
        public Text index;
        public Image savedIcon;

        public void DisplayUnit(Implement implement, Unit unit = null)
        {
            modText.text = implement.implementList.modName;
            implmentText.text = implement.name;
            Sprite baseSprite = implement.BaseSprite;
            baseImage.sprite = baseSprite;
            baseImage.color = baseSprite == null ? Color.clear : Color.white;
            unitPosText.text = unit != null ? MapReader.WorldToGridSpace(unit.transform.position).ToString() : "" ;
            index.text = implement.index.ToString();
        }
        public void DisplayUnit(ImplementList implementList, int index, Unit unit = null)
        {
            if(implementList.implements != null && implementList.implements.Length > index && index >= 0)
            {
                DisplayUnit(implementList[index], unit);
            }
        }
        public void Saved()
        {
            if(savedIcon.gameObject.activeInHierarchy)
            {
                StartCoroutine(SavedTween());
            }
        }
        private IEnumerator SavedTween()
        {
            savedIcon.enabled = true;
            yield return new WaitForSeconds(secondsOfSave);
            savedIcon.enabled = false;
        }
    } 
}
