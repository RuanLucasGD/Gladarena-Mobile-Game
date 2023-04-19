using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class CharacterKiller : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerCharacter>(out var c))
            {
                c.KillCharacter();
            }
        }
    }
}