using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainScreen : MonoBehaviour
    {
        public MainMenuManager MainMenu;
        [Space]
        public Button StartButton;
        public Button QuitButton;

        void Start()
        {
            if (StartButton) StartButton.onClick.AddListener(StartGame);
            if (QuitButton) QuitButton.onClick.AddListener(QuitGame);
        }

        public void StartGame()
        {
            MainMenu.StartGame();
        }

        public void QuitGame()
        {
            MainMenu.ExitGame();
        }
    }
}