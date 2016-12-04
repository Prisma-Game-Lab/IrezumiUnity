using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DialogBox : MonoBehaviour
    {
        public GameObject DialogBoxObject;
        public Text DialogText;
        public TextAsset TextFile;
        public bool TestOnScreen;
        public string[] TextLines;
        public int CurrentLine;
        public int EndAtLine;
        public bool IsActive;
        public Font Font;
        public int FontSize;
        public bool ScrollText;
        public int Cps;


        private Image[] _dialogPortrait;
        private float _typeSpeed;
        private float _typeSpeedDefaut;
        private bool _isTyping;
        private bool _cancelTyping;
        private GameManager _gameManager;
        private string _visualNovelFolder = "VisualNovel/";
        private readonly Regex _showRegex = new Regex(@"(\bshow (left|right|center) [A-z0-9]+\b)");
        private readonly Regex _cpsRegex = new Regex(@"\<cps( )*=( )*[0-9]+\>");
        private readonly Regex _cpsEndRegex = new Regex(@"\<\/cps\>");
        private readonly Regex _waitRegex = new Regex(@"\<w( )*=( )*[0-9]+\>");

        public enum Position
        {
            Left,
            Right,
            Center
        }

        // Use this for initialization
        void Start ()
        {
            _typeSpeed = (float) (1.0/Cps);
            _typeSpeedDefaut = _typeSpeed;

            SetAllDialogPortraits();
           
            _gameManager = GameManager.Instance;
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

        private void SetAllDialogPortraits()
        {
            _dialogPortrait = new Image[3];

            _dialogPortrait[(int)Position.Left] = transform.Find("DialogPortraitLeft").GetComponent<Image>();
            _dialogPortrait[(int)Position.Center] = transform.Find("DialogPortraitCenter").GetComponent<Image>();
            _dialogPortrait[(int)Position.Right] = transform.Find("DialogPortraitRight").GetComponent<Image>();

            for (int i = 0; i < _dialogPortrait.Length; i++)
            {
                _dialogPortrait[i].GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
                _dialogPortrait[i].enabled = false;
            }
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
        /// <param charname="lineOfText"></param>
        /// <returns></returns>
        private IEnumerator TextScroll(string lineOfText)
        {
            int waitTime = 0;
            var letter = 0;
            DialogText.text = "";
            _isTyping = true;
            _cancelTyping = false;
            while (_isTyping && !_cancelTyping && letter < lineOfText.Length -1)
            {
                if (lineOfText[letter] == '<')
                {
                    if (!CheckIfCps(ref lineOfText))
                        if (!CheckIfCpsEnd(ref lineOfText))
                            if(CheckIfWait(ref lineOfText, ref waitTime))
                                yield return CoroutineUtilities.WaitForRealtimeSeconds(waitTime);
                }
                DialogText.text += lineOfText[letter++];
                yield return CoroutineUtilities.WaitForRealtimeSeconds(_typeSpeed);
            }
            DialogText.text = FilterForCommands(lineOfText);
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
                DialogText.text = FilterForCommands(TextLines[CurrentLine]);
        }

        /// <summary>
        /// if user cancels scrolling remove all comands from line before letting it go to the screen
        /// </summary>
        /// <param charname="textLine"></param>
        /// <returns></returns>
        private string FilterForCommands(string textLine)
        {
            textLine = _cpsRegex.Replace(textLine, "");
            textLine = _cpsEndRegex.Replace(textLine, "");
            textLine = _waitRegex.Replace(textLine, "");
            return textLine;
        }

        public void DisableDialogBox()
        {
            DialogBoxObject.SetActive(false);
            IsActive = false;
            for (var i = 0; i < _dialogPortrait.Length; i++)
            {
                _dialogPortrait[i].enabled = false;
            }
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

        private bool CheckIfCps(ref string currentLine)
        {
            var match = _cpsRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                var split = match.ToString().Split(' ');
                var number = int.Parse(split[2].Split('>')[0]);
                _typeSpeed = (float) (1.0/number);
                currentLine = _cpsRegex.Replace(currentLine, "");
            }
            return sucess;
        }

        private bool CheckIfCpsEnd(ref string currentLine)
        {
            var match = _cpsEndRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                _typeSpeed = _typeSpeedDefaut; //sets Type Speed to default
                currentLine = _cpsEndRegex.Replace(currentLine, "");
            }
            return sucess;
        }
        

        private bool CheckIfWait(ref string currentLine, ref int waitTime)
        {
            var match = _waitRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                var split = match.ToString().Split(' ');
                waitTime = int.Parse(split[2].Split('>')[0]);
                currentLine = _waitRegex.Replace(currentLine, "");
            }
            return sucess;
        }

        private bool CheckIfChangePortrait(string currentLine)
        {
            var match = _showRegex.Match(currentLine);
            if (match.Success)
            {
                var split = match.ToString().Split(' ');
                var position = split[1];
                var charname = split[2];
                ChangePortrait(position, charname);
                return true;
            }
            return false;
        }
        
        private void ChangePortrait(string position, string charname)
        {
            var pos =0;
            switch (position)
            {
                case "left":
                    pos = (int)Position.Left;
                    break;
                case "right":
                    pos = (int)Position.Right;
                    break;
                case "center":
                    pos = (int)Position.Center;
                    break;
            }
            _dialogPortrait[pos].enabled = true;
            var sprite = Resources.Load<Sprite>(_visualNovelFolder + charname);

            if (sprite != null)
                _dialogPortrait[pos].sprite = sprite;
            else
                _dialogPortrait[pos].enabled = false;
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
