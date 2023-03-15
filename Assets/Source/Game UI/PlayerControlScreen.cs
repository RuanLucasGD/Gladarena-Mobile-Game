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
            ArenaManager.Instance.OnStartLevel.AddListener(l => EnableControl());
            ArenaManager.Instance.OnCompleteLevel.AddListener(l => DisableControl());

            DisableControl();
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