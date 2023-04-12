using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class GameMatchWidgets : MonoBehaviour
    {
        [System.Serializable]
        public class Widget
        {
            public GameObject MainObject;
            public Text Text;

            [Space]

            [Min(0)] public float LifeTime;

            public Widget()
            {
                LifeTime = 2;
            }
        }

        public Widget StartHorder;
        public Widget CompleteHorder;
        public Widget StartLevel;
        public Widget CompleteLevel;
        public Widget GameWin;
        public Widget GameOver;

        private void Start()
        {
            DisableAll();

            if (ArenaManager.Instance)
            {
                ArenaManager.Instance.OnStartHorder.AddListener(ShowStartHorderWidget);
                ArenaManager.Instance.OnCompleteHorder.AddListener(ShowCompletedHorderWidget);
                ArenaManager.Instance.OnStartLevel.AddListener(ShowStartLevelWidget);
                ArenaManager.Instance.OnCompleteLevel.AddListener(ShowCompletedLevelWidget);
                ArenaManager.Instance.OnGameWin.AddListener(ShowGameWinWidget);

                GameManager.Instance.Player.OnDeath.AddListener(ShowGameOverWidget);
                GameManager.Instance.Player.OnRevive.AddListener(DisableAll);
            }
        }

        private void ShowWidget(Widget widget, string message)
        {
            if (!widget.MainObject)
            {
                Debug.LogError($"Widget prefab not setted correctly on GameObject {name}");
                return;
            }

            if (!widget.Text)
            {
                Debug.LogError($"Widget Text not setted correctly on GameObject {name}");
                return;
            }

            // enable widget
            widget.MainObject.SetActive(true);

            // setup widget
            widget.Text.text = message;

            // disable widget after time
            GameManager.Instance.Delay(widget.LifeTime, () => DisableWidget(widget));
        }

        private void DisableWidget(Widget widget)
        {
            widget.MainObject.SetActive(false);
        }

        public void DisableAll()
        {
            DisableWidget(StartHorder);
            DisableWidget(CompleteHorder);
            DisableWidget(StartLevel);
            DisableWidget(CompleteLevel);
            DisableWidget(GameWin);
            DisableWidget(GameOver);
        }

        private void ShowStartHorderWidget(int newHorder)
        {
            ShowWidget(StartHorder, $"HORDER {++newHorder} STARTED!");
        }

        private void ShowCompletedHorderWidget(int completedHorder)
        {
            ShowWidget(CompleteHorder, $"HORDER {++completedHorder} COMPLETED!");
            DisableWidget(StartHorder);
        }

        private void ShowStartLevelWidget(int newLevel)
        {
            ShowWidget(StartLevel, $"LEVEL {++newLevel} STARTED!");
            DisableWidget(StartLevel);
        }

        private void ShowCompletedLevelWidget(int completedLevel)
        {
            ShowWidget(CompleteLevel, $"LEVEL {++completedLevel} COMPLETED!");
            DisableWidget(CompleteHorder);
        }

        private void ShowGameWinWidget()
        {
            ShowWidget(GameWin, "GAME WIN!");
        }

        private void ShowGameOverWidget()
        {
            ShowWidget(CompleteLevel, "GAME OVER!");
        }
    }
}