using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct DiseaseWave
{
    public int count;
}

public class DiseaseManager : MonoBehaviour
{
    public Image ImagePrefab;
    public Gradient DiseaseColor;
    public DiseaseWave Waves;

	public static Action<Node, Disease> OnDiseaseAdded;

    private Canvas m_DiseaseCanvas;

    public static DiseaseManager Instance
    {
        get
        {

            if (m_Instance == null)
            {
                GameObject obj = GameObject.Find("DiseaseManager");
                if (obj != null) m_Instance = obj.GetComponent<DiseaseManager>();
                if (m_Instance != null) m_Instance.Init();
            }
            return m_Instance;
        }
    }

    public Image CreateDiseaseImage()
    {
        Image i = Instantiate<Image>(ImagePrefab, m_DiseaseCanvas.transform);
        return i;
    }

    private static DiseaseManager m_Instance;

    void Init()
    {
        m_DiseaseCanvas = GameObject.Find("DiseaseCanvas").GetComponent<Canvas>();
	}

    public void AddDisease()
    {
        Node n = GraphManager.Instance.GetRandomNode();
        Disease disease = n.GetComponent<Disease>();
		if (disease == null)
        {
            Debug.Log("Added disease to node " + n.name);
			disease = n.gameObject.AddComponent<Disease>();
        }
		if (OnDiseaseAdded != null) 
		{
			OnDiseaseAdded (n, disease);
		}
    }

	void Update ()
    {
        if ( UnityEngine.Random.Range(0, 100) == 0)
        {
            AddDisease();
        }
		
	}
}
