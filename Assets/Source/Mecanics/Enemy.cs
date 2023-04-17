using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class Enemy : MonoBehaviour
    {
        public PlayerCharacter Target;

        [Header("Components")]
        public Rigidbody Rb;
        public Renderer ModelRenderer;

        [Header("Basic")]
        public float MoveSpeed;
        public float MaxLife;
        public float AttackDamage;
        public float AttackInterval;
        public float AttackDistance;

        [Header("Animation")]
        public int WeaponAnimID;
        public EnemyAnimationParameterSettings AnimationSettings;

        public float CurrentLife { get; private set; }
        public bool IsOnScreen { get; private set; }

        public bool IsDeath => CurrentLife <= 0;
        public bool IsFollowingTarget => !IsDeath && Target ? Vector3.Distance(transform.position, Target.transform.position) > AttackDistance : false;
        public bool IsAttacking => !IsDeath && !IsFollowingTarget;

        private void Start()
        {
            Target = FindAnyObjectByType<PlayerCharacter>();
            CurrentLife = MaxLife;

            if (Rb)
            {
                Rb.constraints |= RigidbodyConstraints.FreezePositionY;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationX;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
        }

        private void Update()
        {
            UpdateRotation();

            // don't use OnBecameVisible or OnBecameInvisible becase it's not called when object is created
            UpdateVisibility();
        }

        private void FixedUpdate()
        {
            UpdateMovement(Time.fixedDeltaTime);
        }

        private void UpdateMovement(float delta)
        {
            if (!Rb || !Target)
            {
                return;
            }

            var _moveDirection = GetDirectionToTarget(Target.transform);
            _moveDirection *= MoveSpeed;
            _moveDirection *= delta;

            if (!IsFollowingTarget)
            {
                _moveDirection = Vector3.zero;
            }

            Rb.MovePosition(Rb.position + _moveDirection);
        }

        private void UpdateRotation()
        {
            if (IsDeath || !IsOnScreen)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(GetDirectionToTarget(Target.transform));
        }

        private Vector3 GetDirectionToTarget(Transform target)
        {
            return Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up).normalized;
        }

        private void UpdateVisibility()
        {
            if (!ModelRenderer)
            {
                return;
            }

            IsOnScreen = ModelRenderer.isVisible;
        }

        private void UpdateAnimations()
        {
            if (!AnimationSettings)
            {
                return;
            }

            // TODO
        }
    }
}