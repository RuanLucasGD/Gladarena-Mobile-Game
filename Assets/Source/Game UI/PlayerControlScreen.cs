using UnityEngine;
using Game.Mecanics;

namespace Game.UI
{
    public class PlayerControlScreen : MonoBehaviour
    {
        private void Awake()
        {
            ArenaLevelManager.Instance.OnStartLevel.AddListener(EnableControl);
            ArenaLevelManager.Instance.OnCompleteLevel.AddListener(DisableControl);

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