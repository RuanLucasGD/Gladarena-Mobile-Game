using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.Mecanics;

namespace Game.UI
{
    public class PauseScreen : MonoBehaviour
    {
        public Button PauseButton;
        public Button ContinueButton;
        public Button ExitGameButton;

        [Header("UI screens")]
        public GameObject GameplayScreen;
        public GameObject PausedGameScreen;

        [Tooltip("UI elemenets that show game progression. Example: horder completed / level started / level completed etc. This widgets needs to be deactived when game is paused")]
        public GameObject GameProgressionWidgets;

        [Space]
        public string MainMenuScene;

        void Start()
        {
            if (PauseButton) PauseButton.onClick.AddListener(PauseGame);
            if (ContinueButton) ContinueButton.onClick.AddListener(ContinueGame);
            if (ExitGameButton) ExitGameButton.onClick.AddListener(ExitGame);

            GameManager.Instance.Player.OnDeath.AddListener(FinishGame);

            if (ArenaManager.Instance)
            {
                if (GameplayScreen) GameplayScreen.SetActive(false);

                ArenaManager.Instance.OnStartGame.AddListener(StartGame);
                ArenaManager.Instance.OnCompleteLevel.AddListener(CompleteLevel);
                ArenaManager.Instance.OnStartLevel.AddListener(StartLevel);
                ArenaManager.Instance.OnGameWin.AddListener(FinishGame);
            }
            else
            {
                if (GameplayScreen) GameplayScreen.SetActive(true);
            }

            if (PausedGameScreen) PausedGameScreen.SetActive(false);
        }

        private void Update()
        {
            // when player press back of mobile navigation buttons pause the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            GameManager.Instance.GamePaused = true;

            if (GameplayScreen) GameplayScreen.SetActive(false);
            if (PausedGameScreen) PausedGameScreen.SetActive(true);
            if (GameProgressionWidgets) GameProgressionWidgets.SetActive(false);
        }

        private void ContinueGame()
        {
            GameManager.Instance.GamePaused = false;

            if (GameplayScreen) GameplayScreen.SetActive(true);
            if (PausedGameScreen) PausedGameScreen.SetActive(false);
            if (GameProgressionWidgets) GameProgressionWidgets.SetActive(true);
        }

        private void ExitGame()
        {
            GameManager.Instance.GamePaused = false;

            SceneManager.LoadScene(MainMenuScene);
        }

        private void StartGame()
        {
            if (GameplayScreen) GameplayScreen.SetActive(true);
        }

        private void CompleteLevel(int level)
        {
            if (GameplayScreen) GameplayScreen.SetActive(false);
        }

        private void StartLevel(int level)
        {
            if (GameplayScreen) GameplayScreen.SetActive(true);
        }

        private void FinishGame()
        {
            if (GameplayScreen) GameplayScreen.SetActive(false);
        }
    }
}