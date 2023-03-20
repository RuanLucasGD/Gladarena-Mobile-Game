using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

namespace Game.Mecanics
{
    public class EnemySoldier : Character
    {
        public bool UseNavMeshAI;

        public PlayerCharacter Player => GameManager.Instance.Player;
        public bool IsFollowingPlayer => Vector3.Distance(transform.position, Player.transform.position) > Movimentation.StopDistance;

        private bool OnScreen => CameraUtils.IsSpawnPointOnView(transform.position, Camera.main);

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
                Attack(Player);
            }
        }

        private void FollowPlayer()
        {
            var _directionToPlayer = (Player.transform.position - transform.position).normalized;

            // the soldider needs to use navmesh when is out screen because the character can fall 
            // in holes or take obstacles before found the target
            if (UseNavMeshAI || !OnScreen)
            {
                _directionToPlayer = GetAiPathDirection(Player.transform.position);
            }

            _directionToPlayer /= _directionToPlayer.magnitude;
            CharacterMoveDirection = _directionToPlayer;
        }

        public override void Attack(Character target = null)
        {
            LookAtDirection = (Player.transform.position - transform.position).normalized;
            CharacterMoveDirection = Vector3.zero;
            base.Attack(target);
        }
    }
}


