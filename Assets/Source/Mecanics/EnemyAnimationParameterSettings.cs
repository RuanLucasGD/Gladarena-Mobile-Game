using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [CreateAssetMenu(fileName = "Enemy Animation Settings", menuName = "Characters Settings/Enemy Animation Settings")]
    public class EnemyAnimationParameterSettings : ScriptableObject
    {
        [Header("Animator Parameters")]
        public string IsWalkingParameter;
        public string IsAttackingParameter;
        public string IsSuperAttack;
        public string IsDeathParameter;
        public string WeaponIdParameter;

        [Header("Anim Settings")]
        public float NormalAttackLenght;
        public float SuperAttackLenght;

        public EnemyAnimationParameterSettings()
        {
            IsWalkingParameter = "Is Walking";
            IsAttackingParameter = "Is Attacking";
            IsDeathParameter = "Is Death";
            WeaponIdParameter = "Weapon ID";
            IsSuperAttack = "Is Supper Attack";
        }
    }
}