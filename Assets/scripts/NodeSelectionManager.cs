using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Canvas))]
public class NodeSelectionManager : MonoBehaviour 
{
	public static NodeSelectionManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				var obj = FindObjectOfType<NodeSelectionManager>();
				if (obj != null) m_Instance = obj;
			}
			return m_Instance;
		}
	}
	private static NodeSelectionManager m_Instance;

	[SerializeField]
	private GameObject selectionPrefab;
	[SerializeField]
	private GameObject routePrefab;

	private Canvas nodeSelectorCanvas;

	private List<GameObject> buttonPool = new List<GameObject>();
	private List<GameObject> routePool = new List<GameObject>();

	public GameObject GetNodeSelector(Node node)
	{
		if (nodeSelectorCanvas == null) 
		{
			nodeSelectorCanvas = GetComponent<Canvas> ();
		}
		GameObject obj;
		if (node.OnRoute)
		{
			obj = routePool.FirstOrDefault (candidate => !candidate.activeInHierarchy);
		}
		else 
		{
			obj = buttonPool.FirstOrDefault (candidate => !candidate.activeInHierarchy);
		}
		if (obj == null) 
		{
			if(node.OnRoute)
			{
				obj = GameObject.Instantiate (routePrefab, nodeSelectorCanvas.transform);
				obj.transform.position = node.transform.position;
				routePool.Add (obj);
			}
			else
			{
				obj = GameObject.Instantiate (selectionPrefab, nodeSelectorCanvas.transform);
				obj.transform.position = node.transform.position;
				buttonPool.Add (obj);
			}
		} 
		else 
		{
			obj.transform.position = node.transform.position;
			obj.SetActive (true);
		}
			
		return obj;
	}

	public void ReleaseNodeSelector(Node node)
	{
		var button = buttonPool.FirstOrDefault (b => b.GetComponent<Selecting> ().n == node);
		if (button == null) 
		{
			button = routePool.FirstOrDefault (b => b.GetComponent<Selecting> ().n == node);
		}

		if (button != null) 
		{
			DisableButton (button);
		}
	}

	public void DisableButton(GameObject button)
	{
		button.SetActive (false);
	}
}
