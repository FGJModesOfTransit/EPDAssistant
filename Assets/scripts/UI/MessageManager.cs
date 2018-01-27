using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class MessageManager : MonoBehaviour 
{
	public static MessageManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				var obj = FindObjectOfType<MessageManager>();
				if (obj != null) m_Instance = obj;
			}
			return m_Instance;
		}
	}
	private static MessageManager m_Instance;

	[SerializeField]
	private float messageTime = 3;

	[SerializeField]
	private float lastMessageTimeAdded = 5;

	[SerializeField]
	private GameObject messagePanel;

	[SerializeField]
	private Text messagePanelText;

	private Canvas canvas;

	private class Message
	{
		public string Content { get; set; }
		public float TimeToLive { get; set; }
		public bool Complete { get; set; }
		public Action TapAction { get; set; }
	}

	private Queue<Message> messageQueue = new Queue<Message>();

	private Message currentMessage;

	void Awake()
	{
		canvas = GetComponent<Canvas> ();
		LeanTween.init();
	}

	IEnumerator Start()
	{
		yield return new WaitForSeconds (1);
		AddMessage ("Hi there dispatcher, I'm the local doctor. Seems to be a quiet day!", () => CameraPanAndZoom.Instance.GoToPoint (Movement.PlayerCharacter.transform.position));
	}

	public void AddMessage(string content, Action action = null)
	{
		var message = new Message (){ Content = content, TimeToLive = messageTime, TapAction = action };

		messageQueue.Enqueue (message);	

		if (currentMessage == null) 
		{
			ShowNextMessage();
		}
	}

	void Update()
	{
		if (Time.time > 0.5f && currentMessage != null && !currentMessage.Complete) 
		{
			currentMessage.TimeToLive -= Time.deltaTime;

			if (messageQueue.Count == 0) 
			{
				if (currentMessage.TimeToLive <= -lastMessageTimeAdded) 
				{
					HideCurrentMessage();
				}
			}
			else if (currentMessage.TimeToLive <= 0) 
			{
				HideCurrentMessage();
			}
		}
	}

	public void HideCurrentMessage()
	{
		if (!currentMessage.Complete) 
		{
			currentMessage.Complete = true;

			LeanTween.cancel(messagePanel.gameObject);
			LeanTween.moveY(messagePanel.GetComponent<RectTransform> (), 64 * canvas.scaleFactor, 0.2f)
				.setEase (LeanTweenType.easeInCubic)
				.setOnComplete(() => ShowNextMessage());
		}
	}

	private void ShowNextMessage()
	{
		if (messageQueue.Count > 0) 
		{
			currentMessage = messageQueue.Dequeue();
			if (!messagePanel.activeInHierarchy) 
			{
				messagePanel.SetActive (true);
			}
			messagePanelText.text = currentMessage.Content;

			LeanTween.cancel (messagePanel.gameObject);
			LeanTween.moveY(messagePanel.GetComponent<RectTransform>(), 0, 0.2f)
				.setEase (LeanTweenType.easeInCirc);
		}
		else 
		{
			currentMessage = null;
			messagePanel.SetActive(false);
		}
	}

	public void MessageAction()
	{
		if (currentMessage != null && currentMessage.TapAction != null) 
		{
			currentMessage.TapAction();
		}
	}
}
