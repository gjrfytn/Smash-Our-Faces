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

        public void Setup(Unit unit)
        {
            _NameText.text = unit.name;
            _HealthText.text = $"{unit.ModelUnit.Health}/{unit.Health}";
            _MovePointsText.text = $"{unit.ModelUnit.MovePoints}/{unit.Speed}";
        }
    }
}
