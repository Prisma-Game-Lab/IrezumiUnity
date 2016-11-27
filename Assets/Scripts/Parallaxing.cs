using UnityEngine;

namespace Assets.Scripts
{
    public class Parallaxing : MonoBehaviour
    {
        public Transform[] backgrounds;
        public float smoothing = 1f;

        public bool yParallax;

        private float[] parallaxScales;
        private Transform cam;
        private Vector3 previousCamPos;

        void Awake()
        {
            cam = Camera.main.transform;
        }

        void Start()
        {
            previousCamPos = cam.position;
            parallaxScales = new float[backgrounds.Length];

            for (int i = 0; i < backgrounds.Length; i++)
            {
                parallaxScales[i] = backgrounds[i].position.z*-1;
            }
        }

        void Update()
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];
                float backgroundTargetPosX = backgrounds[i].position.x + parallaxX;

                Vector3 backgroundTargetPos;

                if (yParallax)
                {
                    float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];
                    float backgroundTargetPosY = backgrounds[i].position.y + parallaxY;
                    backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, backgrounds[i].position.z);
                }
                else
                {
                    backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
                }
                backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
            }

            previousCamPos = cam.position;
        }
    }
}
