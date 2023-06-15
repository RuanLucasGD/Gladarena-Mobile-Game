using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    /// <summary>
    /// Weapon selection button with weapon information
    /// </summary>
    public class WeaponSelectionButton : MonoBehaviour
    {
        public MelleWeapon Weapon;
        public Button Button;
        public Text WeaponNameText;

        public UnityEvent OnSetWeapon;

        /// <summary>
        /// Called by weapon selector to configure weapon button
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="selector"></param>
        public void SetupButton(MelleWeapon weapon, WeaponSelector selector)
        {
            Weapon = weapon;
            WeaponNameText.text = Weapon.name;
            Button.onClick.RemoveAllListeners();

            Button.onClick.AddListener(() =>
            {
                selector.SetWeapon(Weapon);
                OnSetWeapon.Invoke();
            });
        }
    }
}