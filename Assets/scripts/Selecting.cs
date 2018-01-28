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

		if (!CameraPanAndZoom.Instance.IsMoving) 
		{
			Movement.PlayerCharacter.GetComponent<Movement>().AddTarget (n);
		}
	}
}
