using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public Node[] nodeList;
    private bool[,] NodeMap;

    public Node startNode;
    public Node nextNode;

    private Node curStartNode;
    private Node curTargetNode;


    // Use this for initialization
    void Start () {
        curStartNode = startNode;
        curTargetNode = nextNode;
	}
	
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        nodeList = GetComponentsInChildren<Node>();
        int num = nodeList.Length;
        NodeMap = new bool[num, num];
        for(int i=0;i<nodeList.Length;i++)
        {
            Vector3 pos1 = nodeList[i].transform.position;
            for(int j=0;j< nodeList[i].adjacentNodes.Count;j++)
            {
                Vector3 pos2 = nodeList[i].adjacentNodes[j].transform.position;
                //Debug.Log("Draw");
                Gizmos.DrawLine(pos1, pos2);
            }
        }
    }

    public Vector3 getCurrentStartPos()
    {
        return curStartNode.transform.position;
    }

    public Vector3 getCurrentTargetPos()
    {
        return curTargetNode.transform.position;
    }

    public Vector3 nextPos()
    {
        if(curTargetNode.adjacentNodes.Count<=1)
        {
            Debug.LogError("No adjacent node found");
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, curTargetNode.adjacentNodes.Count - 1);
            int j = 0;
            for (int i = 0; i < curTargetNode.adjacentNodes.Count; i++)
            {
                if (curTargetNode.adjacentNodes[i] != curStartNode)
                {
                    if (j == rand)
                    {
                        curStartNode = curTargetNode;
                        curTargetNode = curTargetNode.adjacentNodes[i];
                        return curTargetNode.transform.position;
                    }
                    j++;
                }
            }
        }


        return new Vector3(0,0,0);
    }
}
