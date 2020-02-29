using Sof.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class UnitInfoPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Text _NameText;
        [SerializeField]
        private Text _HealthText;
        [SerializeField]
        private Text _MovePointsText;
#pragma warning restore 0649

        private Unit _Unit;

        public void Setup(Unit unit)
        {
            _Unit = unit != null ? unit : throw new System.ArgumentNullException(nameof(unit));

            _Unit.ModelUnit.HealthChanged += ModelUnit_HealthChanged;
            _Unit.ModelUnit.MovePointsChanged += ModelUnit_MovePointsChanged;

            _NameText.text = _Unit.name;
            UpdateHealthText();
            UpdateMovePoints();
        }

        private void ModelUnit_HealthChanged() => UpdateHealthText();
        private void ModelUnit_MovePointsChanged() => UpdateMovePoints();

        private void UpdateHealthText() => _HealthText.text = $"{_Unit.ModelUnit.Health}/{_Unit.Health}";
        private void UpdateMovePoints() => _MovePointsText.text = $"{_Unit.ModelUnit.MovePoints}/{_Unit.Speed}";
    }
}
