using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Node))]
[CanEditMultipleObjects]
class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Node me = (Node)target;

        DrawDefaultInspector();
        EditorGUILayout.Space();

        AddConnectionBox(me, ConnectionType.Path, "path");
        AddConnectionBox(me, ConnectionType.Road, "road");
    		AddConnectionBox(me, ConnectionType.Railway, "railway");
    }

    private void AddConnectionBox(Node me, ConnectionType type, string name)
    {
        Event e = Event.current;
        Rect drop = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
        GUI.Box(drop, "Drag node here to add " + name);

        switch (e.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop.Contains(e.mousePosition)) break;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (e.type == EventType.DragPerform )
                {
                    DragAndDrop.AcceptDrag();
                    foreach (GameObject other in DragAndDrop.objectReferences )
                    {
                        Node otherNode = other.GetComponent<Node>();
                        if ( otherNode != null && otherNode != me)
                        {
                            GameObject connObj = PrefabUtility.InstantiatePrefab(GraphManager.Instance.ConnectionPrefabs[(int)type]) as GameObject;

                            GraphManager.Instance.CreateConnection(connObj, me, otherNode, type);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}
