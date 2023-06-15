using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    /// <summary>
    /// Mostra informações ao jogador sobre a progressão de level, como "Level Completed/ Next level/ Game Over"
    /// </summary>
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
        public Widget GameOver;

        private void Start()
        {
            DisableAll();

            if (ProgressionManager)
            {
                // a cada nivel que começar, mostre a informação na tela
                ProgressionManager.OnStartLevel.AddListener(ShowCompletedLevelWidget);

                // mostre a informação que o jogador morreu quando morrer
                GameManager.Instance.OnGameOver.AddListener(ShowGameOverWidget);
                ShowStartLevelWidget(1);
            }
        }

        /// <summary>
        /// Mostra um widget de informação na tela do jogador
        /// </summary>
        /// <param name="widget">O modelo de widget que quer mostrar</param>
        /// <param name="message">A mensagem do widget</param>
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

            // habilitando a informação
            widget.MainObject.SetActive(true);

            // define a mensagem que o widget deve mostrar
            widget.Text.text = message;

            // desabilita o widget apos algum tempo
            Delay(widget.LifeTime, () => DisableWidget(widget));
        }

        // Desabilita algum widget
        private void DisableWidget(Widget widget)
        {
            widget.MainObject.SetActive(false);
        }

        /// <summary>
        /// Desabilita todos os widgets que estão na tela
        /// </summary>
        public void DisableAll()
        {
            DisableWidget(StartHorder);
            DisableWidget(CompleteHorder);
            DisableWidget(StartLevel);
            DisableWidget(CompleteLevel);
            DisableWidget(GameOver);
        }

        /// <summary>
        /// Mostrar widget com informação de "nivel iniciado".
        /// </summary>
        /// <param name="newLevel">Qual nivel foi iniciado</param>
        private void ShowStartLevelWidget(int newLevel)
        {
            ShowWidget(StartLevel, $"LEVEL {newLevel} STARTED!");
        }

        /// <summary>
        /// Mostrar informação de "nivel completado"
        /// </summary>
        /// <param name="completedLevel">Qual nivel foi completado</param>
        private void ShowCompletedLevelWidget(int completedLevel)
        {
            ShowWidget(CompleteLevel, $"LEVEL {completedLevel} COMPLETED!");
            DisableWidget(CompleteHorder);
        }

        /// <summary>
        /// Mostra informação que o jogador morreu
        /// </summary>
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