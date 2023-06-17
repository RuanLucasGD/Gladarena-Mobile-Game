using UnityEngine;
using Game.Mecanics;

namespace Game.Effects
{
    public class PlayerSfx : MonoBehaviour
    {
        public AudioSource Source;
        public AudioClip WeaponAttackClip;
        public PlayerCharacter Character;

        private void Start()
        {
            // deve funcionar apenas no player original
            // clones do player não possuem a tag Player
            if (transform.tag != "Player")
            {
                enabled = false;
            }
        }

        private void PlayAttackSound()
        {
            Source.PlayOneShot(WeaponAttackClip);
        }

        /// <summary>
        /// Chamado pelo AnimatorEvent do jogador.
        /// Esse metodo deve ser chamado a cada inicio de animação de ataque
        /// Caso o som não seja executado, certifique-se de que a animação de ataque
        /// Tenha esse evento anexado
        /// https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html
        /// </summary>
        public void StartAttackAnimationEvent()
        {
            if (!enabled)
            {
                return;
            }

            PlayAttackSound();
        }
    }
}