using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class StatScreen : MonoBehaviour
    {
        public Text CollectedInk;
        public Text RemaingLife;
        public Text Time;
        
        public void SetStats(Player player, Stopwatch stopWatch)
        {
            RemaingLife.text = ((int)player.Hp).ToString();
            CollectedInk.text = player.InkCollected.ToString();
            Time.text = stopWatch.Elapsed.Minutes + ":" +stopWatch.Elapsed.Seconds;
        }

       
    }
}
