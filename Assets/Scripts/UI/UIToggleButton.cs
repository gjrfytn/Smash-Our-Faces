using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    [RequireComponent(typeof(Button))]
    public class UIToggleButton : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameObject _ObjectToToggle;
#pragma warning restore 0649

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => _ObjectToToggle.SetActive(!_ObjectToToggle.activeSelf));
        }
    }
}