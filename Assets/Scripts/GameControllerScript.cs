using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * @brief Handle full game control: ships, asteroids, shots, volleys.
 * @author Steve
 */
public class GameControllerScript : MonoBehaviour
{
	//////////////////////////////////////////////////////////////////////////
	// Public
	//////////////////////////////////////////////////////////////////////////
	
	/** @brief Prefab of player's ship. */
	public GameObject shipPrefab;
	
	/** @brief Prefab of largest asteroid. */
	public GameObject asteroidLargePrefab;
	
	/** @brief Prefab of medium asteroid. */
	public GameObject asteroidMediumPrefab;
	
	/** @brief Prefab of smallest asteroid. */
	public GameObject asteroidSmallPrefab;
	
	/** @brief Time between player death and next life. */
	public float respawnTime;
	
	public delegate void StateChange(GameState gameState);
	
	
	//////////////////////////////////////////////////////////////////////////
	// Private
	//////////////////////////////////////////////////////////////////////////
	
	/** @brief Instance of main camera. */
	private Camera mainCamera;
	
	/** @brief Holds all spawned objects, to keep hierarchy clean. */
	private GameObject objectsContainer;
	private GameObject guiElements;
	private GameObject guiAsteroids;
	
	/** @brief Number of volleys completed. */
	private uint currentLevel;
	
	/** @brief Number of player ships that can be destroyed before Game Over. */
	private uint numberOfLives;
	
	/** @brief Only resets on game restart, not between volleys. */
	private int score;
	
	/** @brief Array of all bullets currently in play. */
	private List<GameObject> shots;
	
	/** @brief Array of all asteroids currently in play. */
	private List<GameObject> asteroids;
	
	
	/** @brief Initialize all starting values. */
	void Awake()
	{
		this.currentLevel = 1;
		this.numberOfLives = 3;
		this.score = 0;
		this.shots = new List<GameObject>();
		this.asteroids = new List<GameObject>();
		
		GameStateScript.gameState = GameState.GameStart; // PRESS PLAY mode
		
		this.mainCamera = GameObject.FindWithTag("MainCamera").camera;
		if (this.mainCamera == null)
		{
			Debug.LogError("No camera located with tag MainCamera.");
		}
		
		Vector2 screenSize = this.GetFieldSize(this.mainCamera);
		Constants.screenWidth = screenSize.x;
		Constants.screenHeight = screenSize.y;
		Debug.Log(string.Format("({0,2:#0.00},{1,2:#0.00})",
		          -Constants.screenWidth/2.0f, -Constants.screenHeight/2.0f));
		
		// Parent container
		this.objectsContainer = new GameObject("ObjectsContainer");
		
		// Child container, holds GUI elements
		this.guiElements = new GameObject("guiElements");
		this.guiElements.transform.parent = this.objectsContainer.transform;
		this.guiElements.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
		
		// Another child container, holds asteroid elements
		this.guiAsteroids = new GameObject("guiAsteroids");
		this.guiAsteroids.transform.parent = this.objectsContainer.transform;
		this.guiAsteroids.transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
		
		this.GameStart();
	}
	
	
	/** @brief Callback for the GameStateScript.UserInputPlay event */
	void UserInputPlay(KeyCode srcKey)
	{
		if (srcKey == KeyCode.Space)
		{
			//Debug.Log("SPACE was pressed during " + GameStateScript.gameState + " state.");
		}
	}
	
	
	private GUIText guiGameStartTitle;
	private GUIText guiGameStartPlay;
	
