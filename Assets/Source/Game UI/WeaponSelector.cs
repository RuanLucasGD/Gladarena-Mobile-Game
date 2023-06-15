using UnityEngine;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class WeaponSelector : MonoBehaviour
    {
        public WeaponSelectionButton ButtonPrefab;
        public Transform WeaponButtonsParent;

        public Weapon[] Weapons;
        public UnityEvent OnSelectWeapon;

        private void Start()
        {
            ShowScreen();
        }

        private void OnEnable()
        {
            SetupWeaponsList();
        }

        public void SetWeapon(Weapon weapon)
        {
            var _player = GameManager.Instance.Player;

            if (!_player)
            {
                Debug.LogError("Don't exist player on this scene");
                return;
            }

            var _weapon = Instantiate(weapon);
            _player.SetWeapon(_weapon);
            OnSelectWeapon.Invoke();
            HideScreen();
        }

        private void SetupWeaponsList()
        {
            OnSelectWeapon.AddListener(UnsetupWeaponList);

            for (int i = 0; i < Weapons.Length; i++)
            {
                var _newWeaponButton = Instantiate(ButtonPrefab, WeaponButtonsParent);
                _newWeaponButton.SetupButton(Weapons[i], this);
                _newWeaponButton.OnSetWeapon.AddListener(() => OnSelectWeapon.RemoveListener(UnsetupWeaponList));
            }
        }

        private void UnsetupWeaponList()
        {
            foreach (var b in FindObjectsOfType<WeaponSelectionButton>(true))
            {
                Destroy(b.gameObject);
            }
        }

        public void ShowScreen()
        {
            gameObject.SetActive(true);
            GameManager.Instance.GamePaused = true;
        }

        public void HideScreen()
        {
            gameObject.SetActive(false);
            GameManager.Instance.GamePaused = false;
        }
    }
}
