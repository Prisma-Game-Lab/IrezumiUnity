using UnityEngine;
using System.Collections;

public class FreezeFrame : MonoBehaviour {

    #region Variables
    public bool IsFrozen; //Is the game paused?
	public float FreezeFrameCooldown = 3; //How many seconds until freeze frame can happen again?

    [SerializeField]
    private float _cooldownSeconds; //How many seconds until last freeze frane?
	private float _pauseDuration; //Duration of pause
	private float _pauseTime; //When was the game paused?
    private bool _checkingTime; //Should Update check if pause time has passed?
    #endregion

    /// <summary>
    /// Pauses the game during the specified time.
    /// </summary>
    /// <param name="time">Time - time the game will be paused</param>
    public void Pause(float time){
		if (IsFrozen || _cooldownSeconds < FreezeFrameCooldown) return;
		Time.timeScale = 0;
		_pauseTime = Time.realtimeSinceStartup;
		IsFrozen = true;
		_pauseDuration = time;
	    _checkingTime = true;
	}

    public void Pause()
    {
        if (IsFrozen || _cooldownSeconds < FreezeFrameCooldown) return;
        Time.timeScale = 0;
        _pauseTime = Time.realtimeSinceStartup;
        IsFrozen = true;
    }

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume(){
		Time.timeScale = 1;
		IsFrozen = false;
		_cooldownSeconds = 0;
	}

	/// <summary>
	/// Start this instance. Sets value of CooldownSeconds.
	/// </summary>
	public void Start(){
		_cooldownSeconds = FreezeFrameCooldown;
	}

	/// <summary>
	/// Update this instance. Checks if game is paused and if it should be resumed.
	/// </summary>
	public void Update(){
		if(_checkingTime && IsFrozen && (Time.realtimeSinceStartup > _pauseTime + _pauseDuration)){ //if game is paused and it's time to resume it
			Resume();
		    _checkingTime = false;
		}
		_cooldownSeconds+=Time.deltaTime;
	}
}
