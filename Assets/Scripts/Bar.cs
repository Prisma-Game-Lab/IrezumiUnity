using UnityEngine;

//Default bar, can be used for hp
namespace Assets.Scripts
{
    public class Bar : MonoBehaviour
    {
        #region Variables
        private float _value;           /*current value*/
        private float _fullBarValue;    /*value when bar is full*/
        private Vector2 _size;          /*size of bar*/
        public Vector2 Position;        /*position to create the bar*/
        public Texture2D EmptyBarImg;   /*image with empty bar*/
        public Texture2D FullBarImg;    /*image with full bar*/
        #endregion

        public void Start()
        {
            _size = new Vector2(60, 20);
            Position = new Vector2(20, 40); 
        }

        /// <summary>
        /// Unity calls this function for rendering and handling GUI events.
        /// </summary>
        public void OnGUI () {
            /*background*/
            GUI.BeginGroup (new Rect (Position, _size));
            GUI.Box (new Rect (0, 0, _size.x, _size.y), EmptyBarImg);

            /*full part*/
            GUI.BeginGroup (new Rect (0,0 , _size.x * (_value/_fullBarValue), _size.y));
            GUI.Box (new Rect (0, 0, _size.x, _size.y), FullBarImg);
            GUI.EndGroup();
            GUI.EndGroup();
        }

        /// <summary>
        /// Change the value of the bar
        /// </summary>
        /// <param name="newValue">new value for bar</param>
        public void ChangeValue(float newValue){
            _value = newValue;
        }

        /// <summary>
        /// Change the value of the bar when it's full
        /// </summary>
        /// <param name="newValue">new max value for bar</param>
        public void ChangeFullBarValue(float newValue){
            _fullBarValue = newValue;
        }
    }
}
