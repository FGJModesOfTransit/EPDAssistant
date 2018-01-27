using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public Connection ConnectionPrefab;

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

    public void CreateConnection(Node n1, Node n2, ConnectionType type)
    {
        Connection c = Instantiate<Connection>(ConnectionPrefab, this.transform);
        c.Set(n1, n2, type);
        c.gameObject.name = "Connection " + n1.gameObject.name + " to " + n2.gameObject.name;
    }


    void Start()
    {

    }

    void Update()
    {

    }
}
