using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HR_TrafficPooling : MonoBehaviour {

	#region SINGLETON PATTERN
	public static HR_TrafficPooling instance;
	public static HR_TrafficPooling Instance{
		get{
			if(instance == null)
				instance = GameObject.FindObjectOfType<HR_TrafficPooling>();
			return instance;
		}
	}
	#endregion

	private Transform reference;

	public Transform[] lines;

	private bool animateNow;

	public TrafficCars[] trafficCars;

    public float spawnBehindDistance = 150f;
    public float spawnAheadDistance = 250f;

    [System.Serializable]
	public class TrafficCars{
		public GameObject trafficCar;
		public int frequence = 1;
	}
	
	private List<HR_TrafficCar> _trafficCars = new List<HR_TrafficCar>();

	void Start () {

		reference = GameObject.FindWithTag("Player").transform;
		CreateTraffic();
        StartCoroutine(WaitForGameStart());

    }

	void Update(){

		if(animateNow)
			AnimateTraffic();

	}

    IEnumerator WaitForGameStart()
    {
        yield return new WaitForSeconds(4);
        animateNow = true;

    }

    void CreateTraffic () {
		
		for (int i = 0; i < trafficCars.Length; i++) {

			for (int k = 0; k < trafficCars[i].frequence; k++) {
				
//				GameObject go = (GameObject)GameObject.Instantiate(trafficCars[i].trafficCar, trafficCars[i].trafficCar.transform.position, trafficCars[i].trafficCar.transform.rotation);
				GameObject go = (GameObject)GameObject.Instantiate(trafficCars[i].trafficCar, Vector3.zero, Quaternion.identity);
				_trafficCars.Add(go.GetComponent<HR_TrafficCar>());
				go.SetActive(false);

			}

		}
		
	}

    /*void AnimateTraffic () {
		
		for (int i = 0; i < _trafficCars.Count; i++) {
			
			if(reference.transform.position.z > (_trafficCars[i].transform.position.z + 15) || reference.transform.position.z < (_trafficCars[i].transform.position.z - (325)))
				ReAlignTraffic(_trafficCars[i]);
			
		}
		
	}*/

    void AnimateTraffic()
    {
        float playerZ = reference.position.z;

        for (int i = 0; i < _trafficCars.Count; i++)
        {
            float carZ = _trafficCars[i].transform.position.z;

            if (carZ > playerZ + spawnAheadDistance || carZ < playerZ - spawnBehindDistance)
            {
                ReAlignTraffic(_trafficCars[i]);
            }
        }
    }

    void ReAlignTraffic1(HR_TrafficCar realignableObject){

		if(!realignableObject.gameObject.activeSelf)
			realignableObject.gameObject.SetActive(true);

		int randomLine = Random.Range(0, lines.Length );

		realignableObject.currentLine = randomLine;
		//realignableObject.transform.position = new Vector3(lines[randomLine].position.x, lines[randomLine].position.y, (reference.transform.position.z + (Random.Range(100, 300))));

        // Define traffic spawn range relative to player
        float minDistanceBehind = -150f; // how far behind player traffic can spawn
        float maxDistanceAhead = 250f;   // how far ahead player traffic can spawn

        float randomZ = reference.position.z + Random.Range(minDistanceBehind, maxDistanceAhead);

        realignableObject.transform.position = new Vector3(
            lines[randomLine].position.x,
            lines[randomLine].position.y,
            randomZ
        );


        realignableObject.transform.rotation = Quaternion.identity;

		/*switch(HR_GamePlayHandler.Instance.mode){

		case(HR_GamePlayHandler.Mode.OneWay):
				realignableObject.transform.rotation = Quaternion.identity;
				break;
		case(HR_GamePlayHandler.Mode.TwoWay):
			if(realignableObject.transform.position.x <= 0f)
				realignableObject.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 180f, 0f);
			else
				realignableObject.transform.rotation = Quaternion.identity;
			break;
		case(HR_GamePlayHandler.Mode.TimeAttack):
			realignableObject.transform.rotation = Quaternion.identity;
			break;
		case(HR_GamePlayHandler.Mode.Bomb):
			realignableObject.transform.rotation = Quaternion.identity;
			break;

		}*/

		realignableObject.SendMessage("OnReAligned");

		if(CheckIfClipping(realignableObject.triggerCollider))
			realignableObject.gameObject.SetActive(false);

	}

    void ReAlignTraffic(HR_TrafficCar realignableObject)
    {
        if (!realignableObject.gameObject.activeSelf)
            realignableObject.gameObject.SetActive(true);

        int randomLine = Random.Range(0, lines.Length);
        realignableObject.currentLine = randomLine;

        float playerZ = reference.position.z;
        float randomZ;

        // 50/50 chance
        //if (Random.value < 0.5f)
        {
            // Spawn Ahead
            randomZ = playerZ + Random.Range(150f, spawnAheadDistance);
        }
       /* else
        {
            // Spawn Behind
            randomZ = playerZ - Random.Range(20f, spawnBehindDistance);
        }*/

        realignableObject.transform.position = new Vector3(
            lines[randomLine].position.x,
            lines[randomLine].position.y,
            randomZ
        );

        realignableObject.transform.rotation = Quaternion.identity;

        realignableObject.SendMessage("OnReAligned");

        if (CheckIfClipping(realignableObject.triggerCollider))
            realignableObject.gameObject.SetActive(false);
    }


    bool CheckIfClipping(BoxCollider trafficCarBound){

		for (int i = 0; i < _trafficCars.Count; i++) {

			if(!trafficCarBound.transform.IsChildOf(_trafficCars[i].transform) && _trafficCars[i].gameObject.activeSelf){
				
				if(HR_BoundsExtension.ContainBounds(trafficCarBound.transform, trafficCarBound.bounds, _trafficCars[i].triggerCollider.bounds))
					return true;

			}
			
		}

		return false;

	}

}
