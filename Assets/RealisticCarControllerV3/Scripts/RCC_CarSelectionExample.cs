﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2021 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple example manager for how the car selection scene should work. 
/// </summary>
public class RCC_CarSelectionExample : MonoBehaviour {

	private List<RCC_CarControllerV3> _spawnedVehicles = new List<RCC_CarControllerV3> ();		// Our spawned vehicle list. No need to instantiate same vehicles over and over again. 

	public Transform spawnPosition;		// Spawn transform.
	public int selectedIndex = 0;			// Selected vehicle index. Next and previous buttons are affecting this value.

	public RCC_Camera RCCCamera;		// Enabling / disabling camera selection script on RCC Camera if choosen.
	public string nextScene;

	void Start () {

		//	Getting RCC Camera.
		if(!RCCCamera)
			RCCCamera = GameObject.FindObjectOfType<RCC_Camera> ();

		// First, we are instantiating all vehicles and store them in _spawnedVehicles list.
		CreateVehicles ();
		
	}

	private void CreateVehicles(){

		for (int i = 0; i < RCC_DemoVehicles.Instance.vehicles.Length; i++) {

			// Spawning the vehicle with no controllable, no player, and engine off. We don't want to let player control the vehicle while in selection menu.
			RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC (RCC_DemoVehicles.Instance.vehicles[i], spawnPosition.position, spawnPosition.rotation, false, false, false);

			// Disabling spawned vehicle. 
			spawnedVehicle.gameObject.SetActive (false);

			// Adding and storing it in _spawnedVehicles list.
			_spawnedVehicles.Add (spawnedVehicle);

		}

		SpawnVehicle ();

		// If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
		if (RCCCamera) {

			if (RCCCamera.GetComponent<RCC_CameraCarSelection> ())
				RCCCamera.GetComponent<RCC_CameraCarSelection> ().enabled = true;

		}

	}
	
	// Increasing selected index, disabling all other vehicles, enabling current selected vehicle.
	public void NextVehicle () {

		selectedIndex++;

		// If index exceeds maximum, return to 0.
		if (selectedIndex > _spawnedVehicles.Count - 1)
			selectedIndex = 0;

		SpawnVehicle ();
		
	}

	// Decreasing selected index, disabling all other vehicles, enabling current selected vehicle.
	public void PreviousVehicle () {

		selectedIndex--;

		// If index is below 0, return to maximum.
		if (selectedIndex < 0)
			selectedIndex = _spawnedVehicles.Count - 1;

		SpawnVehicle ();

	}

	// Spawns the current selected vehicle.
	public void SpawnVehicle(){

		// Disabling all vehicles.
		for (int i = 0; i < _spawnedVehicles.Count; i++)
			_spawnedVehicles [i].gameObject.SetActive (false);

		// And enabling only selected vehicle.
		_spawnedVehicles [selectedIndex].gameObject.SetActive (true);

//		RCC_SceneManager.Instance.RegisterPlayer (_spawnedVehicles [selectedIndex], false, false);
		RCC_SceneManager.Instance.activePlayerVehicle = _spawnedVehicles [selectedIndex];

	}

	// Registering the spawned vehicle as player vehicle, enabling controllable.
	public void SelectVehicle(){

		// Registers the vehicle as player vehicle.
		RCC.RegisterPlayerVehicle (_spawnedVehicles[selectedIndex]);

		// Starts engine and enabling controllable when selected.
		_spawnedVehicles [selectedIndex].StartEngine ();
		_spawnedVehicles [selectedIndex].SetCanControl(true);

		// Save the selected vehicle for instantianting it on next scene.
		PlayerPrefs.SetInt ("SelectedRCCVehicle", selectedIndex);

		// If RCC Camera is choosen, it will disable RCC_CameraCarSelection script. This script was used for orbiting camera.
		if (RCCCamera) {

			if (RCCCamera.GetComponent<RCC_CameraCarSelection> ())
				RCCCamera.GetComponent<RCC_CameraCarSelection> ().enabled = false;

		}

		if(nextScene != "")
			OpenScene ();

	}

	// Deactivates selected vehicle and returns to the car selection.
	public void DeSelectVehicle(){

		// De-registers the vehicle.
		RCC.DeRegisterPlayerVehicle ();

		// Resets position and rotation.
		_spawnedVehicles [selectedIndex].transform.position = spawnPosition.position;
		_spawnedVehicles [selectedIndex].transform.rotation = spawnPosition.rotation;

		// Kills engine and disables controllable.
		_spawnedVehicles [selectedIndex].KillEngine ();
		_spawnedVehicles [selectedIndex].SetCanControl(false);

		// Resets the velocity of the vehicle.
		_spawnedVehicles [selectedIndex].GetComponent<Rigidbody> ().ResetInertiaTensor ();
		_spawnedVehicles [selectedIndex].GetComponent<Rigidbody> ().linearVelocity = Vector3.zero;
		_spawnedVehicles [selectedIndex].GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		// If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
		if (RCCCamera) {
			
			if (RCCCamera.GetComponent<RCC_CameraCarSelection> ())
				RCCCamera.GetComponent<RCC_CameraCarSelection> ().enabled = true;
			
		}

	}

	public void OpenScene(){

		//	Loads next scene.
		SceneManager.LoadScene (nextScene);

	}

}
