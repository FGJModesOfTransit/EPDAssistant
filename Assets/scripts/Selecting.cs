using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selecting : MonoBehaviour {

	public void OnNodeClick()
	{
		Debug.Log("Clicked node.");
		GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

		player[0].GetComponent<Movement>().AddTarget (gameObject);
	}
}
