using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selecting : MonoBehaviour {

	public Node n = null;

	public void OnNodeClick()
	{
		if (n == null) {
			Debug.LogError ("No node set for selecting");
			return;
		}
		Debug.Log("Clicked node." + n.name);
		GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

		player[0].GetComponent<Movement>().AddTarget (n);
	}
}
