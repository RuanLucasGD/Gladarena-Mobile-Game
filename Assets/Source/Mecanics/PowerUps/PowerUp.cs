using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public abstract class PowerUp : ScriptableObject
    {
        [Header("General")]
        public string PowerUpName;

        [Header("General Events")]
        public UnityEvent OnSetupPowerUp;
        public UnityEvent OnUsePowerUp;
        public UnityEvent OnUpgrated;
        public UnityEvent OnFullUpgrated;

        public int CurrentLevelIndex { get; protected set; }

        protected virtual void OnEnable()
        {
            CurrentLevelIndex = 0;
        }

        public virtual void Upgrade()
        {
            CurrentLevelIndex++;
        }

        public abstract void Use();

        public abstract bool IsFullUpgrade();

        public abstract string UpgradeInfo();
    }
}
