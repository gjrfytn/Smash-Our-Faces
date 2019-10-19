using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    [RequireComponent(typeof(Button))]
    public class UIToggleButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject _ObjectToToggle;

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => _ObjectToToggle.SetActive(!_ObjectToToggle.activeSelf));
        }
    }
}