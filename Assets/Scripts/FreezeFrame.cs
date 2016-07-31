using UnityEngine;
using System.Collections;

public class FreezeFrame : MonoBehaviour {

	public bool IsFrozen = false; //Is the game paused?
	public float FreezeFrameCooldown = 3; //How many seconds until freeze frame can happen again?
	[SerializeField]float CooldownSeconds; //How many seconds until last freeze frane?
	float PauseDuration; //Duration of pause
	float PauseTime; //When was the game paused?

	/// <summary>
	/// Pauses the game during the specified time.
	/// </summary>
	/// <param name="time">Time - time the game will be paused</param>
	public void Pause(float time){
		if (IsFrozen || CooldownSeconds < FreezeFrameCooldown) return;
		Time.timeScale = 0;
		PauseTime = Time.realtimeSinceStartup;
		IsFrozen = true;
		PauseDuration = time;
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume(){
		Time.timeScale = 1;
		IsFrozen = false;
		CooldownSeconds = 0;
	}

	/// <summary>
	/// Start this instance. Sets value of CooldownSeconds.
	/// </summary>
	public void Start(){
		CooldownSeconds = FreezeFrameCooldown;
	}

	/// <summary>
	/// Update this instance. Checks if game is paused and if it should be resumed.
	/// </summary>
	public void Update(){
		if(IsFrozen && (Time.realtimeSinceStartup > PauseTime + PauseDuration)){ //if game is paused and it's time to resume it
			Resume();
		}
		CooldownSeconds+=Time.deltaTime;
	}
}
