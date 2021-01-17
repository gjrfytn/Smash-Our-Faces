using Gjrfytn.Dim.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sof.UI.MainMenu
{
    public class MainMenu : SofMonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}