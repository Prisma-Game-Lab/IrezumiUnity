using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class ArrowTrap : MonoBehaviour
    {
        public GameObject arrowPrefab;

        GameObject arrow;

        string[] dir = { "right", "up", "left", "down" };
        
        // Update is called once per frame
        void Start()
        {
            StartCoroutine(spawn());
        }

        IEnumerator spawn()
        {
            while (true)
            {
                for (int i = 0; i < 4; i++ )
                {
                    yield return new WaitForSeconds(1f);
                    arrow = Instantiate(arrowPrefab, this.transform.GetChild(i).position, Quaternion.identity) as GameObject;
                    arrow.GetComponent<Arrow>().Initialize(dir[i]);
                }
            }
        }
    }
}
