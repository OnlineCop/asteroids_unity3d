using UnityEngine;
using System.Collections;


/**
 * @brief Enumeration for game scenes
 */
public enum GameState
{
	GameStart,  // Game not yet started: whow "Press space to begin" screen
	GamePlay,   // Actual game, while ship and asteroids are alive
	GameWin,    // All asteroids have been destroyed
	GameDie,    // One of player's ships destroyed
	GameOver    // Last of player's ships destroyed: show "Game over" screen
}


/**
 * @brief Imposes minor restrictions for setting game state.
 * @details When a game state change is requested, check that we are coming
 * from a valid "previous state".
 * author Steve
 */
public static class GameStateScript
{
	private static GameState _gamestate;
	
	public static GameState gameState
	{
		get
		{
			return _gamestate;
		}
		set
		{
			// Do nothing if assigning the same value
			if (_gamestate == value)
			{
				return;
			}
			
			// Validate new state
			switch (value)
			{
				case GameState.GameStart:
					if (_gamestate == GameState.GameOver)
					{
						_gamestate = value;
					}
					else
					{
						Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					}
					break;
				
				case GameState.GamePlay:
					if (_gamestate == GameState.GameStart ||
					    _gamestate == GameState.GameWin ||
					    _gamestate == GameState.GameOver)
					{
						_gamestate = value;
					}
					else
					{
						Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					}
					break;
				
				case GameState.GameWin:
					if (_gamestate == GameState.GamePlay)
					{
						_gamestate = value;
					}
					else
					{
						Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					}
					break;
				
				case GameState.GameDie:
					if (_gamestate == GameState.GamePlay)
					{
						_gamestate = value;
					}
					else
					{
						Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					}
					break;
				
				case GameState.GameOver:
					if (_gamestate == GameState.GameDie)
					{
						_gamestate = value;
					}
					else
					{
						Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					}
					break;
				
				default:
					Debug.LogError("Cannot change gamestate from " + _gamestate + " to " + value + "!");
					break;
			}
		}
	}
	
	/** 
	 * Called when a key is pressed.
	 */
	public delegate void KeyEvent(KeyCode srcKey);
	public static event KeyEvent UserInputPlay;
	
	public delegate void DestroyAsteroidEvent();
	public static event DestroyAsteroidEvent DestroyAsteroidListener;
	
	public delegate void DestroyShipEvent();
	public static event DestroyShipEvent DestroyShipListener;
	
	public static void KeyAction(KeyCode srcKey) {
		UserInputPlay(srcKey);
	}
}
