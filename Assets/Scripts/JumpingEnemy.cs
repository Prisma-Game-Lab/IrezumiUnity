using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class JumpingEnemy : MonoBehaviour
    {


        public Direction Direcao;
        public int JumpHeight;
        public float JumpDuration;
        private float _gravity;
        private Vector2 _velocity;
        private Vector3 _enemyPosition;
        private Dictionary<Direction, Vector2> _directionalVector;
        private bool _jumping;

        // Use this for initialization
        void Start ()
        {
            _gravity = JumpHeight/JumpDuration;
            _directionalVector = new Dictionary<Direction, Vector2>
            {
                {Direction.Cima, Vector2.down},
                {Direction.Baixo, Vector2.up },
                {Direction.Direita,  Vector2.left},
                {Direction.Esquerda, Vector2.right }

            };
        }
        
        // Update is called once per frame
        void Update ()
        {
            _enemyPosition = this.transform.position;
            
            var controller = gameObject.GetComponent<JumpingEnemyController>();

            if(!_jumping)
                CheckCollisions(controller);
            
            transform.Translate(_velocity * Time.deltaTime);

        }

        private void CheckCollisions(JumpingEnemyController controller)
        {
            if (Direcao == Direction.Cima || Direcao == Direction.Baixo)
                //Se movimento for para cima ou para baixo checar por colisao dem Y
            {
                if (controller.HitVertical((int) _directionalVector[Direcao].y))
                {

                    _velocity = new Vector3(0, _directionalVector[Direcao].y *_gravity, 0);
                    _jumping = true;
                    Invoke("ChangeJumping", JumpDuration);
                }
                else
                {
                    _velocity = _directionalVector[Direcao]* _gravity;
                }
            }
            else //Se movimento for para direita ou para esquerda checar por colisao dem X
            {
                if (controller.HitHorizontal((int) _directionalVector[Direcao].x))
                {
                    _velocity = new Vector3(_directionalVector[Direcao].x *_gravity, 0, 0);
                    _jumping = true;
                    Invoke("ChangeJumping", JumpDuration);
                }
                else
                {
                    _velocity = _directionalVector[Direcao]* _gravity;
                }
            }
        }

        void ChangeJumping()
        {
            _jumping = !_jumping;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().Damage(1000);
            }
        }

        public enum Direction
        {
            Cima, Baixo, Direita, Esquerda
        }
    }
}
