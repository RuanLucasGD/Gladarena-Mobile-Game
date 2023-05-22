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

        public GameProgressionManager ProgressionManager;
        public Widget StartHorder;
        public Widget CompleteHorder;
        public Widget StartLevel;
        public Widget CompleteLevel;
        public Widget GameWin;
        public Widget GameOver;

        private void Start()
        {
            DisableAll();

            if (ProgressionManager)
            {
                ProgressionManager.OnChangeLevel.AddListener(ShowCompletedLevelWidget);

                ShowStartLevelWidget(1);

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
            Delay(widget.LifeTime, () => DisableWidget(widget));
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

        private void ShowStartLevelWidget(int newLevel)
        {
            ShowWidget(StartLevel, $"LEVEL {newLevel} STARTED!");
        }

        private void ShowCompletedLevelWidget(int completedLevel)
        {
            ShowWidget(CompleteLevel, $"LEVEL {completedLevel} COMPLETED!");
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

        private void Delay(float delay, UnityAction onCompleted)
        {
            IEnumerator _Delay()
            {
                yield return new WaitForSeconds(delay);
                onCompleted();
            }

            StartCoroutine(_Delay());
        }
    }
}