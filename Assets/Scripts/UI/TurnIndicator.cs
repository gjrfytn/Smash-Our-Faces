﻿using Gjrfytn.Dim.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    [RequireComponent(typeof(Text))]
    public class TurnIndicator : SofMonoBehaviour
    {
        private Text _Text;

        private void Start()
        {
            _Text = GetComponent<Text>();
        }

        public void SetCurrentPlayer(Model.Faction faction)
        {
            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            _Text.text = $"{faction.Name} turn";
        }
    }
}