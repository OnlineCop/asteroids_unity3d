using UnityEngine;
using System.Collections;


/** @brief Asteroid event delegates */
public delegate void DestroyAsteroidDelegate();


/**
 * @brief All sizes of asteroids use this same script.
 * @details The smallest asteroid will spawn no other asteroids when it is
 * destroyed, which we can detect simply by having its variable be null.
 *
 * The larger asteroids "break" into smaller ones when shot, so those asteroid
 * GameObjects need to be attached so this script knows whether or not to spawn
 * them when that happens.
 * @author Steve
 */
public class AsteroidScript : EntityScript
{
	public GameObject explosionPrefab;      // Shown when asteroid is hit/destroyed by shot
	public GameObject asteroidPrefab;       // One of the smaller asteroids to spawn when this is destroyed (set to null if none spawned)
	public uint numAsteroidsToSpawn;        // How many "pieces" this is broken into when destroyed
	public int points;                      // Increase player's score by this much when the asteroid is destroyed
	public float maxVelocity;               // Per-axis limiter (value is applied to all axis independently)
	
	private DestroyAsteroidDelegate blowItUp;
	
	private GameControllerScript gcs;            // Handles score, scene transitions, etc.
	private Vector3 rotationSpeed;
	
	
	public void AsteroidWasDestroyed()
	{
		Debug.Log("The asteroid was destroyed.");
	}
	
	
	void Start()
	{
		this.blowItUp = new DestroyAsteroidDelegate(AsteroidWasDestroyed);
		
		GameObject world = GameObject.Find("GameControllerTag");
		if (world != null)
		{
			this.gcs = world.GetComponent<GameControllerScript>();
		}
		
		if (this.numAsteroidsToSpawn == 0)
		{
			this.numAsteroidsToSpawn = 2;
		}
		
		if (this.maxVelocity == 0)
		{
			this.maxVelocity = 2.0f;
		}
	}
	
	
	/**
	 * @brief Asteroid chooses its initial direction, but not its speed.
	 */
	public void SetSpeed(float newSpeed)
	{
		Vector3 newDirection = new Vector3(
			Random.Range(-this.maxVelocity, this.maxVelocity),
			Random.Range(-this.maxVelocity, this.maxVelocity),
			0.0f
		);
		this.gameObject.rigidbody.velocity = newDirection * newSpeed;
	}
	
	
	public void SetRotSpeed(float maxRotX, float maxRotY, float maxRotZ)
	{
		this.rotationSpeed = new Vector3(
			Random.Range(-maxRotX, maxRotX),
			Random.Range(-maxRotY, maxRotY),
			Random.Range(-maxRotZ, maxRotZ)
		);
	}
	
	
	void FixedUpdate()
	{
		this.gameObject.transform.Rotate(this.rotationSpeed * Time.deltaTime, Space.Self);
		
		// From EntityScript parent class
		UpdateBounds();
	}
	
	
	/**
	 * @brief Asteroid has collided with either a shot or the player's ship.
	 */
	void OnCollisionEnter()
	{
		// Call the DestroyAsteroidDelegate delegate
		this.blowItUp();
		
		if (this.gcs != null)
		{
			this.gcs.AddPoints(points);
		}
		
		// If we attached any asteroid prefabs to 'asteroidPrefab', then the
		// current asteroid breaks apart when hit into several smaller ones.
		if (this.asteroidPrefab != null)
		{
			// Destroy the current gameObject, and create smaller asteroids in its place
			for (uint i = 0; i < this.numAsteroidsToSpawn; i++)
			{
				float startingX = transform.position.x + Random.Range(-5.0f, 5.0f);
				float startingY = transform.position.y + Random.Range(-5.0f, 5.0f);
				Vector3 pos = new Vector3(startingX, startingY, transform.position.z);
				
				GameObject newAsteroid = Instantiate(this.asteroidPrefab) as GameObject;
				newAsteroid.transform.position = pos;
				newAsteroid.name = "Asteroid" + i;
				
				// Slightly higher velocity than the original asteroid
				newAsteroid.rigidbody.velocity = this.gameObject.rigidbody.velocity * 1.5f;
				
				AsteroidScript newAsteroidScript = newAsteroid.GetComponent<AsteroidScript>();
				if (newAsteroidScript != null)
				{
					newAsteroidScript.rotationSpeed = this.rotationSpeed;
				}
				
				// Parent new asteroid to correct layer
				this.gcs.AddToAsteroidsLayer(newAsteroid);
				
				// Add new asteroid(s) into gcs's array for management
				this.gcs.AddAsteroid(asteroidPrefab);
			}
		}
		
		Instantiate(explosionPrefab, transform.position, transform.rotation);
		
		// Remove current asteroid from gcs's array
		this.gcs.RemoveAsteroid(gameObject);
	}
}
