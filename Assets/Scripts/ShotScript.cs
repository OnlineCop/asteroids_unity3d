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
	
	
	/**
	 * @brief Set the object's lifespan so it will self-destruct after a few seconds.
	 */
	void Start()
	{
		this.gcs = GameObject.Find("/GameController").GetComponent<GameControllerScript>();
		this.gcs.AddShot(gameObject);
		
		if (this.timeToLive != 2.0f)
		{
			this.timeToLive = 2.0f;
		}
		this.lifetime = Time.time + this.timeToLive;
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
