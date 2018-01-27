using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Node : MonoBehaviour
{
	private GameObject selectionButton;

	[SerializeField]
	private int initialPopulation = 100;
    private bool m_Lost;

	public int CurrentPopulation
	{ get { return initialPopulation; } }

	public int level = 0;

	public bool OnRoute = false;

	public float LastHealed { get; set; }

	private bool isSelectable;
	public bool IsSelectable
	{
		get
		{
			return isSelectable;
		}
		set
		{ 
			if (isSelectable != value) 
			{
				isSelectable = value;

				if (isSelectable) 
				{
					selectionButton = NodeSelectionManager.Instance.GetNodeSelector(this);
					selectionButton.GetComponent<Selecting> ().n = this;
				}
				else 
				{
					NodeSelectionManager.Instance.DisableButton(selectionButton);
					selectionButton = null;
				}
			}
		}
	}

    public void SetLost()
    {
		    NodeSelectionManager.Instance.ReleaseNodeSelector(this);
        m_Lost = true;
        Transform imTrs = transform.Find("NodeGraphic");
        if ( imTrs != null )
        {
            var sprite = imTrs.gameObject.GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = Color.red;
        }
    }

    public bool Lost
    {
        get
        { return m_Lost; }
    }

	void Awake()
	{
		if (level > 0) 
		{
			gameObject.SetActive(false);
		}
	}

    public void DebugListConnections()
    {
        string msg = "Connections for node " + gameObject.name;

        var conns = GraphManager.Instance.GetConnections(this);
        foreach (var conn in conns)
        {
            string otherName = (conn.m_Node1 == this ? conn.m_Node2 : conn.m_Node1).gameObject.name;
            msg += otherName + ", ";
        }
        Debug.Log(msg);

    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        Handles.Label(transform.position + 1f * Vector3.up, name + "(" + level + ")", style);
#endif
    }
}
