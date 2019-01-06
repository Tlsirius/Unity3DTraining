using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
class NodeEditor : Editor
{
    Node node;

    private void OnEnable()
    {
        node = (Node)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawDragAndDrop(3);
        if (GUILayout.Button("Clear Adjacent Nodes"))
            node.clearAdjacent();
    }

    void DrawDragAndDrop(int m)
    {

        Event evt = Event.current;

        GUIStyle GuistyleBoxDND = new GUIStyle(GUI.skin.box);
        GuistyleBoxDND.alignment = TextAnchor.MiddleCenter;
        GuistyleBoxDND.fontStyle = FontStyle.Italic;
        GuistyleBoxDND.fontSize = 12;
        GUI.skin.box = GuistyleBoxDND;

        Rect dadRect = new Rect();
        dadRect = GUILayoutUtility.GetRect(0, 80, GUILayout.ExpandWidth(true));
        GUI.Box(dadRect, "Drag and Drop Prefabs to this Box!", GuistyleBoxDND);

        if (dadRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                //Debug.Log("Drag Updated!");
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                //Debug.Log("Drag Perform!");
                Debug.Log(DragAndDrop.objectReferences.Length);
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    GameObject g = DragAndDrop.objectReferences[i] as GameObject;
                    Node draggedNode = g.GetComponent<Node>();
                    node.addAdjacent(draggedNode);
                    draggedNode.addAdjacent(node);

                }
                Event.current.Use();
            }
        }
    }


    
}