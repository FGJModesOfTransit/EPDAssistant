using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour 
{
	[SerializeField]
	private float bounceMin = 1.4f;

	[SerializeField]
	private float bounceMax = 2.1f;

	[SerializeField]
	private float bounceTime = 0.5f;

	void OnEnable() 
	{
		BounceUp();
	}

	void OnDisable()
	{
		LeanTween.cancel(gameObject);
	}

	void BounceUp()
	{
		LeanTween.moveLocalY (gameObject, bounceMax, bounceTime)
			.setEase (LeanTweenType.easeInOutExpo)
			.setOnComplete(BounceDown);
	}

	void BounceDown()
	{
		LeanTween.moveLocalY (gameObject, bounceMin, bounceTime)
			.setEase (LeanTweenType.easeInOutExpo)
			.setOnComplete(BounceUp);
	}
}
