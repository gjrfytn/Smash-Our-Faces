using Gjrfytn.Dim.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class FactionInfoPanel : SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Text _TreasuryGoldText;
#pragma warning restore 0649

        private Model.Faction _Faction;

        public void Setup(Model.Faction faction)
        {
            _Faction = faction ?? throw new System.ArgumentNullException(nameof(faction));

            _Faction.GoldChanged += Faction_GoldChanged;

            UpdateGoldText();
        }

        private void Faction_GoldChanged(int change) => UpdateGoldText();

        private void UpdateGoldText() => _TreasuryGoldText.text = _Faction.Gold.ToString();
    }
}
