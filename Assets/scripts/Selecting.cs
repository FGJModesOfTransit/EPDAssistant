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

		Movement.PlayerCharacter.GetComponent<Movement>().AddTarget (n);
	}
}
