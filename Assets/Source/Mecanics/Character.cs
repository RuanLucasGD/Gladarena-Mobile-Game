using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    /// <summary>
    /// The base to make characters.
    /// Have all base behaviours of a NPC.
    /// Can move, or attack.
    /// Dont have self-control / AI
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour
    {
        [System.Serializable]
        public class Moviment
        {
            [Min(0)] public float MoveSpeed;
            [Min(0)] public float Gravity;
            [Min(0)] public float TurnSpeed;
            [Min(0)] public float ExternalForceDeceleration;
            [Min(0)] public float StopDistance;

            public Moviment()
            {
                MoveSpeed = 10;
                Gravity = 10;
                TurnSpeed = 10;
                ExternalForceDeceleration = 2;
                StopDistance = 1;
            }
        }

        [System.Serializable]
        public class WeaponSlot
        {
            public Weapon WeaponObject;
            public Transform Hand;
            [Min(0)] public float DelayToAttack;

            public WeaponSlot()
            {
                DelayToAttack = 0.5f;
            }
        }

        [System.Serializable]
        public class LifeStorage
        {
            [Min(1)] public float LifeAmount;
            [Min(0.1f)] public float AutoDestroyOnDeathDelay;
            public UnityEvent OnResetLife;

            public LifeStorage()
            {
                LifeAmount = 100;
            }
        }

        public Moviment Movimentation;
        public WeaponSlot Weapon;
        public LifeStorage Life;

        [Space]

        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnSetWeapon;

        private bool _deleyedAttackStarted;
        private Vector3 _moveDirection;
        private Vector3 _externalForce;
        private CharacterController _characterController;

        public CharacterController CharacterController => _characterController;

        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;
        public bool IsDeath => CurrentLife <= 0;
        public bool IsAttacking => IsStoped;
        public bool HasWeapon => Weapon.WeaponObject;
        public float CurrentLife { get; private set; }

        public bool CanMove { get; set; }

        /// <summary>
        /// Current player moviment velocity with gravity
        /// </summary>
        /// <value></value>
        public Vector3 CharacterVelocity { get; private set; }

        /// <summary>
        /// Set direction to character move. To stop set as Vector3(0, 0, 0)
        /// </summary>
        /// <returns></returns>
        public Vector3 CharacterMoveDirection { get => _moveDirection; set => _moveDirection = new Vector3(value.x, 0, value.z).normalized; }

        public Vector3 LookAtDirection { get; set; }

        protected virtual void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            CanMove = true;
            CurrentLife = Life.LifeAmount;
            LookAtDirection = transform.forward;

            if (!CharacterController)
            {
                Debug.LogError($"Character Controller not added on gameObject {CharacterController}");
                return;
            }
        }

        protected virtual void Start()
        {
            if (Weapon.WeaponObject)
            {
                SetWeapon(Weapon.WeaponObject);
            }
        }

        protected virtual void Update()
        {
            UpdateRotation(Time.deltaTime);
            UpdateMoviment(Time.deltaTime);
            UpdateExternalForce(Time.deltaTime);

            if (transform.position.y < -100)
            {
                KillCharacter();
            }
        }

        private void UpdateMoviment(float delta)
        {
            if (!CharacterController)
            {
                return;
            }

            if (!CanMove || !CharacterController.isGrounded)
            {
                CharacterMoveDirection = Vector3.zero;
            }

            CharacterVelocity = new Vector3(CharacterMoveDirection.x, -Movimentation.Gravity, CharacterMoveDirection.z);
            CharacterVelocity *= Movimentation.MoveSpeed;
            CharacterVelocity += _externalForce;
            CharacterVelocity *= delta;

            CharacterController.Move(CharacterVelocity);
        }

        private void UpdateRotation(float delta)
        {
            var _turnSpeed = Mathf.Clamp01(delta * Movimentation.TurnSpeed);
            var _currentRot = transform.rotation;

            if (!IsStoped)
            {
                LookAtDirection = CharacterMoveDirection / CharacterMoveDirection.magnitude;
            }

            var _targetRot = Quaternion.LookRotation(LookAtDirection);

            transform.rotation = Quaternion.Lerp(_currentRot, _targetRot, _turnSpeed);
        }

        private void UpdateExternalForce(float delta)
        {
            _externalForce -= _externalForce * Mathf.Clamp(delta * Movimentation.ExternalForceDeceleration, 0, _externalForce.magnitude);
        }

        private IEnumerator AttackDeleyed(Character target)
        {
            yield return new WaitForSeconds(Weapon.DelayToAttack);
            Weapon.WeaponObject.Attack(target);
            _deleyedAttackStarted = false;
        }

        public void SetWeapon(Game.Mecanics.Weapon weapon)
        {
            if (!Weapon.Hand)
            {
                Debug.LogError($"Character '{gameObject.name}' does not have '{nameof(Weapon.Hand)}' of '{nameof(Weapon)}' assigned. Is not possible set the weapon.");
                return;
            }

            weapon.transform.parent = Weapon.Hand;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.Owner = this;
            Weapon.WeaponObject = weapon;

            OnSetWeapon.Invoke();
        }

        public void AddDamage(float damage)
        {
            CurrentLife -= damage;
            OnDamaged.Invoke();

            if (CurrentLife <= 0)
            {
                CurrentLife = 0;
                enabled = false;

                OnDeath.Invoke();
                Destroy(gameObject, Life.AutoDestroyOnDeathDelay);
            }
        }

        public void KillCharacter()
        {
            AddDamage(Mathf.Infinity);
        }

        public void ResetLife()
        {
            CurrentLife = Life.LifeAmount;
        }

        public void AddExternalForce(Vector3 force)
        {
            _externalForce += force;
        }

        public virtual void Attack(Character target = null)
        {
            if (!Weapon.WeaponObject || Weapon.WeaponObject.IsAttacking)
            {
                return;
            }

            if (IsStoped && !_deleyedAttackStarted)
            {
                _deleyedAttackStarted = true;
                StartCoroutine(AttackDeleyed(target));
            }
        }
    }
}

