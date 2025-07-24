//----------------------------------------------
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

public class RCC_DemoVehicles : ScriptableObject {

	[System.Serializable]
	public class RCC_DemoVehicle {

		public RCC_CarControllerV3 carController;
		public bool unlocked = true;
	
	}

	public RCC_DemoVehicle[] RCCDemoVehicles;

	public RCC_CarControllerV3[] vehicles;

	#region singleton
	private static RCC_DemoVehicles instance;
	public static RCC_DemoVehicles Instance{	get{if(instance == null) instance = Resources.Load("RCC Assets/RCC_DemoVehicles") as RCC_DemoVehicles; return instance;}}
	#endregion

}
