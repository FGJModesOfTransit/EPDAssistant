using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    None = 0,
    Path = 1,
    Road = 2
}

[ExecuteInEditMode]
[System.Serializable]
public class Connection : MonoBehaviour
{
    public Node m_Node1, m_Node2;
    public ConnectionType m_Type;

    public void Set(Node n1, Node n2, ConnectionType type)
    {
        m_Node1 = n1;
        m_Node2 = n2;
        m_Type = type;
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

    private void Update()
    {
        if ( !Application.isPlaying )
        {
            EditorUpdate();
        }
        else
        {

        }
    }

    public void Stretch(GameObject sprite, Vector3 pos1, Vector3 pos2, bool _mirrorZ)
    {
        SpriteRenderer rend = sprite.GetComponent<SpriteRenderer>();
        if (rend == null) return;
        float origSize = rend.sprite.pixelsPerUnit * rend.sprite.texture.texelSize.x;

        Vector3 centerPos = (pos1 + pos2) / 2f;
        sprite.transform.position = centerPos;
        Vector3 direction = (pos2-pos1).normalized;
        sprite.transform.right = direction;
        if (_mirrorZ) sprite.transform.right *= -1f;
        Vector3 scale = new Vector3(1, 1, 1);
        scale.x = Vector3.Distance(pos1, pos2);
        sprite.transform.localScale = origSize * scale ;
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
            default:
                color = Color.black;
                break;
        }

        Gizmos.color = color;
        Gizmos.DrawLine(m_Node1.transform.position, m_Node2.transform.position);
    }
}
