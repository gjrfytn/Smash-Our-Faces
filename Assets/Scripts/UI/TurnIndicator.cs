using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    [RequireComponent(typeof(Text))]
    public class TurnIndicator : MonoBehaviour
    {
        private Text _Text;

        private void Start()
        {
            _Text = GetComponent<Text>();
        }

        public void SetCurrentPlayer(int id)
        {
            _Text.text = $"Player {id} turn";
        }
    }
}