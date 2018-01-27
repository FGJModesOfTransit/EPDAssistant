using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
	private GameObject selectionButton;

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
				}
				else 
				{
					NodeSelectionManager.Instance.DisableButton(selectionButton);
					selectionButton = null;
				}
			}
		}
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
