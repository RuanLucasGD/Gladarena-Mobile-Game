using UnityEngine;
using Game.Mecanics;

namespace Game.UI
{
    public class PlayerControlScreen : MonoBehaviour
    {
        private void Awake()
        {
            ArenaManager.Instance.OnStartLevel.AddListener((l) => EnableControl());
            ArenaManager.Instance.OnCompleteLevel.AddListener((l) => DisableControl());

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