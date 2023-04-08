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
            [HideInInspector] public float DelatyToAttackMultiplier;

            public WeaponSlot()
            {
                DelayToAttack = 0.5f;
                DelatyToAttackMultiplier = 1;
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

        [System.Serializable]
        public class AnimationControl
        {
            [Space]
            public Animator Animator;

            [Tooltip("A interger value, from 0 to 1 representing if the player is moving")]
            public string MovimentParam;

            [Tooltip("Bool parameter, check if the character is death")]
            public string IsDeath;

            [Tooltip("Bool parameter")]
            public string IsAttacking;

            [Tooltip("Bool parameter, true if the character is using special attack animation")]
            public string IsSuperAttack;

            [Tooltip("Interger parameter, what weapon animation ID is using on AnimatorController?")]
            public string AttackAnimationID;

            public AnimationControl()
            {
                MovimentParam = "Vertical";
                IsDeath = "Is Death";
                IsAttacking = "Is Attacking";
                IsSuperAttack = "Is Super Attack";
                AttackAnimationID = "Attack Animation ID";
            }
        }

        public Moviment Movimentation;
        public WeaponSlot Weapon;
        public LifeStorage Life;
        public AnimationControl Animation;

        [Space]

        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnSetWeapon;

        [Header("Animation Events")]
        public UnityEvent OnAttackAnimationEvent;

        private bool _deleyedAttackStarted;
        private Vector3 _moveDirection;
        private Vector3 _externalForce;
        private NavMeshPath _aiPath;
        private CharacterController _characterController;
        private List<PowerUp> _powerUps;

        private float DelayToAttack => Weapon.DelayToAttack * Weapon.DelatyToAttackMultiplier;

        public CharacterController CharacterController => _characterController;

        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;
        public bool IsInvencible { get; set; }
        public bool IsDeath { get; private set; }
        public bool IsAttacking => HasWeapon && Weapon.WeaponObject.IsAttacking;
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
            _powerUps = new List<PowerUp>();

            CanMove = true;
            CurrentLife = Life.LifeAmount;
            LookAtDirection = transform.forward;
            Weapon.DelatyToAttackMultiplier = 1;

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

        protected virtual void LateUpdate()
        {
            // update on late update to process all character logic
            UpdateAnimations();
        }

        private void UpdateMoviment(float delta)
        {
            if (!CharacterController)
            {
                return;
            }

            if (!CanMove || IsAttacking)
            {
                CharacterMoveDirection = Vector3.zero;
            }

            CharacterVelocity = CharacterMoveDirection;
            CharacterVelocity *= Movimentation.MoveSpeed;
            CharacterVelocity = new Vector3(CharacterVelocity.x, -Movimentation.Gravity, CharacterVelocity.z);
            CharacterVelocity += _externalForce;
            CharacterVelocity *= delta;

            CharacterController.Move(CharacterVelocity);
        }

        private void UpdateRotation(float delta)
        {
            if (!CanMove || IsAttacking)
            {
                return;
            }

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
            yield return new WaitForSeconds(DelayToAttack);
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

        protected void UpdateAnimations()
        {
            if (!Animation.Animator)
            {
                return;
            }

            var _isAttacking = false;
            if (HasWeapon && Weapon.WeaponObject.IsAttacking) _isAttacking = true;
            if (IsDeath) _isAttacking = false;

            Animation.Animator.SetFloat(Animation.MovimentParam, CharacterMoveDirection.magnitude);
            Animation.Animator.SetBool(Animation.IsAttacking, _isAttacking);
            Animation.Animator.SetBool(Animation.IsDeath, IsDeath);

            if (HasWeapon)
            {
                Animation.Animator.SetBool(Animation.IsSuperAttack, Weapon.WeaponObject.IsSuperAttack);
                Animation.Animator.SetInteger(Animation.AttackAnimationID, Weapon.WeaponObject.AnimationID);
            }
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
                IsDeath = true;
                StartCoroutine(DisableCharacter());

                OnDeath.Invoke();

                // wait to finalize all character logic to desactive
                IEnumerator DisableCharacter()
                {
                    yield return new WaitForEndOfFrame();
                    enabled = false;
                    Destroy(gameObject, Life.AutoDestroyOnDeathDelay);
                }
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

        public void AddPowerUp(PowerUp powerUp)
        {
            if (!enabled && !powerUp)
            {
                return;
            }

            powerUp.gameObject.transform.parent = transform;
            powerUp.gameObject.transform.localPosition = Vector3.zero;
            powerUp.gameObject.transform.localRotation = Quaternion.identity;
            powerUp.Owner = this;

            _powerUps.Add(powerUp);
        }

        public PowerUp[] GetPowerUps() => _powerUps.ToArray();

        public void UsePowerUps()
        {
            if (!enabled)
            {
                return;
            }

            foreach (var p in _powerUps)
            {
                p.UsePowerUp();
            }
        }

        public void RemovePowerUp(PowerUp powerUp)
        {
            powerUp.OnRemove();
            _powerUps.Remove(powerUp);
            Destroy(powerUp.gameObject);
        }

        public void RemovePowerUps()
        {
            foreach (var p in _powerUps)
            {
                p.OnRemove();
                Destroy(p.gameObject);
            }

            _powerUps.Clear();
        }

        // called by character animation event
        public void AttackAnimationEvent()
        {
            OnAttackAnimationEvent.Invoke();
        }
    }
}

