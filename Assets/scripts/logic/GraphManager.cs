using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = new GraphManager();
            return m_Instance;
        }
    }
    private static GraphManager m_Instance;

    //public void AddConnection(Node n1, Node n2, ConnectionType type)
    //{
        //Connection c = new Connection(n1, n2, type);
        //m_Connections.Add(c);
    //}

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
/*
    public void OnDrawGizmos()
    {

        if (m_Connections != null)
        {
            for (int i = 0; i < m_Connections.Count; ++i)
            {
                if (m_Connections[i].m_Node1 != null && m_Connections[i].m_Node2 != null)
                {
                    m_Connections[i].DrawGizmo();
                }
            }
        }
    }
    */
}
