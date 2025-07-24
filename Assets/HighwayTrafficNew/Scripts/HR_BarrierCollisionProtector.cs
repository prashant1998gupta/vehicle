using UnityEngine;
using System.Collections;

public class HR_BarrierCollisionProtector : MonoBehaviour {

	public CollisionSide collisionSide;
	public enum CollisionSide{Left, Right}

	private Rigidbody playerRigid;

	void OnTriggerStay (Collider col) {

		if (!col.transform.root.CompareTag ("Player"))
			return;

		if(!playerRigid)
			playerRigid = col.gameObject.GetComponentInParent<RCC_CarControllerV3>().rigid;

		if(collisionSide == CollisionSide.Right)
			playerRigid.AddForce (-Vector3.right * 50f, ForceMode.Acceleration);
		else
			playerRigid.AddForce (Vector3.right * 50f, ForceMode.Acceleration);

		playerRigid.linearVelocity = new Vector3 (0f, playerRigid.linearVelocity.y, playerRigid.linearVelocity.z);
		playerRigid.angularVelocity =  new Vector3 (playerRigid.angularVelocity.x, 0f, 0f);
		
	}

	void OnDrawGizmos(){

		Gizmos.color = new Color(1f, .5f, 0f, .75f);
		Gizmos.DrawCube(transform.position, GetComponent<BoxCollider> ().size);

	}

}
