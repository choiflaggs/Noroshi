using UnityEngine;
using UnityEngine.UI;

namespace Noroshi.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizationUIText : MonoBehaviour
    {
        [SerializeField] string _textID;

        void Start()
        {
            var uiText = GetComponent<Text>();
            uiText.text = GlobalContainer.LocalizationManager.GetText(_textID);
        }
    }
}
