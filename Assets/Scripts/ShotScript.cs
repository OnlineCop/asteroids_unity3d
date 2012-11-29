using UnityEngine;
using System.Collections;


/**
 * @brief Attached to bullets; handles movement and lifespan.
 * @details Bullets need to wrap around the screen just like the ship and
 * asteroids. They also need a lifespan which indicates how long they persist
 * until they simply self-destruct.
 * @author Steve
 */
public class ShotScript : EntityScript
{
	public GameObject shotOwner;        // Player's ship or UFO, should not collide with its own shot
	
	public float timeToLive;            // Seconds before shot gets auto-destroyed
	private float lifetime;             // How long this shot has been alive so far
	
	private GameControllerScript gcs;   // Handles score, scene transitions, etc.
	
	
	void Start()
	{
		GameObject world = GameObject.Find("GameController");
		if (world != null)
		{
			this.gcs = GameObject.FindWithTag("GameControllerTag").GetComponent<GameControllerScript>();
		}
		
		if (timeToLive == 0.0f)
		{
			timeToLive = 2.0f;
		}
		this.lifetime = Time.time + this.timeToLive;
		this.gcs.AddShot(gameObject);
	}
	
	
	/**
	 * @brief Destroy a shot after a fixed time so it doesn't drift forever.
	 */
	void Update()
	{
		if (Time.time > this.lifetime)
		{
			this.gcs.RemoveShot(gameObject);
			return;
		}
		
		// From EntityScript parent class
		UpdateBounds();
	}
	
	
	/**
	 * @brief Destroy a shot when it collides with an asteroid.
	 */
	void OnCollisionEnter()
	{
		this.gcs.RemoveShot(gameObject);
	}
}
