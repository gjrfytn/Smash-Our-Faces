﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sof.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
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