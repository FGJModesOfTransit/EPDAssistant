using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScoreDisplay : MonoBehaviour 
{
	[SerializeField]
	private RectTransform waveScorePanelsParent;

	[SerializeField]
	private Text totalScoreDisplay;

	[SerializeField]
	private ScrollRect waveResultsScroller;

	public const int townBaseScore = 200;
	public const int cityBaseScore = 600;

	private List<WaveScoreDisplay> waveScoreDisplays;

	int score = 0;

	void Start()
	{
		var results = MakeDummyData ();
		Init (results);
		StartCoroutine (Show_Coroutine(results));
	}

	private IEnumerator Show_Coroutine(List<WaveResults> results)
	{
		waveResultsScroller.StopMovement ();

		yield return new WaitForSeconds (1f);

		var magick = 0.211f;

		for (int i = 0; i < waveScoreDisplays.Count; i++) 
		{
			if (i < results.Count) 
			{
				yield return waveScoreDisplays [i].Pulsate_Coroutine ();

				var townsScore = results [i].SavedNodePopulations.Where (pop => pop <= 100).Count() * townBaseScore;
				var citiesScore = results [i].SavedNodePopulations.Where (pop => pop > 100).Count() * cityBaseScore;

				yield return AddScore_Coroutine (townsScore + citiesScore - results [i].Infections);
			}

			if (i + 1 < results.Count)
			{
				float normalizePosition = (float)(i + 1) * magick;
				LeanTween.value (waveResultsScroller.horizontalNormalizedPosition, normalizePosition, 0.5f)
					.setEase(LeanTweenType.easeOutBounce)
					.setOnUpdate ((scroll) => waveResultsScroller.horizontalNormalizedPosition = scroll);

				yield return new WaitForSeconds(1.5f);
			}
		}

		waveResultsScroller.horizontal = true;
	}

	private IEnumerator AddScore_Coroutine(int amount)
	{
		if (amount > 0) 
		{
			totalScoreDisplay.text = "Score " + score + " + " + amount;
		}
		else if (amount < 0)
		{
			totalScoreDisplay.text = "Score " + score + " - " + Mathf.Abs(amount);
		}

		LeanTween.scale (totalScoreDisplay.gameObject, Vector3.one * 1.1f, 0.5f).setEasePunch ();
		yield return new WaitForSeconds (0.75f);

		int newTotal = score + amount;

		LeanTween.value (score, newTotal, 1f).setEaseInOutExpo ().setOnUpdate((currentScore) => {
			var remaining = Mathf.RoundToInt(Mathf.Abs (newTotal - currentScore));
			score = Mathf.RoundToInt(currentScore);
			if (amount > 0) 
			{
				totalScoreDisplay.text = "Score " + score + " + " + remaining;
			}
			else if (amount < 0) 
			{
				totalScoreDisplay.text = "Score " + score + " - " + remaining;
			}
			else 
			{
				totalScoreDisplay.text = "Score " + score;
			}
		});

		yield return new WaitForSeconds (1.05f);

		score = newTotal;
		totalScoreDisplay.text = "Score " + score;

		LeanTween.scale (totalScoreDisplay.gameObject, Vector3.one * 1.1f, 0.5f).setEasePunch ();
		yield return new WaitForSeconds (0.5f);
	}

	private void Init(List<WaveResults> results)
	{
		waveScoreDisplays = new List<WaveScoreDisplay>(waveScorePanelsParent.GetComponentsInChildren<WaveScoreDisplay>());
		for (int i = 0; i < waveScoreDisplays.Count; i++)
		{
			if (i < results.Count) 
			{
				waveScoreDisplays[i].Init(results[i]);
			}
			else 
			{
				waveScoreDisplays[i].Clear();
			}
		}
	}

	private List<WaveResults> MakeDummyData()
	{
		var results = new List<WaveResults> () {
			new WaveResults (){
				WaveNumber = 1,
				DiseaseName = "Influenza",
				Infections = 30,
				LostNodePopulations = new List<int> (),
				SavedNodePopulations = new List<int> () {
					100,
					100
				}
			},
			new WaveResults () {
				WaveNumber = 2,
				DiseaseName = "Plague",
				Infections = 128,
				LostNodePopulations = new List<int> () { 100 },
				SavedNodePopulations = new List<int> () {
					100,
					100,
					300
				}
			},
			new WaveResults () {
				WaveNumber = 3,
				DiseaseName = "Dummy",
				Infections = 1128,
				LostNodePopulations = new List<int> () { 100, 500 },
				SavedNodePopulations = new List<int> () {
					100,
					100,
					100,
					100,
					100
				}
			},
			new WaveResults () {
				WaveNumber = 4,
				DiseaseName = "Dummy 2",
				Infections = 1328,
				LostNodePopulations = new List<int> () { 100, 100, 100, 100 },
				SavedNodePopulations = new List<int> () {
					100,
					100,
					500,
					100,
					100,
					100,
					100,
					100
				}
			}
		};

		return results;
	}
}
