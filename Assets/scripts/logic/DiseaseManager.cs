using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct DiseaseWave
{
    public float[] diseaseTimes;
}

public class DiseaseManager : MonoBehaviour
{
    private enum State
    {
        Waiting,
        InWave
    }

    public Image ImagePrefab;
    public Gradient DiseaseColor;
    public DiseaseWave[] Waves;
    public float TimeBetweenWaves;

	public static Action<Node, Disease> OnDiseaseAdded;

    private Canvas m_DiseaseCanvas;
    private int m_CurrentWave;
    private int m_DiseaseCounter;
    private float m_WaveTimer;
    private State m_State;

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

	void OnEnable()
	{
		Movement.OnMovementComplete += HandlePlayerArrivedAtNode;
	}

	void OnDisable()
	{
		Movement.OnMovementComplete -= HandlePlayerArrivedAtNode;
	}

	private void HandlePlayerArrivedAtNode(GameObject playerObject, Node node)
	{
		if (playerObject.tag == "Player") 
		{
			Disease d = node.GetComponent<Disease>();
			if ( d != null)
			{
				d.Remove();
			}
		}
	}

    private void Start()
    {
        Debug.Log("Starting game");
        m_CurrentWave = -1;
        NextWave();
    }

    private void NextWave()
    {
        m_State = State.InWave;
        ++m_CurrentWave;
        m_DiseaseCounter = 0;
        m_WaveTimer = 0f;
        if ( m_CurrentWave == Waves.Length )
        {
            MessageManager.Instance.AddMessage("Congratulations! No more disease!");
            gameObject.SetActive(false);
            return;
        }

        MessageManager.Instance.AddMessage("Starting wave " + (1+m_CurrentWave) );
    }

    private void EndWave()
    {
        MessageManager.Instance.AddMessage("Wave " + (1+m_CurrentWave) + " complete!");
        m_WaveTimer = 0f;
        m_State = State.Waiting;
    }

    public bool AddDisease()
    {
        Node n = GraphManager.Instance.GetRandomNode();
        Disease disease = n.GetComponent<Disease>();
		if (disease == null)
        {
			disease = n.gameObject.AddComponent<Disease>();

            if (OnDiseaseAdded != null)
            {
                OnDiseaseAdded(n, disease);
            }
            return true;
        }
        return false;
    }

    public void DebugHealAll()
    {
        var nodes = GraphManager.Instance.GetNodes();
        for (int i = 0; i < nodes.Length; ++i)
        {
            Disease d = nodes[i].GetComponent<Disease>();
            if ( d != null)
            {
                d.Remove();
            }
        }
    }

    public int CountSickNodes()
    {
        int count = 0;
        var nodes = GraphManager.Instance.GetNodes();
        for (int i = 0; i < nodes.Length; ++i)
        {
            if ( nodes[i].GetComponent<Disease>() != null )
            {
                count++;
            }
        }
        return count;
    }

	void Update ()
    {
        /*
        if ( Input.GetKeyDown(KeyCode.H) )
        {
            Debug.Log("Healing all");
            DebugHealAll();
        }*/

        m_WaveTimer += Time.deltaTime;

        if (m_State == State.Waiting)
        {
            if (m_WaveTimer > TimeBetweenWaves)
            {
                NextWave();
            }
        }
        else if (m_State == State.InWave)
        {
            if (m_CurrentWave < Waves.Length && m_DiseaseCounter < Waves[m_CurrentWave].diseaseTimes.Length &&  m_WaveTimer > Waves[m_CurrentWave].diseaseTimes[m_DiseaseCounter])
            {
                if (AddDisease())
                {
                    m_DiseaseCounter++;
                    m_WaveTimer = 0f;
                }
            }

            if (m_DiseaseCounter >= Waves[m_CurrentWave].diseaseTimes.Length)
            {
                if (CountSickNodes() == 0)
                {
                    EndWave();
                    NextWave();
                }
            }

        }
    }
}
