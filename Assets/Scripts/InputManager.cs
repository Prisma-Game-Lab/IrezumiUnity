using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static bool Dash_Input()
        {
            return Input.GetButtonDown("Dash");
        }
        public static bool Jump_InputDown()
        {
            return Input.GetButtonDown("Jump");
        }
        public static bool Jump_InputUp()
        {
            return Input.GetButtonUp("Jump");
        }
        public static float Horizontal_Input()
        {
            float hi = 0.0f;
            hi += Input.GetAxis("Kb_Horizontal");
            hi += Input.GetAxis("J_Horizontal");
            return Mathf.Clamp(hi,-1,1);
        }
        public static float Vertical_Input()
        {
            float vi = 0.0f;
            vi += Input.GetAxis("Kb_Vertical");
            vi += Input.GetAxis("J_Vertical");
            return Mathf.Clamp(vi, -1, 1);
        }
        public static Vector2 Directional_Input()
        {
           return new Vector2 (Horizontal_Input(),Vertical_Input());
        }
        public static bool Directional_Zero()
        {
            return (Directional_Input() == Vector2.zero);
        }
    }
}

