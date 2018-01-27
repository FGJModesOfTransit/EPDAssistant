using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiseaseManager : MonoBehaviour
{
    public Image ImagePrefab;
    public Gradient DiseaseColor;

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
        Disease oldDisease = n.GetComponent<Disease>();
        if (oldDisease == null)
        {
            Debug.Log("Added disease to node " + n.name);
            n.gameObject.AddComponent<Disease>();
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
