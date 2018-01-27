using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour 
{
	public static event System.Action<GameObject, Node> OnMovementComplete;

	public static Movement PlayerCharacter { get; private set; }

	public Node CurrentNode { get { return currentNode_; }}
	private Node currentNode_ = null;
    private Node prevNode_ = null;
    
	bool moving_ = false;

	GraphManager graph_ = null;

	List<Node> route_ = new List<Node> ();

	List<Node> highlighted_ = new List<Node> ();

	int id = 0;

	void OnEnable()
	{
		PlayerCharacter = this;
		DiseaseManager.OnWaveCompleted += DiseaseManager_OnWaveCompleted;
	}

	void OnDisable()
	{
		DiseaseManager.OnWaveCompleted -= DiseaseManager_OnWaveCompleted;
	}

	void Start () 
	{
		graph_ = GameObject.Find ("GraphManager").GetComponent<GraphManager> ();
		currentNode_ = graph_.GetRandomNode ();
		transform.position = currentNode_.transform.position;

		Debug.Log ("Set starting position to: " + currentNode_.name, currentNode_);

		highlightNeighbours ();
	}
		
	public void AddTarget(Node target)
	{
		// assumes always legal
		Debug.Log("Adding a new target");

		if (currentNode_ == null) {
			Debug.LogError ("Current node not set! Set currrent node in editor.");
			return;
	  }

		if (target.OnRoute) {
			Debug.Log ("Deleting route");
			//check if on route and delete route from that point onwards
			if (target == currentNode_) {
				Debug.Log ("Its the curren node");
				if (route_.Count > 0) {
					GraphManager.Instance.GetConnection (currentNode_, 
						route_ [0]).SetOnPath (false, false);
					if (route_.Count > 1) {
						for (int i = 1; i < route_.Count; ++i) {
							GraphManager.Instance.GetConnection (route_ [i - 1], 
								route_ [i]).SetOnPath (false, false);
						}
					}
				}
			} else {
				// delete nodes from end of route until we find the clickd node
				for (int i = route_.Count; i > 1; --i) {
					if(route_[i - 1] == target)
					{
						break;
					}
					GraphManager.Instance.GetConnection (route_ [i - 2], 
						route_ [i - 1]).SetOnPath (false, false);
					route_.RemoveAt (i - 1);
				}
			}
		}
		else if (!moving_) {
			Debug.Log ("Starting to move");
			route_.Add (target);
			moveNext ();

		} else if (route_.Count > 0 && target == route_ [route_.Count - 1]) {
			Debug.Log ("Removing a node from route");
      if (route_.Count >= 2)
      {
          GraphManager.Instance.GetConnection(route_[route_.Count - 2], route_[route_.Count - 1]).SetOnPath(false, false);
      }
            route_.RemoveAt (route_.Count - 1);
		} else
        {
            Node prev = route_.Count > 0 ? route_[route_.Count-1] : currentNode_;
            Connection conn = GraphManager.Instance.GetConnection(prev, target);
            conn.SetOnPath(true, conn.m_Node1 == target);
			route_.Add (target);
		}

		disableHighlighted ();
		highlightRoute ();
		highlightNeighbours ();
	}

	void disableHighlighted()
	{
		Debug.Log ("Clearing " + highlighted_.Count + " nodes.");
		foreach(Node n in highlighted_)
		{
			n.OnRoute = false;
			n.IsSelectable = false;
		}
		highlighted_.Clear ();
	}

	void highlightRoute()
	{

		//Debug.Log ("Highligting current node on route:" + currentNode_.name, currentNode_);
		currentNode_.OnRoute = true;
		currentNode_.IsSelectable = true;
		highlighted_.Add (currentNode_);

		foreach (Node n in route_) {
			//Debug.Log ("Highligting route:" + n.name, n);
			n.OnRoute = true;
			n.IsSelectable = true;
			highlighted_.Add (n);
		}
	}

	void highlightNeighbours()
	{
		var target = route_.LastOrDefault ();

		if (target == null) 
		{
			target = currentNode_;
		}

		if (target != null) {
			//Debug.Log ("Target node:" + target.name, target);
			var conns = graph_.GetConnections (target);
			foreach (Connection c in conns) {
				Node high = c.OtherEnd (target);
				if (!high.OnRoute) {
					//Debug.Log ("Highligting node:" + high.name, high);
					high.IsSelectable = true;
					highlighted_.Add (high);
				}
			}
		} else {
			Debug.LogError ("Current node was NULL");
		}
	}
		
	void moveNext()
	{
		if (route_.Count == 0)
        {
			Debug.Log ("Route finished");
            if (prevNode_ != null)
            {
                graph_.GetConnection(prevNode_, currentNode_).SetOnPath(false, false);
            }
            moving_ = false;
			return;
		}
		moving_ = true;
		Node nextNode = route_ [0];
        route_.RemoveAt(0);

		Connection conn = graph_.GetConnection(currentNode_, nextNode);
        conn.SetOnPath(true, conn.m_Node2 == currentNode_);

		float routeSpeed = conn.TravelTime * 
			Vector2.Distance(currentNode_.gameObject.transform.position, 
		  nextNode.gameObject.transform.position);
		//Debug.Log("Setting travel time to: " + routeSpeed);
		if (conn.m_Type == ConnectionType.Path) {
			id = LeanTween.move (gameObject, nextNode.gameObject.transform, routeSpeed).setEase (LeanTweenType.linear).id;
		} else {
			id = LeanTween.move (gameObject, nextNode.gameObject.transform, routeSpeed).setEase (LeanTweenType.easeInOutSine).id;
		}
		LTDescr d = LeanTween.descr( id );

		if(d!=null){ // if the tween has already finished it will return null
			// change some parameters
			d.setOnComplete( HandleMovementComplete );
		}

        if (prevNode_ != null)
        {
            graph_.GetConnection(prevNode_, currentNode_).SetOnPath(false, false);
        }
        prevNode_ = currentNode_;
		currentNode_ = nextNode;

		disableHighlighted ();
		highlightRoute ();
		highlightNeighbours ();
	}

	private void HandleMovementComplete()
	{
		if (OnMovementComplete != null) 
		{
			OnMovementComplete(gameObject, currentNode_);
		}
		moveNext ();
	}

	void DiseaseManager_OnWaveCompleted (int wave)
	{
		disableHighlighted();
		highlightNeighbours();
	}
}
