using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    public class InitiateScript : MonoBehaviour
    {
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
