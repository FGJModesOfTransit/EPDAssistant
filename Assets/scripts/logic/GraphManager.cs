using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public GameObject[] ConnectionPrefabs;

    public static GraphManager Instance
    {
        get
        {

            if (m_Instance == null)
            {
                GameObject obj = GameObject.Find("GraphManager");
                if (obj != null) m_Instance = obj.GetComponent<GraphManager>(); 
            }
            return m_Instance;
        }
    }
    private static GraphManager m_Instance;

    private Node[] m_Nodes;
    private Dictionary<Node, List<Connection>> m_Connections;

	void OnEnable()
	{
		DiseaseManager.OnWaveCompleted += DiseaseManager_OnWaveCompleted;
	}

	void OnDisable()
	{
		DiseaseManager.OnWaveCompleted += DiseaseManager_OnWaveCompleted;
	}

    public List<Connection> GetConnections(Node n1)
    {
		return m_Connections[n1].Where(n => n.OtherEnd(n1).gameObject.activeInHierarchy).ToList();
    }

	public Connection GetConnection(Node n1, Node n2)
	{
		Connection con = null;
			foreach (Connection c in m_Connections[n1]) {
				if (c.OtherEnd (n1) == n2) {
				con = c;
				break;
				}
			}
		return con;
	}
		
    // Called from the editor to set up a connection 
    public void CreateConnection(GameObject connObj, Node n1, Node n2, ConnectionType type)
    {
        //connObj.transform.SetParent(this.transform);
        Connection c = connObj.GetComponent<Connection>();
        c.Set(n1, n2, type);
        c.gameObject.name = "Connection " + n1.gameObject.name + " to " + n2.gameObject.name;
    }

    public List<Node> GetNeighbors(Node n)
    {
        List<Node> result = new List<Node>();
        List<Connection> conns = m_Connections[n];
        for(int i = 0; i < conns.Count; ++i)
        {
			if (conns [i].m_Node1.gameObject.activeInHierarchy && conns [i].m_Node2.gameObject.activeInHierarchy) 
			{
				result.Add(conns[i].m_Node1 == n ? conns[i].m_Node2 : conns[i].m_Node1);
			}
        }
        return result;
    }

    public Node[] GetNodes()
    {
        return m_Nodes;
    }

    public Node GetRandomNode()
    {
		var activeNodes = m_Nodes.Where (node => node.gameObject.activeInHierarchy).ToArray();
		return activeNodes[UnityEngine.Random.Range(0, m_Nodes.Length)];
    }

    void Awake()
    {
        m_Connections = new Dictionary<Node, List<Connection>>();

        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        m_Nodes = new Node[nodes.Length];

        for (int i = 0; i < nodes.Length; ++i)
        {
            m_Nodes[i] = nodes[i].GetComponent<Node>();
            if ( m_Nodes[i] == null )
            {
                Debug.LogError("Error: Node " + nodes[i].name + " has a node tag but no Node component!");
            } else
            {
                Debug.Log("Added node " + m_Nodes[i].name);
                m_Connections[m_Nodes[i]] = new List<Connection>();
            }
        }

        GameObject[] conns = GameObject.FindGameObjectsWithTag("Connection");
        int connCount = 0;
        for (int i = 0; i < conns.Length; ++i)
        {
            Connection c = conns[i].GetComponent<Connection>();
            if ( c != null )
            {
                if ( c.m_Node1 == null || c.m_Node2 == null)
                {
                    Debug.LogError("Error: Connection " + conns[i].name + " has invalid nodes!");
                } else
                {
                    ++connCount;
                    if (m_Connections.ContainsKey(c.m_Node1))
                    {
                        m_Connections[c.m_Node1].Add(c);
                    }
                    else Debug.LogError("Error: Connection " + conns[i].name + " refers to node " + c.m_Node1 + " which doesn't exist");
                    if (m_Connections.ContainsKey(c.m_Node1))
                    {
                        m_Connections[c.m_Node2].Add(c);
                    }
                    else Debug.LogError("Error: Connection " + conns[i].name + " refers to node " + c.m_Node2 + " which doesn't exist");
                }
            } else
            {
                Debug.LogError("Error: Connection " + conns[i].name + " has a connection tag but no Connection component!");
            }
        }

        Debug.Log(m_Nodes.Length + " nodes and " + connCount + " connections initialized");
    }

	void DiseaseManager_OnWaveCompleted (int wave)
	{
		foreach (var node in m_Nodes) 
		{
			if (node && !node.gameObject.activeInHierarchy && node.level <= wave) 
			{
				node.gameObject.SetActive(true);
			}
		}
	}
}
