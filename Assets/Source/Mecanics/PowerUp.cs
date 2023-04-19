using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class PowerUp : MonoBehaviour
    {
        public virtual PlayerCharacter Owner { get; set; }

        public virtual void UsePowerUp() { }
        public virtual void OnRemove() { }
    }
}