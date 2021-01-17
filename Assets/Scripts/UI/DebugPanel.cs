using Gjrfytn.Dim.Object;
using Sof.Object;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class DebugPanel : SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private GameManager _GameManager;
        [SerializeField]
        private UIManager _UIManager;
        [SerializeField]
        private Map _Map;
        [SerializeField]
        private Button _SpawnUnitButton;
        [SerializeField]
        private Dropdown _UnitFactionDropdown; // TODO Update list
        [SerializeField]
        private Button _ResetMapButton;
#pragma warning restore 0649

        private Model.Faction _SelectedFaction;

        private void Start()
        {
            _UnitFactionDropdown.onValueChanged.AddListener((index) => _SelectedFaction = _GameManager.Game.Factions.Single(f => f.Name == _UnitFactionDropdown.options[index].text));
            _SpawnUnitButton.onClick.AddListener(() => _UIManager.DebugCreateUnit(_SelectedFaction));

            _UnitFactionDropdown.AddOptions(_GameManager.Game.Factions.Select(f => f.Name).ToList());
            _SelectedFaction = _GameManager.Game.Factions.First();

            _ResetMapButton.onClick.AddListener(() => _Map.ModelMap.ApplyScenario(_GameManager.CurrentScenario));
        }
    }
}
