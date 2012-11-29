using UnityEngine;
using System.Collections;


/**
 * @brief Objects that leave one edge of the screen wrap to the other side.
 * @details Wrapping objects is achieved by finding the camera's viewable area
 * and using that as the movement bounds. When the object goes off one side,
 * simply force its position to appear at the other.
 * @author Steve
 */
public class EntityScript : MonoBehaviour
{
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	
	
	/**
	 * @brief Determine the dimensions of the field by the size of the camera.
	 */
	void Awake()
	{
		Vector3 cameraWorldZ = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
		this.cameraBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, -cameraWorldZ.z));
		this.cameraTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, -cameraWorldZ.z));
		/*
		Debug.Log(string.Format("({0,2:#0.00},{1,2:#0.00} - {2,2:#0.00},{3,2:#0.00})",
		          this.cameraBottomLeft.x, this.cameraBottomLeft.y,
		          this.cameraTopRight.x, this.cameraTopRight.y));
		*/
	}
	
	
	public void UpdateBounds()
	{
		float objX = transform.position.x;
		float objY = transform.position.y;
		float objZ = transform.position.z;
		float boundX = collider.bounds.extents.x;
		float boundY = collider.bounds.extents.y;
		
		if (objX + boundX < this.cameraBottomLeft.x)
		{
			objX = this.cameraTopRight.x + boundX;
		}
		else if (objX - boundX > this.cameraTopRight.x)
		{
			objX = this.cameraBottomLeft.x - boundX;
		}
		
		if (objY + boundY < this.cameraBottomLeft.y)
		{
			objY = this.cameraTopRight.y + boundY;
		}
		else if (objY - boundY > this.cameraTopRight.y)
		{
			objY = this.cameraBottomLeft.y - boundY;
		}
		
		transform.position = new Vector3(objX, objY, objZ);
	}
}
