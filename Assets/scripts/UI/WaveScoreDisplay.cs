using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveScoreDisplay : MonoBehaviour 
{
	[SerializeField]
	private Text diseaseTitle;

	[SerializeField]
	private Text infectionsPenaltyLabel;

	[SerializeField]
	private Text townBaseScoreLabel;

	[SerializeField]
	private Text cityBaseScoreLabel;

	[SerializeField]
	private RectTransform townsPanel;

	[SerializeField]
	private RectTransform citiesPanel;

	[SerializeField]
	private GameObject citySymbolPrefab;

	[SerializeField]
	private GameObject townSymbolPrefab;
	private WaveResults results;

	public void Init(WaveResults results)
	{
		this.results = results;

		diseaseTitle.text = results.DiseaseName;
		if (results.Infections > 0)
		{
			infectionsPenaltyLabel.text = "Infections penalty: -" + results.Infections;
		}
		else
		{
			infectionsPenaltyLabel.text = string.Empty;
		}

		var savedTowns = results.SavedNodePopulations.Where (population => population <= 100).Count();
		var lostTowns = results.LostNodePopulations.Where (population => population <= 100).Count();
		var savedCities = results.SavedNodePopulations.Where (population => population > 100).Count();
		var lostCities = results.LostNodePopulations.Where (population => population > 100).Count();

		if (savedTowns == 0 && lostTowns > 0) 
		{
			townBaseScoreLabel.text = "x0";
		}
		if (savedTowns > 0) 
		{
			townBaseScoreLabel.text = "x" + EndScoreDisplay.townBaseScore;
		}
		else 
		{
			townBaseScoreLabel.text = string.Empty;
		}

		if (savedCities == 0 && lostCities > 0) 
		{
			cityBaseScoreLabel.text = "x0";
		}
		if (savedCities > 0) 
		{
			cityBaseScoreLabel.text = "x" + EndScoreDisplay.cityBaseScore;
		}
		else 
		{
			cityBaseScoreLabel.text = string.Empty;
		}

		for (int i = 0; i < savedTowns; i++) 
		{
			var symbol = Instantiate (townSymbolPrefab, townsPanel).GetComponent<Image> ();
			symbol.color = Color.white;
			symbol.name = "saved";
		}
		for (int i = 0; i < lostTowns; i++) 
		{
			var symbol = Instantiate (townSymbolPrefab, townsPanel).GetComponent<Image> ();
			symbol.color = Color.red;
			symbol.name = "lost";
		}

		for (int i = 0; i < savedCities; i++) 
		{
			var symbol = Instantiate (citySymbolPrefab, citiesPanel).GetComponent<Image> ();
			symbol.color = Color.white;
			symbol.name = "saved";
		}
		for (int i = 0; i < lostCities; i++) 
		{
			var symbol = Instantiate (citySymbolPrefab, citiesPanel).GetComponent<Image> ();
			symbol.color = Color.red;
			symbol.name = "lost";
		}
	}

	public void Clear()
	{
		diseaseTitle.text = "Unknown";
		infectionsPenaltyLabel.text = string.Empty;
		townBaseScoreLabel.text = string.Empty;
		cityBaseScoreLabel.text = string.Empty;
		var townChildren = townsPanel.GetComponentsInChildren<Transform> (true);
		foreach (var child in townChildren) 
		{
			Destroy (child.gameObject);
		}
		var cityChildren = citiesPanel.GetComponentsInChildren<Transform> (true);
		foreach (var child in cityChildren) 
		{
			Destroy (child.gameObject);
		}
	}

	public IEnumerator Pulsate_Coroutine()
	{
		var towns = townsPanel.GetComponentsInChildren<Image> ().Where (img => img.gameObject != townsPanel.gameObject);
		foreach (var town in towns) 
		{
			if (town.name != "lost") 
			{
				LeanTween.scale (town.gameObject, Vector3.one * 1.1f, 0.5f).setEasePunch ();
				yield return new WaitForSeconds (0.18f);
			}
		}

		var cities = citiesPanel.GetComponentsInChildren<Image> ().Where (img => img.gameObject != citiesPanel.gameObject);
		foreach (var city in cities) 
		{	
			if (city.name != "lost") 
			{
				LeanTween.scale (city.gameObject, Vector3.one * 1.15f, 0.5f).setEasePunch ();
				yield return new WaitForSeconds (0.2f);
			}
		}

		if (results.Infections > 0) 
		{
			LeanTween.textColor (infectionsPenaltyLabel.GetComponent<RectTransform> (), Color.red, 0.75f).setEaseInOutElastic ();

			yield return new WaitForSeconds (0.75f);
		}
	}
}
