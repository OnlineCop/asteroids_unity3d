using UnityEngine;
using System.Collections;


/**
 * @brief Everything that should be able to be called from global scope.
 * @details One example of how to get the size of the field based off the main
 * camera is to test its orthographic size versus its camera aspect:
 *
 * <pre><code>
 *   Constants.screenHeight = 2.0f * mainCamera.orthographicSize;
 *   Constants.screenWidth = Constants.screenHeight * mainCamera.aspect;
 * </code></pre>
 *
 * This would give the size only if the camera used is set up correctly for
 * orthographic mode, though.
 */
static public class Constants
{
#region Screen Dimensions
	static private float _screenWidth;
	static private float _screenHeight;
	
	/** @brief Central location to get global screen width */
	static public float screenWidth
	{
		get
		{
			return _screenWidth;
		}
		
		set
		{
			if (value != _screenWidth)
			{
				_screenWidth = value;
			}
		}
	}
	
	/** @brief Central location to get global screen height */
	static public float screenHeight
	{
		get
		{
			return _screenHeight;
		}
		
		set
		{
			if (value != _screenHeight)
			{
				_screenHeight = value;
			}
		}
	}
#endregion

#region Asteroid Movement Speeds
	static private float _asteroidMoveSpeedLarge  = 15.0f;
	static private float _asteroidMoveSpeedMedium = 18.0f;
	static private float _asteroidMoveSpeedSmall  = 20.0f;
	
	/** @brief Speed of large asteroids */
	static public float asteroidMoveSpeedLarge
	{
		get
		{
			return _asteroidMoveSpeedLarge;
		}
		
		set
		{
			if (_asteroidMoveSpeedLarge != value)
			{
				_asteroidMoveSpeedLarge = value;
			}
		}
	}
	
	/** @brief Speed of medium asteroids */
	static public float asteroidMoveSpeedMedium
	{
		get
		{
			return _asteroidMoveSpeedMedium;
		}
		
		set
		{
			if (_asteroidMoveSpeedMedium != value)
			{
				_asteroidMoveSpeedMedium = value;
			}
		}
	}
	
	/** @brief Speed of small asteroids */
	static public float asteroidMoveSpeedSmall
	{
		get
		{
			return _asteroidMoveSpeedSmall;
		}
		
		set
		{
			if (_asteroidMoveSpeedSmall != value)
			{
				_asteroidMoveSpeedSmall = value;
			}
		}
	}
#endregion
}
