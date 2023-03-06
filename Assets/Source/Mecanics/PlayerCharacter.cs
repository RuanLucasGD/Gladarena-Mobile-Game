using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Mecanics
{
    /// <summary>
    /// A character with player controls.
    /// </summary>
    public sealed class PlayerCharacter : Character
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

        /// <summary>
        /// Has all settings of player control. 
        /// </summary>
        public PlayerInputs InputMaps;

        public InputAction VerticalAction { get; private set; }
        public InputAction HorizontalAction { get; private set; }
        public InputAction MobileJoystickAction { get; private set; }

        public Vector3 Forward { get; set; }

        public bool SearchEnemies => IsStoped;
        public bool IsAttacking => IsStoped && NearEnemies.Length > 0;
        public Character[] NearEnemies { get; private set; }

        private void OnEnable()
        {
            InputMaps.InputAsset.Enable();
        }

        private void OnDisable()
        {
            InputMaps.InputAsset.Disable();
        }

        protected override void Awake()
        {
            base.Awake();
            Forward = transform.forward;
            NearEnemies = new Character[0] { };
        }

        protected override void Start()
        {
            base.Start();

            if (!InputMaps.InputAsset)
            {
                Debug.LogError("Input action map not added on player!");
                return;
            }

            VerticalAction = InputMaps.InputAsset.FindAction(InputMaps.VerticalAction, throwIfNotFound: true);
            HorizontalAction = InputMaps.InputAsset.FindAction(InputMaps.HorizontalAction, throwIfNotFound: true);
            MobileJoystickAction = InputMaps.InputAsset.FindAction(InputMaps.MobileJoystickAction, throwIfNotFound: true);
        }

        protected override void Update()
        {
            base.Update();
            UpdatePlayerControls();

            if (HasWeapon)
            {
                UpdateNearEnemiesList();
                AttackEnemies();
            }
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

        private void UpdateNearEnemiesList()
        {
            var _allCharacter = new List<Character>(FindObjectsOfType<Character>());
            _allCharacter.Remove(this);  // remove self player of all characters list

            // remove all distant characters 
            for (int i = 0; i < _allCharacter.Count; i++)
            {
                var _weaponPosition = Weapon.WeaponObject.transform.position;
                var _enemyPosition = _allCharacter[i].transform.position;

                if (Vector3.Distance(_weaponPosition, _enemyPosition) > Weapon.WeaponObject.AttackRange)
                {
                    _allCharacter.Remove(_allCharacter[i]);
                }
            }

            // check if list is equals to avoid desnecerary memory allocation 
            if (_allCharacter.Count != NearEnemies.Length)
            {
                NearEnemies = _allCharacter.ToArray();
            }
        }

        private void AttackEnemies()
        {
            if (IsAttacking)
            {
                Weapon.WeaponObject.Attack();
            }
            else
            {
                Weapon.WeaponObject.IsAttacking = false;
            }
        }
    }
}

