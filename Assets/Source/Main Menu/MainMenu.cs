using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public string GameScene;

        public void LoadGameScene()
        {
            SceneManager.LoadScene(GameScene);
        }
    }
}
