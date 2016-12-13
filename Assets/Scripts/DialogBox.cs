using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DialogBox : MonoBehaviour
    {
        #region Variables
        public GameObject DialogBoxObject;
        public Text DialogText;
        public TextAsset TextFile;
        public bool IsActive;
        public Font Font;
        public int FontSize;
        public int NamesSize;
        public bool ScrollText;
        public int Cps;
        public float ScaleFactor;
        [HideInInspector]
        public int CurrentLine;
        public float OffsetX;
        public float OffsetY;

        private int _endAtLine;
        private string[] _textLines;
        private Image[] _dialogPortraits;
        private float _typeSpeed;
        private float _typeSpeedDefaut;
        private bool _isTyping;
        private bool _cancelTyping;
        private GameManager _gameManager;
        private bool _fixed;
        private bool _cpsClosed;
        private const string _visualNovelFolder = "VisualNovel/";
        private readonly Regex _showRegex = new Regex(@"(\bshow (left|right|center) [A-z0-9]+\b)");
        private readonly Regex _cpsRegex = new Regex(@"\<cps( )*=( )*[0-9]+\>");
        private readonly Regex _cpsEndRegex = new Regex(@"\<\/cps\>");
        private readonly Regex _waitRegex = new Regex(@"\<w( )*=( )*[0-9]+(\.)?[0-9]*\>");
        private readonly Regex _talkingRegex = new Regex(@"(\btalking (left|right|center|none)\b)");
        private readonly Regex _fixedRegex = new Regex(@"\[fixed\]");
        private readonly Regex _nameRegex = new Regex(@"\[[A-z0-9 ]+\]");
        private bool _hasName;
        private string _charName;
        private GameObject _endTextIcon;

        internal enum Position
        {
            Left,
            Right,
            Center
        }
        #endregion


        // Use this for initialization
        public void Start ()
        {
            _endTextIcon = transform.FindChild("EndTextIcon").gameObject;
            _endTextIcon.SetActive(false);

            ScaleFactor = 1.3f;

            _cpsClosed = true;
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
                _textLines = TextFile.text.Split('\n');//slip txt file in lines
            if (_endAtLine == 0) //endatline not set in inspector then get all lines
                _endAtLine = _textLines.Length -1;
            if (IsActive)
                EnableDialogBox();
            else
                DisableDialogBox();
        }

        private void SetAllDialogPortraits()
        {
            _dialogPortraits = new Image[3];

            _dialogPortraits[(int)Position.Left] = transform.Find("DialogPortraitLeft").GetComponent<Image>();
            _dialogPortraits[(int)Position.Center] = transform.Find("DialogPortraitCenter").GetComponent<Image>();
            _dialogPortraits[(int)Position.Right] = transform.Find("DialogPortraitRight").GetComponent<Image>();

            foreach (var dialogPortrait in _dialogPortraits)
            {
                dialogPortrait.GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
                dialogPortrait.enabled = false;
                dialogPortrait.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }


        // Update is called once per frame
        public void Update ()
        {
            if(!IsActive)
                return;

            _gameManager.EnablePause(); //pause game //todo: instead of pausing, focus camera on other things
            if (Input.anyKeyDown) 
            {
                if (Input.GetMouseButtonDown(0)
                || Input.GetMouseButtonDown(1)
                || Input.GetMouseButtonDown(2))
                    return; //Do Nothing, only keyboard allowed

                if (!_isTyping)
                {
                    CurrentLine++;
                    if (CurrentLine > _endAtLine)
                    {
                        DisableDialogBox();
                    }
                    else
                    {
                        while (PreprocessLine()){}
                        WriteDialog();
                    }
                }
                else if(_isTyping && !_cancelTyping && !_fixed) //is already scrolling
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
            float waitTime = 0;
            var letter = 0;
            _isTyping = true;
            _cancelTyping = false;

            lineOfText = Regex.Unescape(lineOfText);

            while (_isTyping && !_cancelTyping && letter < lineOfText.Length -1)
            {
                bool wait = false;
                ProcessCommandsInLine(ref lineOfText, letter, ref waitTime, ref wait);

                if (wait)
                    yield return CoroutineUtilities.WaitForRealtimeSeconds(waitTime);

                DialogText.text += lineOfText[letter++];
                yield return CoroutineUtilities.WaitForRealtimeSeconds(_typeSpeed);
            }
            
            DialogText.text = FilterForCommands(lineOfText);
            _isTyping = false;
            _cancelTyping = false;
        }

        private void ProcessCommandsInLine(ref string lineOfText, int letter, ref float waitTime, ref bool wait)
        {
            if (lineOfText[letter] == '<')
            {
                if (!CheckIfCps(ref lineOfText))
                    if (!CheckIfCpsEnd(ref lineOfText))
                        if (CheckIfWait(ref lineOfText, ref waitTime))
                            wait = true;
            }
        }

        public void EnableDialogBox()
        {
            _gameManager.EnablePause();
            DialogBoxObject.SetActive(true);
            IsActive = true;
            while (PreprocessLine()){}
            WriteDialog();
        }

        /// <summary>
        /// Checks if the line is a dialog or a 'change portrait'/'talking' command or a empty line
        /// if it is a command or empty line then we should skip the line and check if the next line exists (is not the last one)
        /// after that we initialize the dialogtext with an empty string and check for [Name] or [fixed]
        /// </summary>
        private bool PreprocessLine()
        {
            if (CheckIfChangePortrait(_textLines[CurrentLine]) || CheckIfCharacterIsTalking(_textLines[CurrentLine]) || CheckIfEmptyLine(_textLines[CurrentLine]))
            {
                CurrentLine++;
                if (CurrentLine > _endAtLine)
                {
                    DisableDialogBox();
                }
                return true;
            }
            DialogText.text = ""; //inicializa o text
            CheckForFixed(ref _textLines[CurrentLine]);
            CheckForName(ref _textLines[CurrentLine]);
            return false;
        }

        /// <summary>
        /// Start a coroutine to scroll the text (if ScrollText is true) or write the entire line at once
        /// </summary>
        private void WriteDialog()
        {
            _endTextIcon.SetActive(false);
            if (ScrollText)
                StartCoroutine(TextScroll(_textLines[CurrentLine]));
            else
                DialogText.text = FilterForCommands(_textLines[CurrentLine]);
            
        }

        /// <summary>
        /// if user cancels scrolling remove all comands from line before letting it go to the screen
        /// </summary>
        /// <param name="textLine"></param>
        /// <returns></returns>
        private string FilterForCommands(string textLine)
        {
            if (_hasName)
                textLine = _charName + "\n" + textLine.Trim();
            textLine = _cpsRegex.Replace(textLine, "");
            textLine = _cpsEndRegex.Replace(textLine, "");
            textLine = _waitRegex.Replace(textLine, "");

            _endTextIcon.transform.position = transform.FindChild("EndIconPos").gameObject.transform.position;
            _endTextIcon.SetActive(true);

            return textLine;
        }

        public void DisableDialogBox()
        {
            DialogBoxObject.SetActive(false);
            IsActive = false;
            foreach (var dialogPortrait in _dialogPortraits)
            {
                dialogPortrait.enabled = false;
            }
            _gameManager.DisablePause();

            _endTextIcon.SetActive(false);
        }

        public void ReloadScript(TextAsset textFile)
        {
            if (textFile != null)
            {
                _textLines = new string[TextFile.text.Length];
                _textLines = textFile.text.Split('\n');
            }
        }
        
        private void CheckForFixed(ref string currentLine)
        {
            _fixed = false;
            var match = _fixedRegex.Match(currentLine.ToLower());
            var sucess = match.Success;
            if (sucess)
            {
                _fixed = true;
                currentLine = _fixedRegex.Replace(currentLine, "");
            }
        }

        private void CheckForName(ref string currentLine)
        {
            _hasName = false;
            var match = _nameRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                var split =match.ToString().Split('[');
                var charname = split[1].Split(']')[0].Trim();
                currentLine = _nameRegex.Replace(currentLine, "");
                currentLine =  currentLine.Trim();
                _hasName = true;
                _charName = "<color=red><size="+ NamesSize + ">" + charname + "</size></color>";
                //DialogText.fontSize = FontSize + 1;
                DialogText.text += _charName + "\n";
                //DialogText.fontSize = FontSize;
            }
        }

        /// <summary>
        /// Check for cps command
        /// </summary>
        /// <param name="currentLine"></param>
        /// <returns></returns>
        private bool CheckIfCps(ref string currentLine)
        {
            if (!_cpsClosed) //nao deixa procurar por outro cps antes de ter fechado o anterior
                return false;
            var match = _cpsRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                var split = Regex.Match(match.ToString(), @"[0-9]+");
                var number = int.Parse(split.ToString());
                _typeSpeed = (float) (1.0/number);
                currentLine = _cpsRegex.Replace(currentLine, "", 1);
                _cpsClosed = false;
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
                currentLine = _cpsEndRegex.Replace(currentLine, "", 1);
                _cpsClosed = true;
            }
            return sucess;
        }

        private bool CheckIfWait(ref string currentLine, ref float waitTime)
        {
            var match = _waitRegex.Match(currentLine);
            var sucess = match.Success;
            if (sucess)
            {
                var number = Regex.Match(match.ToString(), @"[0-9]+(\.)?[0-9]*");
                waitTime = float.Parse(number.ToString());
                currentLine = _waitRegex.Replace(currentLine, "", 1);
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

        private bool CheckIfEmptyLine(string currentLine)
        {
            if (currentLine.Equals("\r"))
                return true;
            return false;
        }

        private bool CheckIfCharacterIsTalking(string currentLine)
        {
            var match = _talkingRegex.Match(currentLine);
            if (match.Success)
            {
                var split = match.ToString().Split(' ');
                var position = split[1];
                ChangePortraitScale(position);
                return true;
            }
            return false;
        }

        private void ChangePortraitScale(string position)
        {
            var pos = 0;
            var others = new List<Position>();
            switch (position)
            {
                case "left":
                    pos = (int) Position.Left;
                    others.Add(Position.Center);
                    others.Add(Position.Right);
                    break;
                case "right":
                    pos = (int) Position.Right;
                    others.Add(Position.Center);
                    others.Add(Position.Left);
                    break;
                case "center":
                    pos = (int) Position.Center;
                    others.Add(Position.Left);
                    others.Add(Position.Right);
                    break;
                default:
                    others.Add(Position.Left);
                    others.Add(Position.Right);
                    others.Add(Position.Center);
                    break;
            }
            if (_dialogPortraits[pos].enabled)
            {
                var actualScale = _dialogPortraits[pos].gameObject.transform.localScale;
                _dialogPortraits[pos].gameObject.transform.localScale = new Vector3(ScaleFactor*actualScale.x,
                    ScaleFactor*actualScale.y, actualScale.z);
            }
            foreach (var other in others)
                _dialogPortraits[(int) other].gameObject.transform.localScale = new Vector3(1, 1, 1);

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
            
            _dialogPortraits[pos].enabled = true;
            var sprite = Resources.Load<Sprite>(_visualNovelFolder + charname);

            if (sprite != null)
                _dialogPortraits[pos].sprite = sprite;
            else
                _dialogPortraits[pos].enabled = false;

        }

        public void ChangeEndAtLine(int newEndAtLine)
        {
            if (newEndAtLine == 0) //endatline not set in inspector then get all lines
                _endAtLine = _textLines.Length - 1;
            if (newEndAtLine <= _textLines.Length - 1)
                _endAtLine = newEndAtLine;
            else
                _endAtLine = _textLines.Length - 1;
        }

       
    }
}
