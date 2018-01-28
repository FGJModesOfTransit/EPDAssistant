using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraPanAndZoom : MonoBehaviour 
{
	public static CameraPanAndZoom Instance
	{
		get
		{
			if (m_Instance == null)
			{
				var obj = FindObjectOfType<CameraPanAndZoom>();
				if (obj != null) m_Instance = obj;
			}
			return m_Instance;
		}
	}
	private static CameraPanAndZoom m_Instance;

	private float dragAmount = 0;
	public bool IsMoving { get { return dragAmount > 150; } }

	[SerializeField]
	private float mouseZoomScale = 1;

	[SerializeField]
	private float touchZoomScale = 1;

	[SerializeField]
	private float smoothTime = 0.5f;

	[SerializeField]
	private float minCameraScale = 1f;

	[SerializeField]
	private float maxCameraScale = 5f;

	[SerializeField]
	private Transform initialFocus;

	[SerializeField]
	private Vector2 minBounds;
	[SerializeField]
	private Vector2 maxBounds;

	private bool dragging = false;

	private Camera gameCamera;

	private bool isPinching = false;

	private float pinchDistance = 0;

	private Vector3 origin;

	private Vector3 velocity;

	private bool underInertia;

	private float time = 0.0f;

	LTDescr goToAnimation;

	private void Awake()
	{
		gameCamera = GetComponent<Camera>();
		gameCamera.orthographicSize = maxCameraScale;
	}

	IEnumerator Start()
	{
		yield return new WaitForSeconds (1);

		if (initialFocus != null) 
		{
			GoToPoint(initialFocus.position);
			LeanTween.value (gameCamera.orthographicSize, 7, 1.5f)
				.setOnUpdate ((zoom) => {
					gameCamera.orthographicSize = zoom;
			});
		}
	}

	public void GoToPoint(Vector3 point)
	{
		if (goToAnimation != null) 
		{
			LeanTween.cancel(goToAnimation.id);
		}

		goToAnimation = LeanTween.move(gameObject, new Vector3 (point.x, point.y, transform.position.z), 1.2f);
	}

	private void Update()
	{		
		if (!MenuManager.Instance.SideMenu.IsOpen) 
		{
			HandelDrag();	

			HandleZoom();

			HandleInertia();
		}
	}

	private void HandelDrag()
	{
		Vector3 difference = Vector3.zero;

		if (Input.GetMouseButtonUp(0)) 
		{
			underInertia = true;
			dragging = false;
			dragAmount = 0;
		}
		else if (Input.GetMouseButton(0))
		{
			if (goToAnimation != null) 
			{
				LeanTween.cancel(goToAnimation.id);
				goToAnimation = null;
			}
			var touchposition = Input.mousePosition;
			if (Input.touchCount > 0) 
			{
				touchposition = Input.touches[0].position;
			}
			difference = (gameCamera.ScreenToWorldPoint(touchposition)) - transform.position;
			dragAmount += difference.magnitude;
			if (!dragging)
			{
				dragging = true;
				origin = gameCamera.ScreenToWorldPoint(touchposition);
			}
			underInertia = false;
		}
		else
		{
			dragging = false;
		}

		if (dragging) 
		{
			var newPos = origin - difference;
			newPos.x = Mathf.Clamp (newPos.x, minBounds.x, maxBounds.x);
			newPos.y = Mathf.Clamp (newPos.y, minBounds.y, maxBounds.y);
			velocity = newPos - transform.position;
			transform.position = newPos;
		}
	}

	private void HandleZoom()
	{
		float scale = gameCamera.orthographicSize;
		float step = Input.GetAxis("Mouse ScrollWheel") * gameCamera.orthographicSize * mouseZoomScale;
		scale -= step;

		if (Input.touchCount >= 2) 
		{
			var newPinchDistance = Vector2.Distance (Input.touches [0].position, Input.touches [1].position);

			if (isPinching) 
			{
				var delta = newPinchDistance - pinchDistance;
				delta /= Screen.height;
				delta *= gameCamera.orthographicSize * touchZoomScale;
				scale -= delta;
			}

			pinchDistance = newPinchDistance;

			isPinching = true;
		}
		else 
		{
			isPinching = false;
		}

		gameCamera.orthographicSize = Mathf.Clamp(scale, minCameraScale, maxCameraScale);
	}

	private void HandleInertia()
	{
		if (underInertia && time <= smoothTime) 
		{
			var newPos = transform.position + velocity;
			newPos.x = Mathf.Clamp (newPos.x, minBounds.x, maxBounds.x);
			newPos.y = Mathf.Clamp (newPos.y, minBounds.y, maxBounds.y);

			transform.position = newPos;

			velocity = Vector3.Lerp (velocity, Vector3.zero, time);
			time += Time.smoothDeltaTime;
		} 
		else 
		{
			underInertia = false;
			time = 0.0f;
		}
	}
}
