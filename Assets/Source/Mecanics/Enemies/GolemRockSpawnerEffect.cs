using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Effects
{
    public class GolemRockSpawnerEffect : MonoBehaviour
    {
        public GameObject Rock;

        private void Start()
        {
            Rock.SetActive(false);
        }

        public void GetRockAnimEvent()
        {
            Rock.SetActive(true);
        }

        public void DropRockAnimEvent()
        {
            Rock.SetActive(false);
        }
    }
}


