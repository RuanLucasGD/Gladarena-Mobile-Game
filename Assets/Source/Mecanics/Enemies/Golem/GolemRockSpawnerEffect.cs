using UnityEngine;

namespace Game.Effects
{
    // Usado pela animação de pegar a pedra do chão e e jogar.
    // Não interfere no sistema do golem.
    // Apenas mostra uma pedra fake que o golem pega do chão para jogar
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


