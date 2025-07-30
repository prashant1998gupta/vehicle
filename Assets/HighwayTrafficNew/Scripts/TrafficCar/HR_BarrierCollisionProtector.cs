using UnityEngine;
using System.Collections;

public class HR_BarrierCollisionProtector : MonoBehaviour {

	public CollisionSide collisionSide;
	public enum CollisionSide{Left, Right}


	void OnTriggerStay (Collider col) {

		
		
	}

	void OnDrawGizmos(){

		Gizmos.color = new Color(1f, .5f, 0f, .75f);
		Gizmos.DrawCube(transform.position, GetComponent<BoxCollider> ().size);

	}

}
