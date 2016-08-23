using System;
using UnityEngine;

namespace Application
{
	public struct StageData{
		public int ink;
		public int hp;
		public int time;

		public StageData(int ink, int hp, int time){
			this.ink = ink;
			this.hp = hp;
			this.time = time;
		}
	}
	/// <summary>
	/// Provides a point of access for Iremuzi game data.
	/// </summary>
	public class IrezumiDAO
	{
		/// <summary>
		/// The number of levels
		/// </summary>
		private static int stagesCount = 5;

		private static string kInkId = "ink";
		private static string kTimeId = "time";
		private static string kHpId = "hp";

		private IrezumiDAO _sharedInstance = null;
		private IrezumiDAO sharedInstance{
			get {
				if (_sharedInstance == null)
					_sharedInstance = new IrezumiDAO ();
				return _sharedInstance;
			}
		}

		private IrezumiDAO ()
		{
			//Set up any important data, when needed
		}

		static public StageData[] getStagesData(){
			StageData[] stages = new StageData[stagesCount];
			for (int i = 0; i < stagesCount; i++) {
				stages [i] = getStageData (i);
			}
			return stages;
		}

		static public void saveStageData(int stageId, StageData stageData){
			string id = "l" + (stageId+1)+"_";
			PlayerPrefs.SetInt (id + kInkId, stageData.ink);
			PlayerPrefs.SetInt (id + kHpId, stageData.hp);
			PlayerPrefs.GetInt (id + kTimeId, stageData.time);
		}

		static private StageData getStageData(int stageId){
			string id = "l" + (stageId+1) + "_";
			return StageData(PlayerPrefs.GetInt(id+kInkId),PlayerPrefs.GetInt(id+kHpId),PlayerPrefs.GetInt(id+kTimeId));
		}
	}
}

