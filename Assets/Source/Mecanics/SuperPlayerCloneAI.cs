using Game.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SuperPlayerCloneAI : PlayerCloneAI
    {
        [Header("Super Clone")]
        public float StartDashDelay;
        public float DashCooldown;
        public float DashTime;
        public bool ExplodeOnFinishDash;
        public int MaxDashs;

        private float _dashIntervalTimer;
        private float _dashLenghtTimer;
        private int _currentDashsAmount;
        private bool _dashStarted;

        protected override void Start()
        {
            base.Start();
            Invoke(nameof(StartDash), StartDashDelay);
        }

        protected override void Update()
        {
            if (!_dashStarted)
            {
                base.Update();
                return;
            }

            if (!DashModeEnabled && (_currentDashsAmount >= MaxDashs) && MaxDashs > 0)
            {
                Clone.IsInvencible = false;
                Clone.KillCharacter();
                enabled = false;
                return;
            }

            if (!DashModeEnabled)
            {
                _dashIntervalTimer += Time.deltaTime;
                if (_dashIntervalTimer > DashCooldown)
                {
                    _dashIntervalTimer = 0;
                    _currentDashsAmount++;
                    EnableDashMode();
                }
            }

            if (DashModeEnabled)
            {
                _dashLenghtTimer += Time.deltaTime;
                if (_dashLenghtTimer > DashTime)
                {
                    if (ExplodeOnFinishDash)
                    {
                        PowerUpController.AddExplosion(transform.position);
                    }

                    _dashLenghtTimer = 0;
                    DisableDashMode();
                }
            }

            if (DashModeEnabled) DashMode();
            else IdleMode();
        }

        private void StartDash()
        {
            _dashStarted = true;
        }
    }
}
