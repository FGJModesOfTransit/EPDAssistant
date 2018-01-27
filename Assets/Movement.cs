using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	public GameObject currentNode_ = null;
	bool moving_ = false;

	List<GameObject> route_ = new List<GameObject> ();

	int id = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void AddTarget(GameObject target)
	{
		if (currentNode_ == null) {
			Debug.Log ("Current node not set! Set currrent node in editor.");
			return;
		}

		//check if target legal
//		if (route.Count > 0 && route [route.Count - 1].neighbour (target)
		//			|| currentNode_.neighbour (target)) {
		if(true){
			route_.Add (target);
			if (!moving_) {
				moveNext ();
			}
		}
	}


	void moveNext()
	{
		if (route_.Count == 0) {
			Debug.Log ("ROute finished");
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
}
