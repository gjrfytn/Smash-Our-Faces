using Sof.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class UnitPurchasePanel : Auxiliary.SofSceneMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Dropdown _UnitListDropdown;
        [SerializeField]
        private Button _SpawnUnitButton;
#pragma warning restore 0649

        private Unit[] _AvailableUnits;
        private System.Action<Unit> _UnitSelectAction;
        private Unit _SelectedUnit;

        public void Setup(IEnumerable<Unit> availableUnits, System.Action<Unit> unitSelectAction)
        {
            if (availableUnits == null)
                throw new System.ArgumentNullException(nameof(availableUnits));

            _AvailableUnits = availableUnits.ToArray();
            _UnitSelectAction = unitSelectAction ?? throw new System.ArgumentNullException(nameof(unitSelectAction));

            _UnitListDropdown.ClearOptions();
            _UnitListDropdown.AddOptions(_AvailableUnits.Select(u => u.name).ToList());
            _SelectedUnit = _AvailableUnits[0];
        }

        private void Start()
        {
            _UnitListDropdown.onValueChanged.AddListener(OnUnitListDropdownValueChange);
            _SpawnUnitButton.onClick.AddListener(OnSpawnUnitButtonClick);
        }

        private void OnUnitListDropdownValueChange(int index) => _SelectedUnit = _AvailableUnits[index];
        private void OnSpawnUnitButtonClick() => _UnitSelectAction(_SelectedUnit);
    }
}
