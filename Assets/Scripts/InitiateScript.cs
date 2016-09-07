using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Assets.Scripts
{
    public class InitiateScript : MonoBehaviour
    {

        private static GameObject _gameManager;
        
        [RuntimeInitializeOnLoadMethod]
        static void InitializeGameManager()
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            if (!gm)
            {
                GameObject gameManager = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Game Manager.prefab", typeof(GameObject));
                Instantiate(gameManager);
            }
        }
    }
}
