using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class ArrowTrap : MonoBehaviour
    {
        public GameObject arrowPrefab;

        GameObject arrow;

        /// <summary>
        /// Directions arrows will fire, in order. (in degrees).
        /// </summary>
        [SerializeField] private float[] _degrees;
        [SerializeField] private float _timeToFirstShoot;
        [SerializeField] private float _cooldownTime;
        [SerializeField] private float _arrowSpeed;
        private int _actualDir;
        // Update is called once per frame
        void Start()
        {
			Physics2D.IgnoreLayerCollision(0, 15);
            _actualDir = 0;
            StartCoroutine(spawn());
        }

        IEnumerator spawn()
        {
            yield return new WaitForSeconds(_timeToFirstShoot);
            while (true)
            {
                //for (int i = 0; i < 4; i++ )
                //{
                    //yield return new WaitForSeconds(1f);
                    yield return new WaitForSeconds(_cooldownTime);
                arrow = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity) as GameObject;
                //arrow = Instantiate(arrowPrefab, this.transform.GetChild(i).position, Quaternion.identity) as GameObject;
                //arrow.transform.Rotate(new Vector3(0, 0, i*90));
                arrow.gameObject.GetComponent<Arrow>().speed = _arrowSpeed;
                arrow.transform.Rotate(new Vector3(0, 0, _degrees[_actualDir]));
                    _actualDir = (_actualDir + 1)%_degrees.Length;
                //}
            }
        }
    }
}
