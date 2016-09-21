/* The MIT License (MIT)
 * Copyright © 2016 Pietro Ribeiro Pepe.
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
	public class LevelSelectorAnimated : MonoBehaviour
	{
		private RectTransform[] modes;
		private Vector3[] positions;
		private Vector3[] scales;
		[SerializeField] private float switchTime = 0.3f;

		private float[] maxDists = new float[5];
		private float[] maxScale = new float[5];

		private bool is_busy = false;
		private int selectedMode = 1;

		[SerializeField] private string[] levels = {
			"TestScene",
			"Scene",
			"Level3",
			"Level4",
			"Level5"
		};
		private int levelsQuant;

		private Text ink;
		private Text hp;
		private Text time;

		void Awake () {
			loadOptions();
			levelsQuant = levels.Length - 4;
			ink = GameObject.Find("Ink").GetComponent<Text>();
			hp = GameObject.Find("Hp").GetComponent<Text>();
			time = GameObject.Find("Time").GetComponent<Text>();
			updateText ();
		}

		private void loadOptions()
		{
			List<RectTransform> lM = new List<RectTransform>();
			List<Vector3> lP = new List<Vector3>();
			List<Vector3> lS = new List<Vector3>();
			RectTransform t;
			int i;
			foreach (Transform child in transform.Find("Spots"))
			{
				t = child.GetComponent<RectTransform>();
				lM.Add(t);
				lP.Add(t.localPosition);
				lS.Add(t.localScale);
			}
			modes = lM.ToArray() as RectTransform[];
			positions = lP.ToArray() as Vector3[];
			scales = lS.ToArray() as Vector3[];
			string[] aux = new string[levels.Length + 4];
			aux[0] = "0Alpha"; aux[1] = "0Alpha";
			for (i = 2; i < levels.Length+2; i++)
				aux [i] = levels [i - 2];
			aux [i++] = "0Alpha"; aux [i] = "0Alpha";
			levels = aux;
			for (i = 0; i < modes.Length; i++) {
				modes [i].GetComponent<Image> ().sprite = Resources.Load<Sprite> (levels[i]);
			}
		}

		public void changeRight(){
			if (!is_busy && selectedMode<levelsQuant) {
				is_busy = true;
				for (int i = 0; i < modes.Length-1; i++) {
					modes [i].localPosition = positions [i + 1];
					modes [i].localScale = scales [i + 1];
					maxDists [i] = Vector3.Distance (modes[i].localPosition,positions[i])/switchTime;
					maxScale [i] = Vector3.Distance (modes [i].localScale, scales [i]) / switchTime;
					modes[i].GetComponent<Image>().sprite = modes[i+1].GetComponent<Image>().sprite;
				}
				selectedMode++;
				modes[modes.Length-1].GetComponent<Image>().sprite = Resources.Load<Sprite>(levels[selectedMode+3]);
				//Debug.Log (levels [selectedMode + 3]);
				updateText ();
			}
		}

		public void changeLeft(){
			if (!is_busy && selectedMode>1) {
				is_busy = true;
				Sprite last, aux;
				last = modes [0].GetComponent<Image> ().sprite;
				for (int i = 1; i < modes.Length; i++) {
					modes [i].localPosition = positions [i-1];
					modes [i].localScale = scales [i-1];
					maxDists [i] = Vector3.Distance (modes[i].localPosition,positions[i])/switchTime;
					maxScale [i] = Vector3.Distance (modes [i].localScale, scales [i]) / switchTime;
					aux = modes [i].GetComponent<Image> ().sprite;
					modes [i].GetComponent<Image> ().sprite = last;
					last = aux;
				}
				selectedMode--;
				modes[0].GetComponent<Image>().sprite = Resources.Load<Sprite>(levels[selectedMode-1]);
				updateText ();
			}
		}

		private void updateText(){
			int index = selectedMode + 1;
			ink.text = "Ink Coletado(Best): " + PlayerPrefs.GetInt(levels[index] + "_Ink");
			hp.text = "Hp(Best): " + PlayerPrefs.GetInt(levels[index] + "_Hp");
			time.text = "Melhor Tempo: " + PlayerPrefs.GetInt(levels[index] + "_Minutes") + "m " + PlayerPrefs.GetInt(levels[index] + "_Seconds") + "s";
		}

		// Update is called once per frame
		void Update () {
			if (!is_busy) {
				if (Input.GetKey (KeyCode.Return))
					SceneManager.LoadScene (levels [selectedMode + 1]);
				else if (Input.GetKey (KeyCode.LeftArrow))
					changeLeft ();
				else if (Input.GetKey (KeyCode.RightArrow))
					changeRight ();
			}
			else{
				int count = 0;
				for (int i = 0; i < modes.Length; i++) {
					modes[i].localPosition = Vector3.MoveTowards (modes[i].localPosition,positions[i],maxDists[i]*Time.deltaTime);
					modes[i].localScale = Vector3.MoveTowards (modes[i].localScale,scales[i],maxScale[i]*Time.deltaTime);
					if (modes [i].localPosition == positions [i])
						count++;
				}
				is_busy = count != modes.Length;
			}
		}
	}
}