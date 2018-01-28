using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public struct DiseaseWave
{
    public float[] diseaseTimes;
    public string name;
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

    //public static List<List<Node>> sLostHistory;
    //public static int sTotalInfected;
    public static List<WaveResults> sWaveResults;
    public static List<List<Node>> sLostHistory;
    public static int sTotalInfected;
    public Image ImagePrefab;
    public Gradient DiseaseColor;
    public DiseaseWave[] Waves;
    public float TimeBetweenWaves;

	[SerializeField]
	private float growthSpeed = 0.0003f;
	public float GrowthSpeed { get { return growthSpeed * deseaseSpeedMultiplier; }}

    public int SpreadDelay;
    private int m_LostCount;

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
			GetComponents<AudioSource> ()[3].Play ();
		}
    }

    private static DiseaseManager m_Instance;

    void Init()
    {
        m_DiseaseCanvas = GameObject.Find("DiseaseCanvas").GetComponent<Canvas>();
        sWaveResults = new List<WaveResults>();
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

        if (m_CurrentWave == Waves.Length)
        {
            MessageManager.Instance.AddMessage("Congratulations! No more disease!");
            --m_CurrentWave;
            GameOver();
            //gameObject.SetActive(false);
            return;
        }

        if (m_CurrentWave == 0) 
		{
			MessageManager.Instance.AddMessage ("A pathogen detected! Stand by.");
		}
		else 
		{
			MessageManager.Instance.AddMessage ("New pathogen detected! Prepare for wave " + (1 + m_CurrentWave));
		}
    }

    private void SetWaveResults()
    {
        while (sWaveResults.Count <= m_CurrentWave)
        {
            sWaveResults.Add(new WaveResults());
            sWaveResults[sWaveResults.Count - 1].LostNodePopulations = new List<int>();
        }

        sWaveResults[m_CurrentWave].SavedNodePopulations = new List<int>();
        Node[] nodes = GraphManager.Instance.GetActiveNodes();
        foreach (Node n in nodes)
        {
            if (!n.Lost)
            {
                sWaveResults[m_CurrentWave].SavedNodePopulations.Add(n.CurrentPopulation);
            }
        }
        sWaveResults[m_CurrentWave].DiseaseName = Waves[m_CurrentWave].name;
        int total = CountTotalInflicted();
		int previous = m_CurrentWave > 0 ? sWaveResults.Sum(result => result.Infections) : 0;
        sWaveResults[m_CurrentWave].Infections = total - previous;
        sWaveResults[m_CurrentWave].WaveNumber = m_CurrentWave + 1;
    }

    private void EndWave()
    {
        SetWaveResults();

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

			MessageManager.Instance.AddMessage("Outbreak detected! Please advice.\n[Tap to locate]",
				() => CameraPanAndZoom.Instance.GoToPoint(position));

			GetComponents<AudioSource> ()[0].Play ();
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

		GetComponents<AudioSource> ()[2].Play ();

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
		MessageManager.Instance.AddMessage("Uncontrolled outbreak! Quarantine issued\n[Tap to locate]",
			() => CameraPanAndZoom.Instance.GoToPoint(position));

		GetComponents<AudioSource> ()[1].Play ();

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

    public string GetDiseaseName()
    {
        if ( m_State == State.InWave && m_CurrentWave >= 0 )
        {
            return Waves[m_CurrentWave].name;
        } else
        {
            return "None";
        }
    }

    public void GameOver()
    {
        SetWaveResults();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public int LostCount { get { return m_LostCount; } }

    public void AddLost(Node n)
    {
        while ( sWaveResults.Count <= m_CurrentWave )
        {
            sWaveResults.Add(new WaveResults());
            sWaveResults[sWaveResults.Count - 1].LostNodePopulations = new List<int>();
        }

        sWaveResults[m_CurrentWave].LostNodePopulations.Add(n.CurrentPopulation);

        m_LostCount++;
        int totalCount = GraphManager.Instance.GetActiveNodes().Length;
        if ( 2 * m_LostCount >= totalCount )
        {
            GameOver();
        }
    }
}
