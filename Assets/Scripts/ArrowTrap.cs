using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class ArrowTrap : MonoBehaviour
    {
        public GameObject arrowPrefab;

        GameObject arrow;
                
        // Update is called once per frame
        void Start()
        {
			Physics2D.IgnoreLayerCollision(0, 15);
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
                    arrow.transform.Rotate(new Vector3(0, 0, i*90));
                }
            }
        }
    }
}
