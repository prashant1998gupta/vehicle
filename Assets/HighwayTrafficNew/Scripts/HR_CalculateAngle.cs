//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2021 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HR_CalculateAngle : MonoBehaviour{

	public static float CalculateAngle(Quaternion transformA, Quaternion transformB){

		var forwardA = transformA * Vector3.forward;
		var forwardB = transformB * Vector3.forward;

		float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
		float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

		var angleDiff = Mathf.DeltaAngle( angleA, angleB );

		return angleDiff;

	}

	/// <summary>
	/// Calculates the approach angle of an object towrads another object
	/// </summary>
	/// <param name="forward"></param>
	/// <param name="targetDirection"></param>
	/// <param name="up"></param>
	/// <returns></returns>
	public static float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
	{
		// Calculate the approach angle
		float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);

		// If the angle is higher than 0, we approach from the left ( so we must rotate right )
		if (approachAngle > 0f)
		{
			return 1f;
		}
		else if (approachAngle < 0f) //Otherwise, if the angle is lower than 0, we approach from the right ( so we must rotate left )
		{
			return -1f;
		}
		else // Otherwise, we are within the angle range so we don't need to rotate
		{
			return 0f;
		}
	}

}
