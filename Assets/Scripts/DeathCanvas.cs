using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class DeathCanvas : MonoBehaviour
    {

        // Use this for initialization
        Player PlayerScript;
        Animator DeathCanvasAnimator;
        public bool PlayerDead;
        private bool AnimationPlayed;
        private bool Started;
        private float Timed;

        void Start()
        {
            PlayerScript = GameObject.Find("Player").GetComponent<Player>();
            PlayerDead = false;
            AnimationPlayed = false;
            DeathCanvasAnimator = GetComponent<Animator>();
            Started = false;
        }

        // Update is called once per frame
        void Update()
        {
                       
            if(!PlayerScript.ALIVE)
            {
                PlayerDead = true;
                DeathCanvasAnimator.SetBool("PlayerIsDead", PlayerDead);
                DeathCanvasAnimator.SetBool("AnimationPlayed", AnimationPlayed);
                AnimationPlayed = true;
            }
            DeathCanvasAnimator.SetBool("Started", Started);
        }

        public void ResetLevel()
        {
            if (!PlayerScript.ALIVE)
            {
                GameManager.Instance.ResetLevel();
            }
            
        }
    }
}