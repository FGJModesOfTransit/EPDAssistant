using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	public GameObject currentNode_ = null;
	bool moving_ = false;

	public GraphManager graph_ = null;

	List<GameObject> route_ = new List<GameObject> ();

	List<Node> highlighted_ = new List<Node> ();

	int id = 0;
	// Use this for initialization
	void Start () {
		if (graph_ == null) {
			Debug.Log ("No graphmanager set for movement(in player)");
		}
		highlightNeighbours ();
	}

	// Update is called once per frame
	void Update () {
	}

	public void AddTarget(GameObject target)
	{
		// assumes always legal

		if (currentNode_ == null) {
			Debug.Log ("Current node not set! Set currrent node in editor.");
			return;
	  }
		route_.Add (target);
		if (!moving_) {
			moveNext ();
		} else if (route_.Count > 0 && target == route_ [route_.Count - 1]) {
			//route_.RemoveAt (route_.Count - 1);
		}
		disableHighlighted ();
		highlightNeighbours ();
	}

	void disableHighlighted()
	{
		foreach(Node n in highlighted_)
		{
			n.IsSelectable = false;
		}
	}

	void highlightNeighbours()
	{
		List<Connection> conns;
		Node highlight = currentNode_.GetComponent<Node>();
		if (route_.Count > 0) {
			highlight = route_ [route_.Count - 1].GetComponent<Node>();
		}
		conns = graph_.GetConnections (highlight);
		foreach (Connection c in conns) {
			c.OtherEnd (highlight).IsSelectable = true;
		}
	}

	void moveNext()
	{
		if (route_.Count == 0) {
			Debug.Log ("Route finished");
			moving_ = false;
			return;
		}
		moving_ = true;
		GameObject nextNode = route_ [0];
		route_.RemoveAt (0);

		//TODO: get route speed
		float routeSpeed = 1.0f;
		id = LeanTween.move(gameObject, nextNode.transform, routeSpeed).id;
		LTDescr d = LeanTween.descr( id );

		if(d!=null){ // if the tween has already finished it will return null
			// change some parameters
			d.setOnComplete( moveNext );
		}

		currentNode_ = nextNode;
	}
	/*
	Connection GetConnections(Node n1, Node n2)
	{
		
	}*/
}
