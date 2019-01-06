using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    
    public List<Node> adjacentNodes = new List<Node>();
    //public Node adjacentNode;

    private PathManager pathManager;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, new Vector3(1,1,1));
    }

    //private void OnValidate()
    //{

    //    Debug.Log("Onvalidate");
    //    if (adjacentNode!=null)
    //    {
    //        addAdjacent(adjacentNode);
    //        adjacentNode.addAdjacent(this);
    //        adjacentNode = null;            
    //    }
    //}

    public void addAdjacent(Node n)
    {
        Debug.Log("Add Adjacent");
        if (!adjacentNodes.Find(x => x == n))
        {
            adjacentNodes.Add(n);
        }
    }

    public void removeAdjacent(Node n)
    {
        Debug.Log("Remove Adjacent");
        adjacentNodes.Remove(n);
    }

    public void clearAdjacent()
    {
        Debug.Log("Clear Adjacent");
        for (int i=0;i<adjacentNodes.Count;i++)
        {
            adjacentNodes[i].removeAdjacent(this);
        }
        adjacentNodes.Clear();
    }
    

}
