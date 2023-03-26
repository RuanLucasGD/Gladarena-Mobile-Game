using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        private NavMeshPath _aiPath;
        private CharacterController _characterController;
        private PowerUp _powerUp;

        public CharacterController CharacterController => _characterController;

        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;
        public bool IsInvencible { get; set; }
        public bool IsDeath { get; private set; }
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
            _aiPath = new NavMeshPath();
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

        protected Vector3 GetAiPathDirection(Vector3 target)
        {
            if (NavMesh.SamplePosition(target, out var hit, Vector3.Distance(transform.position, target), 0))
                target = hit.position;
            var _pathFounded = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, _aiPath);
            var _direction = Vector3.zero;

            if (_pathFounded)
            {
                if (_aiPath.corners.Length > 1)
                {
                    _direction = (_aiPath.corners[1] - _aiPath.corners[0]).normalized;
                }
            }

            return _direction;
        }

        public void SetWeapon(Game.Mecanics.Weapon newWeapon)
        {
            if (!enabled)
            {
                return;
            }

            if (!Weapon.Hand)
            {
                Debug.LogError($"Character '{gameObject.name}' does not have '{nameof(Weapon.Hand)}' of '{nameof(Weapon)}' assigned. Is not possible set the weapon.");
                return;
            }

            if (!newWeapon)
            {
                if (Weapon.WeaponObject)
                {
                    Destroy(Weapon.WeaponObject.gameObject);
                    return;
                }
            }

            newWeapon.transform.parent = Weapon.Hand;
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;
            newWeapon.Owner = this;
            Weapon.WeaponObject = newWeapon;

            OnSetWeapon.Invoke();
        }

        public void AddDamage(float damage, Vector3 attackForce = default)
        {
            if (IsDeath || IsInvencible || !enabled)
            {
                return;
            }

            CurrentLife -= damage;
            AddExternalForce(attackForce);
            OnDamaged.Invoke();

            if (CurrentLife <= 0)
            {
                CurrentLife = 0;
                enabled = false;
                IsDeath = true;

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
            if (!enabled)
            {
                return;
            }

            CurrentLife = Life.LifeAmount;
        }

        public void AddExternalForce(Vector3 force)
        {
            if (!enabled)
            {
                return;
            }

            _externalForce += force;
        }

        public virtual void Attack(Character target = null)
        {
            if (!Weapon.WeaponObject || Weapon.WeaponObject.IsAttacking || !enabled)
            {
                return;
            }

            if (IsStoped && !_deleyedAttackStarted)
            {
                _deleyedAttackStarted = true;
                StartCoroutine(AttackDeleyed(target));
            }
        }

        public void SetPowerUp(PowerUp powerUp, bool destroyWhenRemove = true)
        {
            if (!enabled)
            {
                return;
            }

            if (powerUp)
            {
                powerUp.gameObject.transform.parent = transform;
                powerUp.gameObject.transform.localPosition = Vector3.zero;
                powerUp.gameObject.transform.localRotation = Quaternion.identity;
                powerUp.Owner = this;
            }
            else if (_powerUp)
            {
                if (destroyWhenRemove)
                {
                    Destroy(_powerUp.gameObject);
                }
                else
                {
                    _powerUp.transform.parent = null;
                }

                _powerUp.Owner = null;
            }

            _powerUp = powerUp;
        }

        public PowerUp GetPowerUp() => _powerUp;

        public void UsePowerUp()
        {
            if (!enabled)
            {
                return;
            }

            if (GetPowerUp())
            {
                GetPowerUp().UsePowerUp();
            }
        }
    }
}

