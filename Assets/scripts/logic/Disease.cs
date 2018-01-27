using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Disease : MonoBehaviour
{
    [Range(0f, 1f)]
    public float progress;

    private Image m_Image;

    public void Remove()
    {
        Destroy(m_Image.gameObject);
        Destroy(this);
    }

    void Start ()
    {
        m_Image = DiseaseManager.Instance.CreateDiseaseImage();
        m_Image.transform.position = transform.position;
	}
	
	void Update ()
    {
        progress += DiseaseManager.Instance.GrowthSpeed;
        progress = Mathf.Min(1f, progress);
        m_Image.fillAmount = progress;
        m_Image.color = DiseaseManager.Instance.DiseaseColor.Evaluate(progress);

        int rndvalue = Random.Range(0, DiseaseManager.Instance.SpreadDelay);
        if ( rndvalue == 0)
        {
            float val = 1f - Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 3f);
            //Debug.Log(val + " vs " + progress);
            if (val < progress)
            {
                DiseaseManager.Instance.SpreadFrom(GetComponent<Node>());
            }
        }

		if (progress >= 1) 
		{
			ScreenShake.Instance.Shake (0.5f);
			DiseaseManager.Instance.RemoveDisease(this);
		}
	}
}
