using Sof.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField]
        private GameManager _GameManager;
        [SerializeField]
        private Button _SpawnUnitButton;
        [SerializeField]
        private InputField _UnitPlayerIdInputField;

        private void Start()
        {
            _SpawnUnitButton.onClick.AddListener(() => _GameManager.DebugCreateUnit(int.Parse(_UnitPlayerIdInputField.text)));
        }
    }
}
