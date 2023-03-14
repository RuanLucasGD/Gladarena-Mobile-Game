using UnityEngine;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class WeaponSelector : MonoBehaviour
    {
        public Weapon[] Weapons;
        public UnityEvent OnSelectWeapon;

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

            ArenaManager.Instance.StartGame();
            gameObject.SetActive(false);

            OnSelectWeapon.Invoke();
        }
    }
}
