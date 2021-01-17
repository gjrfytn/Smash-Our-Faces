using Gjrfytn.Dim.Object;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sof.UI
{
    public class EndGamePanel : SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Text _WinnerText;
#pragma warning restore 0649

        public void Setup(string winnerName)
        {
            _WinnerText.text = $"{winnerName} won!";
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
