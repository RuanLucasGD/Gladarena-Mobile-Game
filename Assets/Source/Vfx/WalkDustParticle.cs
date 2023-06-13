using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Mecanics;

namespace Game.Effects
{
    public class WalkDustParticle : MonoBehaviour
    {
        public float DustIntensity;
        public ParticleSystem DustParticle;
        
        [Space]
        public PlayerCharacter Character;

        void Start()
        {

        }

        void Update()
        {
            var _emission = DustParticle.emission;
            _emission.enabled = true;
            _emission.rateOverTimeMultiplier = Character.CharacterMoveDirection.magnitude * DustIntensity;
        }
    }
}


