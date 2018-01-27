using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
	public static ScreenShake Instance
	{
		get
		{
			if (m_Instance == null)
			{
				var obj = FindObjectOfType<ScreenShake>();
				if (obj != null) m_Instance = obj;
			}
			return m_Instance;
		}
	}
	private static ScreenShake m_Instance;

	[SerializeField]
	private float maxShake = 1;

	[SerializeField]
	private float shakeReduction = 2f;

	private float shakeAmount = 0;

	/// <summary>
	/// Shakes the screen! Takes a value between 0 and 1, where 1 is the maximum amount of shake.
	/// </summary>
	public void Shake(float amount)
	{
		var newShake = shakeAmount + amount;

		var clamped = Mathf.Clamp(shakeAmount + amount, 0, 1);

		shakeAmount = clamped;
	}

	void Update()
	{
		shakeAmount -= Time.deltaTime * shakeReduction;

		if (shakeAmount > 0)
		{
			transform.position = new Vector3  (
				Random.Range (-shakeAmount, shakeAmount) * maxShake,
				Random.Range (-shakeAmount, shakeAmount) * maxShake,
				0
			);
		} 
		else 
		{
			shakeAmount = 0;
		}
	}
}
