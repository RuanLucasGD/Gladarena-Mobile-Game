using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class EnemySoldier : Character
    {
        public PlayerCharacter Player => GameManager.Instance.Player;
        
        public bool IsFollowingPlayer
        {
            get
            {
                var _playerDistance = Vector3.Distance(Player.transform.position, Weapon.WeaponObject.transform.position);
                return _playerDistance > Weapon.WeaponObject.AttackRange;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!Player || !HasWeapon)
            {
                CharacterMoveDirection = Vector3.zero;

                if (!HasWeapon)
                {
                    Debug.LogError("The soldier enemy doesn't have weapon!");
                }
                return;
            }

            UpdateCharacterState();
        }

        private void UpdateCharacterState()
        {
            if (IsFollowingPlayer)
            {
                FollowPlayer();

            }
            else
            {
                AttackPlayer();
            }
        }

        private void FollowPlayer()
        {
            var _directionToPlayer = (Player.transform.position - transform.position).normalized;
            CharacterMoveDirection = _directionToPlayer;
        }

        private void AttackPlayer()
        {
            LookAtDirection = (Player.transform.position - transform.position).normalized;
            Weapon.WeaponObject.Attack(Player);
            CharacterMoveDirection = Vector3.zero;
        }
    }
}


