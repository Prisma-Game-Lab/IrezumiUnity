using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class LevelSelector : MonoBehaviour
    {
        Image spot1;
        Image spot2;
        Image spot3;

        Text ink;
        Text hp;
        Text time;

        int index;

        Dictionary<int, string> levels = new Dictionary<int, string>();

        void Start()
        {
            spot1 = GameObject.Find("Spot1").GetComponent<Image>();
            spot2 = GameObject.Find("Spot2").GetComponent<Image>();
            spot3 = GameObject.Find("Spot3").GetComponent<Image>();

            ink = GameObject.Find("Ink").GetComponent<Text>();
            hp = GameObject.Find("Hp").GetComponent<Text>();
            time = GameObject.Find("Time").GetComponent<Text>();

            index = 1;

            levels.Add(1,"TestScene");
            levels.Add(2, "Scene");
            levels.Add(3, "Level3");
            levels.Add(4, "Level4");
            levels.Add(5, "Level5");

            UpdateScene();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                index--;
                if (index < 1)
                {
                    index = 1;
                }
                else
                {
                    UpdateScene();
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                index++;
                if (index > levels.Count)
                {
                    index = levels.Count;
                }
                else
                {
                    UpdateScene();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(levels[index]);
            }
        }

        void UpdateScene()
        {
            if (index == 1)
            {
                spot1.sprite = Resources.Load("0Alpha", typeof(Sprite)) as Sprite;
                spot2.sprite = Resources.Load(levels[1], typeof(Sprite)) as Sprite;
                spot3.sprite = Resources.Load(levels[2], typeof(Sprite)) as Sprite;
            }
            else if (index == levels.Count)
            {
                spot1.sprite = Resources.Load(levels[index - 1], typeof(Sprite)) as Sprite;
                spot2.sprite = Resources.Load(levels[index], typeof(Sprite)) as Sprite;
                spot3.sprite = Resources.Load("0Alpha", typeof(Sprite)) as Sprite;
            }
            else
            {
                spot1.sprite = Resources.Load(levels[index - 1], typeof(Sprite)) as Sprite;
                spot2.sprite = Resources.Load(levels[index], typeof(Sprite)) as Sprite;
                spot3.sprite = Resources.Load(levels[index + 1], typeof(Sprite)) as Sprite;
            }

            ink.text = "Ink Coletado: " + PlayerPrefs.GetInt(levels[index] + "_Ink");
            hp.text = "Hp: " + PlayerPrefs.GetInt(levels[index] + "_Hp");
            time.text = "Tempo: " + PlayerPrefs.GetInt(levels[index] + "_Minutes") + "m " + PlayerPrefs.GetInt(levels[index] + "_Seconds") + "s";
        }
    }
}