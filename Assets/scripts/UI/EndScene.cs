using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour 
{
	[SerializeField]
	private Text scoreLabel;

	void Start () 
	{
		scoreLabel.text = "Wave X\nTotal Infected X";
	}

	public void Replay()
	{
		SceneManager.LoadScene (1);
	}

	public void SignOut()
	{
		Application.Quit();
	}
}
