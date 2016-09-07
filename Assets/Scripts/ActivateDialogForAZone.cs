using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ActivateDialogForAZone : MonoBehaviour
    {
        public TextAsset TextFile;
        public int StartLine;
        public int EndLine;
        public bool DestroyWhenActivated;
        public bool RequireButtonPress;
        private bool _waitForPress;

        private DialogBox _dialogBox;

        // Use this for initialization
        void Start ()
        {
            _dialogBox = FindObjectOfType<DialogBox>();
        }
	
        // Update is called once per frame
        void Update () {

            if (_waitForPress && Input.GetKeyDown(KeyCode.Return))
            {
                UpdateDialogBox();
            }
        }

        void OnCollisionExit2D(Collision2D collider)
        {
            if (collider.collider.name == "Player")
            {
                _waitForPress = false;
            }
        }

        void OnCollisionEnter2D(Collision2D collider)
        {
           if( collider.collider.name == "Player")
           {
               if (RequireButtonPress)
                {
                    _waitForPress = true;
                    return;
                }

               UpdateDialogBox();
           }
        }

        private void UpdateDialogBox()
        {
            _dialogBox.ReloadScript(TextFile);
            _dialogBox.CurrentLine = StartLine;
            _dialogBox.ChangeEndAtLine(EndLine);
            _dialogBox.EnableDialogBox();

            if (DestroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }
}
