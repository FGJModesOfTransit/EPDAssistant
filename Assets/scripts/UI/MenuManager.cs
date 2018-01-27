using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour 
{
	[SerializeField]
	private SideMenu sideMenu;

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Escape)) 
		{
			if (sideMenu.IsOpen) 
			{
				sideMenu.Close();
			}
			else 
			{
				sideMenu.Open();
			}
		}
	}
}
