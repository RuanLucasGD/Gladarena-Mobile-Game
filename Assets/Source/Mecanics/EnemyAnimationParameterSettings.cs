using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [CreateAssetMenu(fileName = "Enemy Animation Settings", menuName = "Characters Settings/Enemy Animation Settings")]
    public class EnemyAnimationParameterSettings : ScriptableObject
    {
        public string IsWalkingParameter;
        public string IsAttackingParameter;
        public string IsDeathParameter;
        public string WeaponIdParameter;

        public EnemyAnimationParameterSettings()
        {
            IsWalkingParameter = "Is Walking";
            IsAttackingParameter = "Is Attacking";
            IsDeathParameter = "Is Death";
            WeaponIdParameter = "Weapon ID";
        }
    }
}