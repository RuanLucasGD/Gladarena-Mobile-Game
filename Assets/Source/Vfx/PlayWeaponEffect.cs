using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Mecanics;

namespace Game.Mecanics
{
    public class PlayWeaponEffect : MonoBehaviour
    {
        public ParticleSystem AttackEffect;
        public MelleWeapon Weapon;
        public PlayerCharacter _player;

        void Awake()
        {
            Weapon.OnSetOwner.AddListener(Setup);
            AttackEffect.Stop();
        }

        private void Update()
        {
            transform.up = Weapon.transform.up;
        }

        private void Setup(PlayerCharacter player)
        {
            // unsetup old player
            if (_player)
            {
                _player.Weapon.OnStartAttack.RemoveListener(Play);
            }

            _player = player;

            if (_player)
            {
                _player.Weapon.OnStartAttack.AddListener(Play);
            }
        }

        private void Play()
        {
            AttackEffect.Play();
        }
    }
}