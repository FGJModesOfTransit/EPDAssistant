using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    None = 0,
    Path = 1,
    Road = 2,
	  Railway = 3
}

[ExecuteInEditMode]
[System.Serializable]
public class Connection : MonoBehaviour
{
    public Node m_Node1, m_Node2;
    public ConnectionType m_Type;
    public float Width = 5;
  	public float TravelTime = 1.0f;

    private GameObject m_RouteSprite;
	private SpriteRenderer m_Sprite;

    ConnectionType Type
    {
        get { return m_Type; }
        set { if ( m_Type != value )
            {
                m_Type = value;
            }
        }
    }

    public void SetOnPath(bool value, bool inverted)
    {
        if (m_RouteSprite != null)
        {
            m_RouteSprite.gameObject.SetActive(value);
            if (value)
            {
                Stretch(m_RouteSprite.gameObject, m_Node1.transform.position, m_Node2.transform.position, inverted);
            }
        }
    }

    public void Set(Node n1, Node n2, ConnectionType type)
    {
        m_Node1 = n1;
        m_Node2 = n2;
        Type = type;
    }

		public Node OtherEnd(Node node) 
		{
		if (m_Node1 == null || m_Node2 == null) {
			Debug.LogError ("null poister in connection");
		}
			if (m_Node1 != node)
				return m_Node1;

			return m_Node2;
		}

    private void EditorUpdate()
    {
        if (m_Node1 != null)
        {
            transform.position = m_Node1.transform.position;

            if (m_Node2 != null)
            {
                Stretch(gameObject, m_Node1.transform.position, m_Node2.transform.position, false);
            }
        }
    }

    void OnEnable()
    {
        Transform marker = transform.Find("RouteMarker");
        if (marker != null ) m_RouteSprite = marker.gameObject;
        if (m_RouteSprite)
        {
            m_RouteSprite.gameObject.SetActive(false);
        }
    }

	void Awake()
	{
		m_Sprite = GetComponent<SpriteRenderer> ();
	}

    private void Update()
    {
        if ( !Application.isPlaying )
        {
            EditorUpdate();
        }
        else
        {
			if (m_Sprite && m_Node1 && m_Node2)
			{
				m_Sprite.enabled = m_Node1.gameObject.activeInHierarchy && m_Node2.gameObject.activeInHierarchy;
			}
        }
    }

    public void Stretch(GameObject sprite, Vector3 pos1, Vector3 pos2, bool _mirrorZ)
    {
        SpriteRenderer rend = sprite.GetComponent<SpriteRenderer>();
        if (rend == null) return;
        Vector2 origSize = rend.sprite.pixelsPerUnit * rend.sprite.texture.texelSize;

        Vector3 centerPos = (pos1 + pos2) / 2f;
        sprite.transform.position = centerPos;
        Vector3 direction = (pos2-pos1).normalized;
        sprite.transform.right = direction;
        if (_mirrorZ) sprite.transform.right *= -1f;
        //Vector3 scale = new Vector3(origSize.x, Width, 1);
        Vector3 scale = new Vector3(1f, Width, 1);
        scale.x *= Vector3.Distance(pos1, pos2);

        rend.size = new Vector2(scale.x, scale.y);
        sprite.transform.localScale = Vector3.one;
    }

    public void DrawGizmo()
    {
        Color color;
        switch ( m_Type )
        {
            case ConnectionType.Path:
                color = Color.cyan;
                break;
            case ConnectionType.Road:
                color = Color.white;
                break;
						case ConnectionType.Railway:
								color = Color.gray;
								break;
		        default:
                color = Color.black;
                break;
        }

        Gizmos.color = color;
        Gizmos.DrawLine(m_Node1.transform.position, m_Node2.transform.position);
    }
}
