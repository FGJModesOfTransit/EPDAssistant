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

	[SerializeField]
	private Sprite carSprite;
	[SerializeField]
	private Sprite bikeSprite;
	[SerializeField]
	private Sprite trainSprite;

	[SerializeField]
	private SpriteRenderer vehicleRenderer;
    
	bool moving_ = false;

	GraphManager graph_ = null;

	List<Node> route_ = new List<Node> ();

	List<Node> highlighted_ = new List<Node> ();

	int moveTweenId = 0;

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

		vehicleRenderer.sprite = bikeSprite;

		highlightNeighbours ();
	}
		
	public void AddTarget(Node target)
	{
		if (currentNode_ == null) 
		{
			Debug.LogError ("Current node not set! Set currrent node in editor.");
			return;
	  	}

		if (target.OnRoute)
		{
			//check if on route and delete route from that point onwards

			if (target == currentNode_) 
			{
				if (route_.Count > 0) 
				{
					if (currentNode_ != route_ [0]) 
					{
						GraphManager.Instance.GetConnection (currentNode_, route_ [0]).SetOnPath (false, false);
					}

					if (route_.Count > 1) 
					{
						for (int i = 1; i < route_.Count; ++i) 
						{
							if (route_ [i - 1] != route_ [i]) 
							{
								GraphManager.Instance.GetConnection (route_ [i - 1], route_ [i]).SetOnPath (false, false);
							}
						}
					}
				}
				route_.Clear ();
			} 
			else 
			{
				// delete nodes from end of route until we find the clickd node
				for (int i = route_.Count; i > 1; --i) 
				{
					if(route_[i - 1] == target)
					{
						break;
					}
					if (route_ [i - 2] != route_ [i - 1]) 
					{
						GraphManager.Instance.GetConnection (route_ [i - 2], route_ [i - 1]).SetOnPath (false, false);
						route_.RemoveAt (i - 1);
					}
				}
			}
		}
		else if (target == prevNode_ && moving_ && route_.Count < 1) 
		{
			if (route_.Count >= 2 && route_[route_.Count - 2] != route_[route_.Count - 1])
			{
				GraphManager.Instance.GetConnection(route_[route_.Count - 2], route_[route_.Count - 1]).SetOnPath(false, false);
			}
			route_.Clear ();
			route_.Add (target);
			prevNode_ = currentNode_;
			LeanTween.cancel (moveTweenId);
			moving_ = false;
			moveNext ();
		}
		else if (!moving_) 
		{
			route_.Add (target);
			moveNext ();
		} 
		else if (route_.Count > 0 && target == route_ [route_.Count - 1]) 
		{
			if (route_.Count >= 2 && route_[route_.Count - 2] != route_[route_.Count - 1])
			{
			  GraphManager.Instance.GetConnection(route_[route_.Count - 2], route_[route_.Count - 1]).SetOnPath(false, false);
			}
            route_.RemoveAt (route_.Count - 1);
		}
		else
        {
            Node prev = route_.Count > 0 ? route_[route_.Count-1] : currentNode_;
            Connection conn = GraphManager.Instance.GetConnection(prev, target);
			if (conn != null) 
			{
				conn.SetOnPath(true, conn.m_Node1 == target);
			}
            
			if (route_.Count < 2) 
			{
				route_.Add (target);
			}
		}

		disableHighlighted ();
		highlightRoute ();

		if (route_.Count < 2)
		{
			highlightNeighbours ();
		}
	}

	void disableHighlighted()
	{
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
				if (!high.OnRoute && !high.Lost) {
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
			if (prevNode_ != null && prevNode_ != currentNode_)
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
		if (conn == null) 
		{
			Debug.LogError ("Trying to move without connection", nextNode);
			return;
		}

		conn.SetOnPath(true, conn.m_Node2 == currentNode_);
		float routeSpeed = conn.TravelTime * Vector2.Distance(transform.position, nextNode.gameObject.transform.position);

		//Debug.Log("Setting travel time to: " + routeSpeed);
		if (conn.m_Type == ConnectionType.Path) 
		{
			moveTweenId = LeanTween.move (gameObject, nextNode.gameObject.transform, routeSpeed).setEase (LeanTweenType.linear).id;
		}
		else
		{
			moveTweenId = LeanTween.move (gameObject, nextNode.gameObject.transform, routeSpeed).setEase (LeanTweenType.easeInOutSine).id;
		}
		LTDescr d = LeanTween.descr( moveTweenId );

		// if the tween has already finished it will return null
		if(d!=null)
		{ 
			// change some parameters
			d.setOnComplete( HandleMovementComplete );
		}

		if (prevNode_ != null && currentNode_ != prevNode_)
        {
            graph_.GetConnection(prevNode_, currentNode_).SetOnPath(false, false);
        }
        prevNode_ = currentNode_;
		currentNode_ = nextNode;

		switch (conn.m_Type) 
		{
		case ConnectionType.Path:
			vehicleRenderer.sprite = bikeSprite;
			break;
		case ConnectionType.Railway:
			vehicleRenderer.sprite = trainSprite;
			break;
		case ConnectionType.Road:
			vehicleRenderer.sprite = carSprite;
			break;
		}
			
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
