using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	/*
	public float speed = 1.0f;
	public GameObject target = null;
	private Transform t = null;
*/

	int id = 0;
	// Use this for initialization
	void Start () {
		//t = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (target != null) {

			float movement = 0;

			t.position = Vector2.Lerp (t.position, target.transform.position, Time.deltaTime*speed);
		}*/
	}

	public void setTarget(GameObject target)
	{
		id = LeanTween.moveX(gameObject, 1f, 1f).id;
		LTDescr d = LeanTween.descr( id );

		if(d!=null){ // if the tween has already finished it will return null
			// change some parameters
			//d.setOnComplete( onCompleteFunc ).setEase( LeanTweenType.easeInOutBack );
		}
	}
}
