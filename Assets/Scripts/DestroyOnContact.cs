using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Assets.Scripts
{
    public class DestroyOnContact : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.tag == "Player")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
