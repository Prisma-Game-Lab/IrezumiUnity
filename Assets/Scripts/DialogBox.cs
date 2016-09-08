using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DialogBox : MonoBehaviour
    {
        public GameObject DialogBoxObject;
        public Text DialogText;
        public TextAsset TextFile;
        public Image DialogPortrait;
        public Sprite[] Sprites;
        public bool TestOnScreen;
        public string[] TextLines;
        public int CurrentLine;
        public int EndAtLine;
        public bool IsActive;
        public float TypeSpeed;
        public Font Font;
        public int FontSize;
        public bool ScrollText;

        private bool _isTyping;
        private bool _cancelTyping;
        private GameManager _gameManager;

        // Use this for initialization
        void Start ()
        {
            DialogPortrait.GetComponent<Image>().color = new Vector4(255,255,255, 255);
            DialogPortrait.enabled = false;
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            CurrentLine = 0;
            if (DialogText != null)
            {
                if (Font != null)
                    DialogText.font = Font;
                if (FontSize != 0)
                    DialogText.fontSize = FontSize;
            }
            if (TextFile != null)
                TextLines = TextFile.text.Split('\n');//slip txt file in lines
            if (EndAtLine == 0) //endatline not set in inspector then get all lines
                EndAtLine = TextLines.Length -1;
            if (IsActive)
                EnableDialogBox();
            else
                DisableDialogBox();
        }
	

        // Update is called once per frame
        void Update ()
        {
            if(!IsActive)
                return;

            _gameManager.EnablePause();
            if (Input.anyKeyDown)
            {
                if (!_isTyping)
                {
                    CurrentLine++;
                    if (CurrentLine > EndAtLine)
                    {
                        DisableDialogBox();
                    }
                    else
                    {
                        PreprocessLine();
                        WriteDialog();
                    }
                }
                else if(_isTyping && !_cancelTyping) //is already scrolling
                    _cancelTyping = true;
            }
        }

        /// <summary>
        /// Courotine to type each letter
        /// </summary>
        /// <param name="lineOfText"></param>
        /// <returns></returns>
        private IEnumerator TextScroll(string lineOfText)
        {
            var letter = 0;
            DialogText.text = "";
            _isTyping = true;
            _cancelTyping = false;
            while (_isTyping && !_cancelTyping && letter < lineOfText.Length -1)
            {
                DialogText.text += lineOfText[letter++];
                yield return CoroutineUtilities.WaitForRealtimeSeconds(TypeSpeed);
            }
            DialogText.text = lineOfText;
            _isTyping = false;
            _cancelTyping = false;
        }

        public void EnableDialogBox()
        {
            _gameManager.EnablePause();
            DialogBoxObject.SetActive(true);
            IsActive = true;
            PreprocessLine();
            WriteDialog();
        }

        /// <summary>
        /// Checks if the line is a dialog or a 'change portrait' command
        /// if it is a command then we should skip the line and check if the next line exists
        /// </summary>
        private void PreprocessLine()
        {
            if (CheckIfChangePortrait(TextLines[CurrentLine]))
            {
                CurrentLine++;
                if (CurrentLine > EndAtLine)
                {
                    DisableDialogBox();
                }
            }
        }

        /// <summary>
        /// Start a coroutine to scroll the text (if ScrollText is true) or write the entire line at once
        /// </summary>
        private void WriteDialog()
        {
            if (ScrollText)
                StartCoroutine(TextScroll(TextLines[CurrentLine]));
            else
                DialogText.text = TextLines[CurrentLine];
        }

        public void DisableDialogBox()
        {
            DialogBoxObject.SetActive(false);
            IsActive = false;
            DialogPortrait.enabled = false;
            _gameManager.DisablePause();
        }

        public void ReloadScript(TextAsset textFile)
        {
            if (textFile != null)
            {
                TextLines = new string[TextFile.text.Length];
                TextLines = textFile.text.Split('\n');
            }
        }

        private bool CheckIfChangePortrait(string currentLine)
        {
            if (currentLine[0] == '>')
            {
                if (currentLine[1] == '\0')
                {
                    Debug.Log("Número esperado. Portrait não foi trocado.");
                    return false;
                }
                ChangePortrait(int.Parse(currentLine[1].ToString()));
                return true;
            }
            return false;
        }

        private void ChangePortrait(int portraitNumber)
        {
            DialogPortrait.enabled = true;
            if (Sprites[portraitNumber] != null)
                DialogPortrait.sprite = Sprites[portraitNumber];
        }

        public void ChangeEndAtLine(int newEndAtLine)
        {
            if (newEndAtLine == 0) //endatline not set in inspector then get all lines
                EndAtLine = TextLines.Length - 1;
            if (newEndAtLine <= TextLines.Length - 1)
                EndAtLine = newEndAtLine;
            else
                EndAtLine = TextLines.Length - 1;
        }
    }
}
