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
        progress += 0.001f;
        progress = Mathf.Min(1f, progress);
        m_Image.fillAmount = progress;
        m_Image.color = DiseaseManager.Instance.DiseaseColor.Evaluate(progress);

	}
}
