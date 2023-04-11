using UnityEngine;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class WeaponSelector : MonoBehaviour
    {
        public Weapon[] Weapons;
        public UnityEvent OnSelectWeapon;

        private void Awake()
        {
            OnSelectWeapon.AddListener(HideScreen);

            if (MainMenuManager.Instance)
            {
                HideScreen();
                MainMenuManager.Instance.OnHideMainMenu.AddListener(ShowWhenExitMainMenu);
            }
        }

        public void SetWeapon(int index)
        {
            if (index > Weapons.Length)
            {
                Debug.LogError($"Not exist weapon with index {index}");
                return;
            }

            var _player = GameManager.Instance.Player;

            if (!_player)
            {
                Debug.LogError("Don't exist player on this scene");
                return;
            }

            var _weapon = Instantiate(Weapons[index].gameObject).GetComponent<Weapon>();
            _player.SetWeapon(_weapon);

            OnSelectWeapon.Invoke();
        }

        private void ShowScreen()
        {
            gameObject.SetActive(true);
        }

        private void HideScreen()
        {
            gameObject.SetActive(false);
        }

        private void ShowWhenExitMainMenu()
        {
            ShowScreen();
        }
    }
}
