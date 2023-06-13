using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Mecanics;

namespace Game.Effects
{
    public class PlayerSfx : MonoBehaviour
    {
        public AudioSource Source;
        public AudioClip WeaponAttackClip;
        public PlayerCharacter Character;

        void Start()
        {
            Character.Weapon.OnStartAttack.AddListener(PlayAttackSound);
        }

        private void Update()
        {
            // works only on original player
            if (transform.tag != "Player")
            {
                Character.Weapon.OnStartAttack.RemoveListener(PlayAttackSound);
                Destroy(this);
            }
        }

        private void PlayAttackSound()
        {
            Source.PlayOneShot(WeaponAttackClip);
        }
    }
}


