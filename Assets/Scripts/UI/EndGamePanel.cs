using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sof.UI
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField]
        private Text _WinnerText;

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
