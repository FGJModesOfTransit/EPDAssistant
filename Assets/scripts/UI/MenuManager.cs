using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour 
{
	public static MenuManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				var obj = FindObjectOfType<MenuManager>();
				if (obj != null) m_Instance = obj;
			}
			return m_Instance;
		}
	}
	private static MenuManager m_Instance;

	[SerializeField]
	private SideMenu sideMenu;
	public SideMenu SideMenu { get { return sideMenu; } }

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
