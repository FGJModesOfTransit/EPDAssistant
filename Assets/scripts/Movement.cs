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
			Debug.Log ("Current node not set! Set currrent node in editor.");
			return;
	  }

		if (!moving_) {
			Debug.Log ("Starting to move");
			route_.Add (target);
			moveNext ();

		} else if (route_.Count > 0 && target == route_ [route_.Count - 1]) {
			Debug.Log ("Removing a node from route");
			route_.RemoveAt (route_.Count - 1);
		} else {
			route_.Add (target);
		}

		disableHighlighted ();
		highlightNeighbours ();
	}

	void disableHighlighted()
	{
		Debug.Log ("Clearing " + highlighted_.Count + " nodes.");
		foreach(Node n in highlighted_)
		{
			n.IsSelectable = false;
		}
		highlighted_.Clear ();
	}

	void highlightNeighbours()
	{
		var target = route_.LastOrDefault ();

		if (target == null) 
		{
			target = currentNode_;
		}

		if (target != null) {
			Debug.Log ("Target node:" + target.name, target);
			var conns = graph_.GetConnections (target);
			foreach (Connection c in conns) {
				Node high = c.OtherEnd (target);
				Debug.Log ("Highligting node:" + high.name, high);
				high.IsSelectable = true;
				highlighted_.Add (high);

			}
		} else {
			Debug.LogError ("Current node was NULL");
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
		Node nextNode = route_ [0];
		route_.RemoveAt (0);

		Connection conn = graph_.GetConnection(currentNode_, nextNode);

		float routeSpeed = conn.TravelTime;
		Debug.Log("Setting travel time to: " + routeSpeed);
		id = LeanTween.move(gameObject, nextNode.gameObject.transform, routeSpeed).id;
		LTDescr d = LeanTween.descr( id );

		if(d!=null){ // if the tween has already finished it will return null
			// change some parameters
			d.setOnComplete( HandleMovementComplete );
		}

		currentNode_ = nextNode;
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
