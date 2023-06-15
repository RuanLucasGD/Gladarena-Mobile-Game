using UnityEngine;
using Game.Mecanics;

namespace Game.UI
{
    /// <summary>
    /// Show power ups screen when finish level and set to next level.
    /// Before to start level, select a power up and next start the level
    /// </summary>
    public class PlayerControlScreen : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.Instance.OnSetPausedGame.AddListener((p) =>
            {
                if (p) DisableControl();
                else EnableControl();
            });
        }

        public void DisableControl()
        {
            gameObject.SetActive(false);
        }

        public void EnableControl()
        {
            gameObject.SetActive(true);
        }
    }
}