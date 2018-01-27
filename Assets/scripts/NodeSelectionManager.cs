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

	private Canvas nodeSelectorCanvas;

	private List<GameObject> buttonPool = new List<GameObject>();

	public GameObject GetNodeSelector(Node node)
	{
		if (nodeSelectorCanvas == null) 
		{
			nodeSelectorCanvas = GetComponent<Canvas> ();
		}

		var button =  buttonPool.FirstOrDefault (candidate => !candidate.activeInHierarchy);
	
		if (button == null) 
		{
			button = GameObject.Instantiate (selectionPrefab, nodeSelectorCanvas.transform);
			button.transform.position = node.transform.position;
			buttonPool.Add (button);
		}
		else 
		{
			button.SetActive(true);
		}

		return button;
	}

	public void DisableButton(GameObject button)
	{
		button.SetActive (false);
	}
}
