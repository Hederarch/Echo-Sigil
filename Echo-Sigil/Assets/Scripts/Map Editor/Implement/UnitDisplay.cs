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
            if (implement != null)
            {
                modText.text = SaveSystem.GetModPaths()[implement.modPathIndex].modName;
                implmentText.text = implement.splashInfo.name;
                Sprite baseSprite = implement.baseSprite;
                baseImage.sprite = baseSprite;
                baseImage.color = baseSprite == null ? Color.clear : Color.white;
                unitPosText.text = unit != null ? MapReader.WorldToGridSpace(unit.transform.position).ToString() : "";
                index.text = implement.index.ToString();
            }
        }
        public void Saved(bool saved)
        {
            if (saved && savedIcon.gameObject.activeInHierarchy)
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
