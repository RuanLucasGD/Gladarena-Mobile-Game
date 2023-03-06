using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class Weapon : MonoBehaviour
    {
        public float AttackRange;

        public Character Owner { get; set; }

        public Weapon()
        {
            AttackRange = 3;
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        protected virtual void FixedUpdate() { }

        public virtual void Attack() { }
    }
}
