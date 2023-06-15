using UnityEngine;

namespace Game.Config
{
    /// <summary>
    /// Armazena e aplica todas as configurações graficas do jogo
    /// </summary>
    public class GraphicConfigManager : MonoBehaviour
    {
        void Awake()
        {
            // Otimiza o jogo para rodar a 60 fps em plataformas mobile
            // por padrão o jogo é limitado a 30 fps
            if (Application.targetFrameRate != 60)
            {
                Application.targetFrameRate = 60;
            }
        }
    }
}