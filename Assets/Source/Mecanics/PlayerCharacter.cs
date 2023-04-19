using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Mecanics
{
    /// <summary>
    /// A character with player controls.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerInputs
        {
            /// <summary>
            /// The input action asset with all gameplay actions
            /// </summary>
            public InputActionAsset InputAsset;

            [Header("Movimentation")]

            [Tooltip("Axis Positive / Negative to move to forward or backward")]
            public string VerticalAction;

            [Tooltip("Axis Positive / Negative to move to right or back")]
            public string HorizontalAction;

            [Tooltip("Vector2 action value with direction to player move. \n'X' to right/left and 'Y' to forward/backward")]
            public string MobileJoystickAction;

            public PlayerInputs()
            {
                VerticalAction = "Vertical";
                HorizontalAction = "Horizontal";
                MobileJoystickAction = "Mobile Joystick";
            }
        }

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
            public float AttackRate;
            public float AttackRateMultiplier;

            public WeaponSlot()
            {
                AttackRate = 0.2f;
                AttackRateMultiplier = 1;
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

        public PlayerInputs InputMaps;
        public Moviment Movimentation;
        public WeaponSlot Weapon;
        public LifeStorage Life;
        public AnimationControl Animation;

        [Space]

        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnSetWeapon;
        public UnityEvent OnRevive;

        [Header("Animation Events")]
        public UnityEvent OnAttackAnimationEvent;

        private Vector3 _moveDirection;
        private Vector3 _externalForce;
        private CharacterController _characterController;
        private List<PowerUp> _powerUps;

        public CharacterController CharacterController => _characterController;

        public float CurrentAttackRate => Weapon.AttackRate * Weapon.AttackRateMultiplier;
        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;
        public bool IsGrounded => CharacterController.isGrounded;
        public bool IsInvencible { get; set; }
        public bool IsDeath { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool HasWeapon => Weapon.WeaponObject;
        public float CurrentLife { get; private set; }

        public InputAction VerticalAction { get; private set; }
        public InputAction HorizontalAction { get; private set; }
        public InputAction MobileJoystickAction { get; private set; }

        public Vector3 LookAtDirection { get; set; }
        public Vector3 Forward { get; set; }
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

        private Vector3 _ArenaCenter => ArenaManager.Instance ? ArenaManager.Instance.ArenaCaracteristics.ArenaCenter.position : Vector3.zero;

        private void OnEnable()
        {
            InputMaps.InputAsset.Enable();
        }

        private void OnDisable()
        {
            InputMaps.InputAsset.Disable();
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _powerUps = new List<PowerUp>();

            CanMove = true;
            CurrentLife = Life.LifeAmount;
            LookAtDirection = transform.forward;

            if (!CharacterController)
            {
                Debug.LogError($"Character Controller not added on gameObject {CharacterController}");
                return;
            }

            Forward = Camera.main ? Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) : transform.forward;
        }

        private void Start()
        {
            if (Weapon.WeaponObject)
            {
                SetWeapon(Weapon.WeaponObject);
            }

            if (!InputMaps.InputAsset)
            {
                Debug.LogError("Input action map not added on player!");
                return;
            }

            VerticalAction = InputMaps.InputAsset.FindAction(InputMaps.VerticalAction, throwIfNotFound: true);
            HorizontalAction = InputMaps.InputAsset.FindAction(InputMaps.HorizontalAction, throwIfNotFound: true);
            MobileJoystickAction = InputMaps.InputAsset.FindAction(InputMaps.MobileJoystickAction, throwIfNotFound: true);

            Attack();
        }

        private void Update()
        {
            UpdateRotation(Time.deltaTime);
            UpdateMoviment(Time.deltaTime);
            UpdateExternalForce(Time.deltaTime);

            UpdatePlayerControls();
        }

        private void LateUpdate()
        {
            UpdateAnimations();
        }

        private void UpdatePlayerControls()
        {
            if (!InputMaps.InputAsset)
            {
                return;
            }

            if (Forward.magnitude == 0)
            {
                Debug.LogError($"Player {nameof(Forward)} can not be have magnetude as 0. Set player move direction to another value, example {Vector3.forward}");
                return;
            }

            // the movement inputs is the sum of gamepad/keyboard inputs with mobile joystick
            var _mobileJoystick = MobileJoystickAction.ReadValue<Vector2>();
            var _vertical = VerticalAction.ReadValue<float>();
            var _horizontal = HorizontalAction.ReadValue<float>();
            _vertical += _mobileJoystick.y;
            _horizontal += _mobileJoystick.x;

            var _forward = Forward;
            var _right = -Vector3.Cross(_forward, Vector3.up);
            var _moveDirection = ((_forward * _vertical) + (_right * _horizontal)).normalized;

            CharacterMoveDirection = _moveDirection;
        }

        private void UpdateMoviment(float delta)
        {
            if (!CharacterController)
            {
                return;
            }

            if (!CanMove)
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
            if (!CanMove)
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

        private void UpdateAnimations()
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
            if (IsDeath || !enabled)
            {
                return;
            }

            if (!IsInvencible)
            {
                CurrentLife -= damage;
                AddExternalForce(attackForce);
            }

            OnDamaged.Invoke();

            if (CurrentLife <= 0)
            {
                CurrentLife = 0;
                IsDeath = true;
                StartCoroutine(DisableCharacter());
                StartCoroutine(DestroyCharacterDeleyed());

                OnDeath.Invoke();

                // wait to finalize all character logic to desactive
                IEnumerator DisableCharacter()
                {
                    yield return new WaitForEndOfFrame();
                    enabled = false;
                }

                IEnumerator DestroyCharacterDeleyed()
                {
                    yield return new WaitForSeconds(Life.AutoDestroyOnDeathDelay);

                    if (IsDeath)
                    {
                        enabled = false;
                        Destroy(gameObject);
                    }
                }
            }
        }

        private void Attack()
        {
            if (IsAttacking || !enabled)
            {
                return;
            }

            Weapon.WeaponObject.Attack();
            Invoke(nameof(FinalizeAttack), Weapon.WeaponObject.CurrentAttackLength);
        }

        public void KillCharacter()
        {
            AddDamage(Mathf.Infinity);
        }

        public void ResetLife()
        {
            CurrentLife = Life.LifeAmount;

            if (IsDeath)
            {
                IsDeath = false;
                enabled = true;
                OnRevive.Invoke();
            }
        }

        public void AddExternalForce(Vector3 force)
        {
            if (!enabled)
            {
                return;
            }

            _externalForce += force;
        }

        private void FinalizeAttack()
        {
            Weapon.WeaponObject.IsAttacking = false;
            Invoke(nameof(Attack), CurrentAttackRate);
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