	/**
	 * @brief Scene showed to players before an active game session.
	 * @details This should show the game title, Asteroids, have an option to
	 * PLAY GAME or see HIGH SCORES. There will be asteroids flying around
	 * randomly in the background.
	 */
	private void GameStart()
	{
		// Reusing this is okay, since each reference (attaching to a parent
		// transform, for example) retains the GameObject.
		GameObject guiGO;
		
		// Large ASTEROIDS title: GO and its component
		guiGO = new GameObject("guiAsteroidsTitle");
		guiGO.transform.parent = this.guiElements.transform;
		guiGO.transform.localPosition = new Vector3(0.5f, 0.65f, 0.0f);
		
		guiGameStartTitle = guiGO.AddComponent<GUIText>();
		guiGameStartTitle.text = "ASTEROIDS";
		guiGameStartTitle.anchor = TextAnchor.MiddleCenter;
		guiGameStartTitle.alignment = TextAlignment.Center;
		guiGameStartTitle.pixelOffset = Vector2.zero;
		guiGameStartTitle.lineSpacing = 1.0f;
		guiGameStartTitle.tabSize = 4.0f;
		guiGameStartTitle.fontSize = 50;
		guiGameStartTitle.fontStyle = FontStyle.Bold;
		
		// Smaller "Press SPACE" text: GO and its component
		guiGO = new GameObject("guiPressSpace");
		guiGO.transform.parent = this.guiElements.transform;
		guiGO.transform.localPosition = new Vector3(0.5f, 0.4f, 0.0f);
		
		guiGameStartPlay = guiGO.AddComponent<GUIText>();
		guiGameStartPlay.text = "Press SPACE to begin";
		guiGameStartPlay.anchor = TextAnchor.MiddleCenter;
		guiGameStartPlay.alignment = TextAlignment.Center;
		guiGameStartPlay.pixelOffset = Vector2.zero;
		guiGameStartPlay.lineSpacing = 1.0f;
		guiGameStartPlay.tabSize = 4.0f;
		guiGameStartPlay.fontSize = 20;
		guiGameStartPlay.fontStyle = FontStyle.Normal;
		
		// Bunch of random asteroids flying around. These will not be used in
		// the actual game; they are just austhetics while we wait for the
		// player to hit SPACE.
		
		int numAsteroids = 30;
		
		// All asteroids here will be set to increasing z-depths so they don't
		// look funky as they overlap each other.
		float startingZ = 0.0f;
		for (; startingZ < (float)(numAsteroids / 3.0f); startingZ += 1.0f)
		{
			float startingX = (Random.value - 0.5f) * Constants.screenWidth;
			float startingY = (Random.value - 0.5f) * Constants.screenHeight;
			Vector3 pos = new Vector3(startingX, startingY, startingZ);
			
			GameObject newAsteroid = Instantiate(this.asteroidLargePrefab) as GameObject;
			newAsteroid.transform.parent = this.guiAsteroids.transform;
			newAsteroid.transform.localPosition = pos;
			newAsteroid.name = "LargeAsteroid" + Mathf.Floor(startingZ);
			
			AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
			if (newAsteroidScript != null)
			{
				newAsteroidScript.SetSpeed((float)this.currentLevel * 2.0f);
				newAsteroidScript.SetRotSpeed(60.0f, 60.0f, 60.0f);
			}
			
			this.AddAsteroid(newAsteroid);
		}
		
		// Resume at previous z-depth
		for (; startingZ < (float)(numAsteroids * 2.0f / 3.0f); startingZ += 1.0f)
		{
			float startingX = (Random.value - 0.5f) * Constants.screenWidth;
			float startingY = (Random.value - 0.5f) * Constants.screenHeight;
			Vector3 pos = new Vector3(startingX, startingY, startingZ);
			
			GameObject newAsteroid = Instantiate(this.asteroidMediumPrefab) as GameObject;
			newAsteroid.transform.parent = this.guiAsteroids.transform;
			newAsteroid.transform.localPosition = pos;
			newAsteroid.name = "MediumAsteroid" + Mathf.Floor(startingZ);
			
			AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
			if (newAsteroidScript != null)
			{
				newAsteroidScript.SetSpeed((float)this.currentLevel * 2.0f);
				newAsteroidScript.SetRotSpeed(60.0f, 60.0f, 60.0f);
			}
			
			this.AddAsteroid(newAsteroid);
		}
		
		// Keep going at previous z-depth
		for (; startingZ < (float)numAsteroids; startingZ += 1.0f)
		{
			float startingX = (Random.value - 0.5f) * Constants.screenWidth;
			float startingY = (Random.value - 0.5f) * Constants.screenHeight;
			Vector3 pos = new Vector3(startingX, startingY, startingZ);
			
			GameObject newAsteroid = Instantiate(this.asteroidSmallPrefab) as GameObject;
			newAsteroid.transform.parent = this.guiAsteroids.transform;
			newAsteroid.transform.localPosition = pos;
			newAsteroid.name = "SmallAsteroid" + Mathf.Floor(startingZ);
			
			AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
			if (newAsteroidScript != null)
			{
				newAsteroidScript.SetSpeed((float)this.currentLevel * 2.0f);
				newAsteroidScript.SetRotSpeed(60.0f, 60.0f, 60.0f);
			}
			
			this.AddAsteroid(newAsteroid);
		}
		GameStateScript.UserInputPlay += UserInputPlay;
	}
	
	
	/**
	 * @brief Remove scene shown to players before an active game session.
	 * @details This should clean up the gui text and background asteroids
	 * before we move onto creating the player's ship, game asteroids, etc.
	 */
	private void GameStartCleanup()
	{
		Debug.Log("GameStartCleanup called");
		int kids = this.guiElements.transform.GetChildCount();
		for (int kid = 0; kid < kids; kid++)
		{
			Destroy( this.guiElements.transform.GetChild(kid).gameObject );
		}
		this.RemoveAllAsteroids();
	}
	
	
	private Vector2 GetFieldSize(Camera cam)
	{
		Vector3 cameraWorldZ = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
		Vector3 pointBL = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, -cameraWorldZ.z));
		Vector3 pointTR = cam.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, -cameraWorldZ.z));
		return new Vector2(pointTR.x - pointBL.x, pointTR.y - pointBL.y);
	}
	
	
	/** @brief GameState transitioned from GameStart to GamePlay. */
	void BeginPlay()
	{
		this.SpawnShip(this.shipPrefab);
		
		// Spawn large asteroids
		for (uint i = 0; i < this.currentLevel * 3; i++)
		{
			float startingX = (Random.value - 0.5f) * Constants.screenWidth;
			float startingY = (Random.value - 0.5f) * Constants.screenHeight;
			Vector3 pos = new Vector3(startingX, startingY, 0.0f);
			
			GameObject newAsteroid = Instantiate(this.asteroidLargePrefab) as GameObject;
			newAsteroid.transform.parent = this.guiAsteroids.transform;
			newAsteroid.transform.localPosition = pos;
			newAsteroid.name = "LargeAsteroid" + i.ToString();
			
			AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
			if (newAsteroidScript != null)
			{
				newAsteroidScript.SetSpeed((float)this.currentLevel * 2.0f);
				newAsteroidScript.SetRotSpeed(60.0f, 60.0f, 60.0f);
			}
			
			this.AddAsteroid(newAsteroid);
		}
		
		// Spawn medium asteroids
		for (uint i = 0; i < this.currentLevel * 3; i++)
		{
			float startingX = (Random.value - 0.5f) * Constants.screenWidth;
			float startingY = (Random.value - 0.5f) * Constants.screenHeight;
			Vector3 pos = new Vector3(startingX, startingY, 0.0f);
			
			GameObject newAsteroid = Instantiate(this.asteroidMediumPrefab) as GameObject;
			newAsteroid.transform.parent = this.guiAsteroids.transform;
			newAsteroid.transform.localPosition = pos;
			newAsteroid.name = "MediumAsteroid" + i.ToString();
			
			AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
			if (newAsteroidScript != null)
			{
				newAsteroidScript.SetSpeed((float)this.currentLevel * 2.0f);
				newAsteroidScript.SetRotSpeed(60.0f, 60.0f, 60.0f);
			}
			
			this.AddAsteroid(newAsteroid);
		}
		
		GameStateScript.gameState = GameState.GamePlay;
	}
	
	
	void HideGroup(string groupName, bool isHidden)
	{
		GameObject[] guiObjects = GameObject.FindGameObjectsWithTag(groupName);
		foreach (GameObject guiObject in guiObjects)
		{
			Renderer[] renderers = guiObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer rend in renderers)
			{
				rend.enabled = !isHidden;
			}
		}
	}
	
	
	void OnGUI()
	{
		List<KeyCode> KeyList = new List<KeyCode>();
		KeyList.Add (KeyCode.Space);
		KeyList.Add (KeyCode.A);
		KeyList.Add (KeyCode.S);
		KeyList.Add (KeyCode.W);
		KeyList.Add (KeyCode.D);
		foreach (KeyCode tmpKey in KeyList) {
			if (Input.GetKey(tmpKey)) {
				GameStateScript.KeyAction(tmpKey);
			}
		}
		
		GUI.Box(new Rect(10.0f, 10.0f, 100.0f, 30.0f), "Score: " + score);
		string livesString = "Lives: ";
		for (int i = 0; i < numberOfLives; ++i)
		{
			livesString += "A "; // These look like little ships :P
		}
		GUI.Box(new Rect(10.0f, Screen.height - 30.0f, 100.0f, 30.0f), livesString);
	}
	
	
	void Update()
	{
		switch (GameStateScript.gameState)
		{
		case GameState.GameStart:
			// When the player presses the key, clear the playing field
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("SPACE was pressed during " + GameStateScript.gameState + " state.");
				this.GameStartCleanup();
			}
			
			// When the player releases the key, start the next phase
			if (Input.GetKeyUp(KeyCode.Space))
			{
				this.BeginPlay();
			}
			break;
			
		case GameState.GamePlay:
			// While key held down, auto-fire
			if (Input.GetKey(KeyCode.Space))
			{
				//Debug.Log("Fire! Fire! Fire some more!");
			}
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				//Debug.Log("Thrusters! Faster, faster, faster!");
			}
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				//Debug.Log("Turn to port! That means left. Left!");
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				//Debug.Log("Hard to starboard! Right? Right!");
			}
			break;
			
		case GameState.GameOver:
			// Laugh at player and make them wait before they return to the start screen
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("Yeah... let's start over now.");
			}
			break;
		}
	}
	
	
	public bool SpawnShip(GameObject newShipPrefab)
	{
		ShipScript ship = newShipPrefab.GetComponent<ShipScript>();
		if (ship != null)
		{
			
			Debug.Log("Added " + newShipPrefab);
			GameObject newShip = Instantiate(newShipPrefab) as GameObject;
			newShip.transform.parent = this.guiAsteroids.transform;
			return true;
		}
		else
		{
			// 'newShip' is null or isn't a ShipScript object
			Debug.Log("Could not add " + newShipPrefab);
			return false;
		}
	}
	
	
	public void AddPoints(int points)
	{
		this.score += points * (int)this.currentLevel;
		if (this.score < 0)
		{
			this.score = 0;
		}
	}
	
	
	public bool AddShot(GameObject newShot)
	{
		// Ensure that the GameObject is actually a shot by detecting the
		// presence of the ShotScript component
		//
		ShotScript shot = newShot.GetComponent<ShotScript>();
		if (shot != null)
		{
			shots.Add(newShot);
			//Debug.Log("Added " + newShot);
			return true;
		}
		else
		{
			// 'newShot' is null or isn't a ShotScript object
			//Debug.Log("Could not add " + newShot);
			return false;
		}
	}
	
	
	public bool RemoveShot(GameObject shotToKill)
	{
		// Ensure that the GameObject is actually a shot by detecting the
		// presence of the ShotScript component
		//
		ShotScript shot = shotToKill.GetComponent<ShotScript>();
		if (shot != null)
		{
			if (shots.Remove(shotToKill) == true)
			{
				// Item was removed. Now destroy it.
				//Debug.Log("Destroyed " + shotToKill);
				Destroy(shotToKill);
				return true;
			}
			else
			{
				//Debug.Log("Could not find " + shotToKill + " to destroy.");
			}
		}
		return false;
	}
	
	
	public void AddAsteroid(GameObject asteroidObj)
	{
		if (asteroidObj == null)
		{
			Debug.Log("Could not add asteroid: asteroid was null.");
			return;
		}
		
		// Ensure that it's actually an asteroid by the presence of the AsteroidScript component
		AsteroidScript asteroidObjScript = asteroidObj.GetComponent<AsteroidScript>();
		if (asteroidObjScript != null)
		{
			this.asteroids.Add(asteroidObj);
		}
		else
		{
			Debug.Log("Object is not an asteroid.");
		}
	}
	
	
	public void RemoveAsteroid(GameObject asteroidInstance)
	{
		this.asteroids.Remove(asteroidInstance);
	}
	
	
	public void RemoveAllAsteroids()
	{
		Debug.Log("RemoveAllAsteroids called");
		foreach (GameObject asteroid in this.asteroids)
		{
			Destroy( asteroid );
		}
	}
	
	
	public bool AddToObjectsLayer(GameObject obj)
	{
		if (obj != null)
		{
			obj.transform.parent = this.objectsContainer.transform;
			return true;
		}
		return false;
	}
	
	
	public bool AddToElementsLayer(GameObject obj)
	{
		if (obj != null)
		{
			obj.transform.parent = this.guiElements.transform;
			return true;
		}
		return false;
	}
	
	
	public bool AddToAsteroidsLayer(GameObject obj)
	{
		if (obj != null)
		{
			obj.transform.parent = this.guiAsteroids.transform;
			return true;
		}
		return false;
	}
	
}
