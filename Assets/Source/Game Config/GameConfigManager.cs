using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class GameConfigManager : MonoBehaviour
    {
        void Awake()
        {
            if (Application.targetFrameRate != 60)
            {
                Application.targetFrameRate = 60;
            }
        }
    }
}