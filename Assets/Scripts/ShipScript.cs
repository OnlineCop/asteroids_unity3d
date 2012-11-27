using UnityEngine;
using System.Collections;

public class ShipScript : Entity
{
	public Object shot;
	public GameObject explosion;
	public float invulnerableTime;
	
	private float nextShotTime;
	private float invulnerableUntil;
	private const float fireInterval = 0.3f;
	
	private bool wasRotatingCCW;
	private bool wasRotatingCW;
	
	
	void Start()
	{
		this.nextShotTime = Time.time;
		this.invulnerableUntil = invulnerableTime + Time.time;
		
		// Turn off collision between ship and asteroids until you've been alive 2 seconds
		Physics.IgnoreLayerCollision(8, 9, true);
	}
	
	
	void OnGUI()
	{
		float objX = transform.position.x;
		float objY = transform.position.y;
		//float objZ = transform.position.z;
		float boundX = collider.bounds.extents.x;
		float boundY = collider.bounds.extents.y;
		
		GUI.Box(new Rect(10, 30, 300, 30),
		        string.Format("({0,1:#0.0},{1,1:#0.0})::({2,1:#0.0},{3,1:#0.0})::({4,1:#0.0},{5,1:#0.0})",
		        objX - boundX, objY - boundY,
		        objX, objY,
		        objX + boundX, objY + boundY)
		       );
	}
	
	
	void Update()
	{
		// From WrappingObjectScript parent class
		UpdateBounds();
	}
	
	
	void HideModel(bool isHidden)
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer rend in renderers)
		{
			rend.enabled = !isHidden;
		}
	}
	
	
	/**
	 * @brief Prevent damage while flashing, get user inputs.
	 */
	void FixedUpdate()
	{
		if (Time.time > invulnerableUntil)
		{
			// Layer 8: AsteroidLayer
			// Layer 9: ShipLayer
			Physics.IgnoreLayerCollision(8, 9, true);
			this.HideModel(false);
		}
		else
		{
			// Make ship flicker
			if (0 == (Mathf.RoundToInt(Time.time * 10) - Mathf.FloorToInt(Time.time * 10)))
			{
				this.HideModel(true);
			}
			else
			{
				this.HideModel(false);
			}
		}
		
		
		//////////////////////////////////////////////////////////////////////
		// User Inputs
		//////////////////////////////////////////////////////////////////////
		
		// Move ship forward, toward whatever direction it currently faces
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			// Apply a force in direction of the ship
			rigidbody.AddRelativeForce(0.0f, 8.0f, 0.0f);
			//GetComponent<ParticleRenderer>().enabled = true;
		}
		else
		{
			//GetComponent<ParticleRenderer>().enabled = false;
		}
		
		// Rotate ship CCW but do not otherwise affect forces
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			rigidbody.AddRelativeTorque(0, 0, 1.5f);
		}
		
		// Rotate ship CW but do not otherwise affect forces
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			rigidbody.AddRelativeTorque(0, 0, -1.5f);
		}
		
		// Fire a shot
		if (Input.GetKey(KeyCode.Space) && Time.time > nextShotTime)
		{
			nextShotTime = Time.time + fireInterval;
			if (GameObject.FindWithTag("Player"))
			{
				GameObject go = GameObject.FindWithTag("Player");
				Vector3 newPos = new Vector3(0.0f, 1.0f, 0.0f); // put the shot just in front of the player's ship
				newPos = go.transform.TransformPoint(newPos);
				
				GameObject projectile = Instantiate(shot, newPos, go.transform.rotation) as GameObject;
				Physics.IgnoreCollision(projectile.collider, go.collider);
				Rigidbody rb = (Rigidbody)projectile.GetComponent("Rigidbody");
				rb.velocity = go.transform.TransformDirection(Vector3.up) * 10;
			}
		}
	}
	
	
	/**
	 * @brief Handle collision of asteroid and this ship.
	 */
	void OnCollisionEnter()
	{
		Instantiate(explosion, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
