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
		Initializing,
        Waiting,
        InWave
    }

	[SerializeField]
	private GameObject creationEffectPrefab;

	[SerializeField]
	private GameObject healingEffectPrefab;

	[SerializeField]
	private GameObject doneEffectPrefab;

	[SerializeField]
	private GameObject spreadEffectPrefab;

	[SerializeField]
	private float healProtectionTime = 10;

    public Image ImagePrefab;
    public Gradient DiseaseColor;
    public DiseaseWave[] Waves;
    public float TimeBetweenWaves;

	[SerializeField]
	private float growthSpeed = 0.0003f;
	public float GrowthSpeed { get { return growthSpeed * deseaseSpeedMultiplier; }}

    public int SpreadDelay;

	public static event Action<Node, Disease> OnDiseaseAdded;
	public static event Action<int> OnWaveCompleted;

    private Canvas m_DiseaseCanvas;
    private int m_CurrentWave;
    private int m_DiseaseCounter;
    private float m_WaveTimer;
    private State m_State;

	private List<Disease> diseases = new List<Disease>();

	private int pastInflicted = 0;

	private float deseaseSpeedMultiplier = 1;

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

    public void SpreadFrom(Node n)
    {
        List<Node> neighbors = GraphManager.Instance.GetNeighbors(n);
        Node victim = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];

		if (AddDisease (victim)) 
		{
			var effect = Instantiate (spreadEffectPrefab, n.transform.position, Quaternion.identity);
			effect.transform.LookAt (victim.transform.position, -Vector3.forward);
		}
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
				HealDisease(d);
			}
		}
	}

	private IEnumerator Start()
    {
		m_CurrentWave = -1;

		m_State = State.Initializing;

		yield return new WaitForSeconds (10);

        Debug.Log("Starting game");

        NextWave();
    }

    private void NextWave()
    {
		if (m_CurrentWave >= 0 && OnWaveCompleted != null) 
		{
			OnWaveCompleted(m_CurrentWave);
		}

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
			
		if (m_CurrentWave == 0) 
		{
			MessageManager.Instance.AddMessage ("A pathogen detected! Please advice!");
		}
		else 
		{
			MessageManager.Instance.AddMessage ("New pathogen detected! Prepare for wave " + (1 + m_CurrentWave));
		}
    }

    private void EndWave()
    {
        m_WaveTimer = 0f;
        m_State = State.Waiting;
    }

    public bool AddDisease()
    {
		Node n = GraphManager.Instance.GetRandomNode();
	
		n = GraphManager.Instance.GetRandomNode(); 

		var success = AddDisease(n);

		if (success) 
		{
			var position = n.transform.position;

			MessageManager.Instance.AddMessage("Outbreak detected!\n[Locate]",
				() => CameraPanAndZoom.Instance.GoToPoint(position));
		}

		return success;
    }

    public bool AddDisease(Node n)
    {
        if (n.Lost) return false;
        if (Movement.PlayerCharacter != null && n == Movement.PlayerCharacter.CurrentNode) return false;

		// Was just healed
		if (n.LastHealed > 0 && n.LastHealed + healProtectionTime > Time.time) return false;

        Disease disease = n.GetComponent<Disease>();
		if (disease == null)
        {
			disease = n.gameObject.AddComponent<Disease>();

            if (OnDiseaseAdded != null)
            {
                OnDiseaseAdded(n, disease);
            }

			if (creationEffectPrefab != null)
			{
				Instantiate (creationEffectPrefab, disease.transform.position, Quaternion.identity);
			}

			diseases.Add(disease);

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
				HealDisease(d);
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

	public void HealDisease(Disease disease)
	{
		if (healingEffectPrefab != null) 
		{
			Instantiate (healingEffectPrefab, disease.transform.position, Quaternion.identity);
		}

		var node = disease.GetComponentInParent<Node>();

		node.LastHealed = Time.time;

		pastInflicted += Mathf.FloorToInt(disease.progress * (float)node.CurrentPopulation);

		diseases.Remove(disease);

		disease.Remove();
	}

	public void RemoveDisease(Disease disease)
	{
		if (doneEffectPrefab != null) 
		{
			Instantiate (doneEffectPrefab, disease.transform.position, Quaternion.identity);
		}

		deseaseSpeedMultiplier *= 1.1f;

		var node = disease.GetComponentInParent<Node> ();
		pastInflicted += Mathf.FloorToInt(disease.progress * (float)node.CurrentPopulation);
		pastInflicted += node.CurrentPopulation;

		var position = disease.transform.position;
		MessageManager.Instance.AddMessage("Uncontrolled outbreak! Quarantine issued\n[Locate]",
			() => CameraPanAndZoom.Instance.GoToPoint(position));

		diseases.Remove(disease);

		disease.Remove();
	}

	public int CountCurrentInflicted()
	{
		var count = 0f;

		for(int i = 0; i < diseases.Count; i++)
		{
			if (diseases [i] == null) 
			{
				diseases.RemoveAt (i);
				i--;
			}
			else 
			{
				count += diseases[i].progress * (float)diseases[i].GetComponentInParent<Node>().CurrentPopulation;
			}
		}
		return Mathf.FloorToInt (count);
	}

	public int CountTotalInflicted()
	{
		return pastInflicted + CountCurrentInflicted();
	}
}
