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
        public Animator Animator;

        [Header("Basic")]
        public float MoveSpeed;
        public float StopDistance;
        public float MaxLife;
        public float AttackDamage;
        public float AttackInterval;
        public float AttackDistance;

        [Header("Animation")]
        public int WeaponAnimID;
        public EnemyAnimationParameterSettings AnimationSettings;

        public float CurrentLife { get; private set; }
        public bool IsOnScreen { get; private set; }
        public bool SuperAttack { get; set; }

        public bool IsDeath => CurrentLife <= 0;
        public bool IsStoped => Target ? Vector3.Distance(transform.position, Target.transform.position) < StopDistance : false;
        public bool IsTargetNearToAttack => Target ? Vector3.Distance(transform.position, Target.transform.position) < AttackDistance : false;
        public bool IsAttacking { get; private set; }

        private bool _attackDeleyedStarted;

        private void Start()
        {
            Target = FindObjectOfType<PlayerCharacter>();
            CurrentLife = MaxLife;

            if (Rb)
            {
                Rb.constraints |= RigidbodyConstraints.FreezePositionY;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationX;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }

            SetWeaponAnimation();
        }

        private void Update()
        {
            if (IsDeath)
            {
                return;
            }
            
            UpdateRotation();
            UpdateAnimations();

            // don't use OnBecameVisible or OnBecameInvisible becase it's not called when object is created
            UpdateVisibility();
        }

        private void FixedUpdate()
        {
            UpdateMovement(Time.fixedDeltaTime);
        }

        private void UpdateMovement(float delta)
        {
            if (!Rb || !Target || Target.IsDeath)
            {
                return;
            }

            var _moveDirection = GetDirectionToTarget(Target.transform);
            _moveDirection *= MoveSpeed;
            _moveDirection *= delta;

            if (IsStoped)
            {
                if (!_attackDeleyedStarted)
                {
                    StartCoroutine(StartAttackDeleyed());
                }

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

            Animator.SetBool(AnimationSettings.IsWalkingParameter, !IsStoped);
            Animator.SetBool(AnimationSettings.IsAttackingParameter, IsAttacking);
        }

        private void SetDeathAnimation()
        {
            if (!AnimationSettings)
            {
                return;
            }

            Animator.SetBool(AnimationSettings.IsDeathParameter, IsDeath);
        }

        private void SetWeaponAnimation()
        {
            if (!AnimationSettings)
            {
                return;
            }

            Animator.SetInteger(AnimationSettings.WeaponIdParameter, WeaponAnimID);
        }

        private void Attack()
        {
            if (IsAttacking || !IsTargetNearToAttack || !Target || Target.IsDeath)
            {
                return;
            }

            IsAttacking = true;
            StartCoroutine(FinalizeAttackDeleyed());
        }

        private IEnumerator StartAttackDeleyed()
        {
            _attackDeleyedStarted = true;
            yield return new WaitForSeconds(AttackInterval);

            Attack();
            _attackDeleyedStarted = false;
        }

        private IEnumerator FinalizeAttackDeleyed()
        {
            var _animLenght = SuperAttack ? AnimationSettings.SuperAttackLenght : AnimationSettings.NormalAttackLenght;
            yield return new WaitForSeconds(_animLenght);

            IsAttacking = false;
        }

        // called by character animator event
        public void AttackAnimationEvent()
        {
            if (!IsAttacking)
            {
                return;
            }

            Target.AddDamage(AttackDamage);
        }

        public void AddDamage(float damage, Vector3 force = default(Vector3))
        {
            CurrentLife -= damage;

            if (CurrentLife <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            CurrentLife = 0;
            SetDeathAnimation();
            Destroy(gameObject, 10);

            if (TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }

            Rb.useGravity = false;
            Rb.constraints |= RigidbodyConstraints.FreezePositionX;
            Rb.constraints |= RigidbodyConstraints.FreezePositionY;
            Rb.constraints |= RigidbodyConstraints.FreezePositionZ;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
    }
}