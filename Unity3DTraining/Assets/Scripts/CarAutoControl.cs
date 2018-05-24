using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAutoControl : MonoBehaviour
{

    //public variables
    public float sprintSpeed = 40f;         //max speed when sprinting(unit/second)
    public float turningSpeed = 30f;        //turning speed at the crossroad(unit/second)
    public float acceleration = 5f;         //acceleration and deceleration(unit/second^2)

    //Const parameters
    private const float height = 0f;                        //height(y-coordinate) of the car
    private const float roadWidth = 5f;                     //road width
    private const float stoppingDistance = 0.1f;            //If the distance to destination < stoppingDistance, force the car to reach destination                   
    enum TurningDirection { Left, Straight, Right };        //turning direction

    //Map Nodes. Each node represents a crossroad
    private const int numNodes = 6;                         //number of nodes/crossroads
    private Vector3[] Nodes = new Vector3[numNodes];        //node coordinates
    private bool[,] NodeMap = new bool[numNodes, numNodes]; //graph of nodes. NodeMap[a,b] = true means a and b is connected.

    public int startingNode = 0;                           //starting Node of current straight line
    public int targetNode = 1;                             //target Node of current straight line

    //Variables relating to car state
    private bool crossing = false;                          //indicates whether the car is crossing a crossroad
    private float currentSpeed = 0f;                        //current speed
    private Vector3 sprintStartPos;                         //starting position of straight line sprint
    private Vector3 sprintEndPos;                           //ending position of straight line sprint
    private Vector3 turningStartPos;                        //starting position of turning at crossroad
    private Quaternion turningStartDirection;               //starting rotation of turning at crossroad
    private int turningFrameNum;                            //num of frames needed to cross the crossroad
    private int currentTurningFrame = 0;
    TurningDirection turningDirection = TurningDirection.Left;
    private float turningRadius;
    private float decDistance = 0f;                   //distance of deceleration in straight line sprint


    // Use this for initialization
    void Start()
    {
        InitNode();
        InitState();
    }


    // Initialize the nodes' coordinate and connection
    private void InitNode()
    {
        Nodes[0] = new Vector3(77.5f, height, 30f);
        Nodes[1] = new Vector3(0f, height, 30f);
        Nodes[2] = new Vector3(-77.5f, height, 30f);
        Nodes[3] = new Vector3(77.5f, height, -30f);
        Nodes[4] = new Vector3(0f, height, -30f);
        Nodes[5] = new Vector3(-77.5f, height, -30f);

        for (int i = 0; i < numNodes; i++)
        {
            for (int j = 0; j < numNodes; j++)
            {
                string szLog = "i: " + i + " j: " + j;
                NodeMap[i, j] = false;
            }
        }

        ConnectNode(0, 1);
        ConnectNode(1, 2);
        ConnectNode(3, 4);
        ConnectNode(4, 5);
        ConnectNode(0, 3);
        ConnectNode(1, 4);
        ConnectNode(2, 5);
    }

    //Initialize speed and position
    public void InitState()
    {
        currentSpeed = turningSpeed;
        sprintStartPos = GetsprintStartPos();
        sprintEndPos = GetsprintEndPos();
        //Debug.Log("sprintEndPos" + sprintStartPos);
        //Debug.Log("sprintEndPos" + sprintEndPos);
        transform.position = sprintStartPos;
        transform.rotation = Quaternion.Euler(0, -90, 0);
        CalculateDistance();
    }

    // Calculate the starting position of straight line sprint
    Vector3 GetsprintStartPos()
    {
        Vector3 pos = new Vector3(0f, height, 0f);
        if (Nodes[startingNode].x == Nodes[targetNode].x)
        {
            if (Nodes[startingNode].z > Nodes[targetNode].z)
            {
                pos.x = Nodes[startingNode].x - 0.25f * roadWidth;
                pos.z = Nodes[startingNode].z - 0.5f * roadWidth;
            }
            else
            {
                pos.x = Nodes[startingNode].x + 0.25f * roadWidth;
                pos.z = Nodes[startingNode].z + 0.5f * roadWidth;
            }
        }
        else if (Nodes[startingNode].z == Nodes[targetNode].z)
        {
            if (Nodes[startingNode].x > Nodes[targetNode].x)
            {
                pos.x = Nodes[startingNode].x - 0.5f * roadWidth;
                pos.z = Nodes[startingNode].z + 0.25f * roadWidth;
            }
            else
            {
                pos.x = Nodes[startingNode].x + 0.5f * roadWidth;
                pos.z = Nodes[startingNode].z - 0.25f * roadWidth;
            }
        }
        else
        {
            Debug.Log("Error! Invalid Node Input!");
        }
        //Debug.Log(pos.ToString());
        return pos;
    }

    // Calculate the ending position of straight line sprint
    Vector3 GetsprintEndPos()
    {
        Vector3 pos = new Vector3(0f, height, 0f);
        if (Nodes[startingNode].x == Nodes[targetNode].x)
        {
            if (Nodes[startingNode].z > Nodes[targetNode].z)
            {
                pos.x = Nodes[targetNode].x - 0.25f * roadWidth;
                pos.z = Nodes[targetNode].z + 0.5f * roadWidth;
            }
            else
            {
                pos.x = Nodes[targetNode].x + 0.25f * roadWidth;
                pos.z = Nodes[targetNode].z - 0.5f * roadWidth;
            }
        }
        else if (Nodes[startingNode].z == Nodes[targetNode].z)
        {
            if (Nodes[startingNode].x > Nodes[targetNode].x)
            {
                pos.x = Nodes[targetNode].x + 0.5f * roadWidth;
                pos.z = Nodes[targetNode].z + 0.25f * roadWidth;
            }
            else
            {
                pos.x = Nodes[targetNode].x - 0.5f * roadWidth;
                pos.z = Nodes[targetNode].z - 0.25f * roadWidth;
            }
        }
        else
        {
            Debug.Log("Error! Invalid Node Input!");
        }
        //Debug.Log(pos.ToString());
        return pos;
    }

    // Calculate the ending position of straight line sprint
    private void ConnectNode(int i, int j)
    {
        NodeMap[i, j] = true;
        NodeMap[j, i] = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update begin: transform.position:" + transform.position.ToString());
        //Debug.Log("transform.forward:" + transform.forward.ToString());
        if (crossing)
        {
            CrossRoad();
        }
        else
        {
            //Debug.Log("transform.position:"+transform.position.ToString());
            //Debug.Log("sprintEndPos:"+sprintEndPos.ToString());
            //Debug.Log(turningSpeed * Time.deltaTime);
            //Debug.Log(currentSpeed);
            if (Vector3.Distance(transform.position, sprintEndPos) <= turningSpeed * Time.deltaTime)
            {
                transform.position = sprintEndPos;
                crossing = true;
                GetNextNode();
            }
            else
            {
                GoStraight();
            }
        }
        //Debug.Log("Update end: transform.position:" + transform.position.ToString());
    }

    // Randomly choose the a node as destination 
    private void GetNextNode()
    {
        //Debug.Log("GetNextNode");
        Debug.Log(transform.position.ToString());
        List<int> validNodes = new List<int>();
        for (int i = 0; i < numNodes; i++)
        {
            if (i != startingNode && NodeMap[targetNode, i] == true)
            {
                validNodes.Add(i);
            }
        }
        if (validNodes.Count == 0)
        {
            Destroy(this);
        }
        int randomNode = validNodes[Random.Range(0, validNodes.Count)];
        //int randomNode = validNodes[1];
        //Debug.Log("NextNode:" + randomNode);
        float turningAngle = CalculateAngle(Nodes[randomNode] - Nodes[targetNode], Nodes[targetNode] - Nodes[startingNode]);
        if (turningAngle > 45f)
        {
            turningDirection = TurningDirection.Left;
            turningRadius = 0.75f * roadWidth;

            float angleAcc = 2 * Mathf.Asin(0.5f * turningSpeed * Time.deltaTime / turningRadius);
            //Debug.Log("angleAcc:" + angleAcc.ToString());
            turningFrameNum = Mathf.CeilToInt(Mathf.PI / (2 * angleAcc));
            //Debug.Log("turningFrameNum:" + turningFrameNum.ToString());
            turningStartPos = sprintEndPos;
            turningStartDirection = transform.rotation;
        }
        else if (turningAngle > -45f)
        {
            turningDirection = TurningDirection.Straight;
            turningRadius = 0f;
        }
        else
        {
            turningDirection = TurningDirection.Right;
            turningRadius = 0.25f * roadWidth;
            float angleAcc = 2 * Mathf.Asin(0.5f * turningSpeed * Time.deltaTime / turningRadius);
            turningFrameNum = Mathf.CeilToInt(Mathf.PI / (2 * angleAcc));
            turningStartPos = sprintEndPos;
            turningStartDirection = transform.rotation;
        }
        currentTurningFrame = 0;
        startingNode = targetNode;
        targetNode = randomNode;
        sprintStartPos = GetsprintStartPos();
        sprintEndPos = GetsprintEndPos();

        //turningAngle = 90f;
    }

    // Calculate signed angle between 2 vectors
    private float CalculateAngle(Vector3 a, Vector3 b)
    {
        var angle = Vector3.Angle(a, b); // calculate angle
                                         // assume the sign of the cross product's Y component:
        return angle * Mathf.Sign(Vector3.Cross(a, b).y);
    }

    // Calculate distance of deceleration
    private void CalculateDistance()
    {
        float maxDecDistance = 0.5f * (sprintSpeed + turningSpeed) * (sprintSpeed - turningSpeed) / acceleration;
        float entireDistance = Vector3.Distance(sprintStartPos, sprintEndPos);
        decDistance = Mathf.Min(maxDecDistance, 0.5f * entireDistance);
    }

    // Car motion in straight line
    private void GoStraight()
    {
        //Debug.Log("GoStraight");
        if (Vector3.Distance(transform.position, sprintEndPos) <= decDistance && currentSpeed > turningSpeed)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }
        else if (currentSpeed < sprintSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        Vector3 direction = sprintEndPos - sprintStartPos;
        transform.position += currentSpeed * Time.deltaTime * direction / direction.magnitude;
    }

    // Car motion in crossroad
    private void CrossRoad()
    {
        float curAngle;
        Vector3 shiftFoward = new Vector3(0, 0, 0);
        Vector3 shiftLeft = new Vector3(0, 0, 0);
        Vector3 tmp;
        //Debug.Log("CrossRoad");
        switch (turningDirection)
        {
            case TurningDirection.Left:
                //Debug.Log("currentFrame:" + currentTurningFrame.ToString());
                curAngle = Mathf.PI * (float)currentTurningFrame / ((float)turningFrameNum * 2f);
                //Debug.Log("curAngle:" + curAngle);
                shiftFoward = turningRadius * (turningStartDirection * Vector3.forward) * Mathf.Sin(curAngle);
                shiftLeft = turningRadius * (turningStartDirection * Vector3.left) * (1 - Mathf.Cos(curAngle));
                //Debug.Log("shiftFowrad:" + shiftFoward.ToString());
                //Debug.Log("shiftLeft:" + shiftLeft.ToString());
                transform.position = turningStartPos + shiftFoward + shiftLeft;
                tmp = new Vector3(0, -curAngle * 180f / Mathf.PI, 0);
                transform.eulerAngles = tmp + turningStartDirection.eulerAngles;

                //Debug.Log("turningStartDirection:" + turningStartDirection.ToString());
                //Debug.Log("transform.eulerAngles:" + transform.eulerAngles.ToString());
                if (currentTurningFrame >= turningFrameNum)
                {
                    crossing = false;
                }
                currentTurningFrame++;
                break;
            case TurningDirection.Straight:

                if (Nodes[startingNode].x == Nodes[targetNode].x)
                {
                    if (Mathf.Abs(transform.position.z - sprintStartPos.z) < stoppingDistance)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, sprintStartPos.z);
                        crossing = false;
                        return;
                    }
                }
                else
                {
                    if (Mathf.Abs(transform.position.x - sprintStartPos.x) < stoppingDistance)
                    {
                        transform.position = new Vector3(sprintStartPos.x, transform.position.y, transform.position.z);
                        crossing = false;
                        return;
                    }

                }
                Vector3 deltaShift = transform.forward * turningSpeed * Time.deltaTime;
                transform.position = transform.position + deltaShift;
                break;
            case TurningDirection.Right:
                curAngle = Mathf.PI * (float)currentTurningFrame / ((float)turningFrameNum * 2f);
                shiftFoward = turningRadius * (turningStartDirection * Vector3.forward) * Mathf.Sin(curAngle);
                shiftLeft = turningRadius * (turningStartDirection * Vector3.right) * (1 - Mathf.Cos(curAngle));
                transform.position = turningStartPos + shiftFoward + shiftLeft;
                tmp = new Vector3(0, curAngle * 180f / Mathf.PI, 0);
                transform.eulerAngles = tmp + turningStartDirection.eulerAngles;

                if (currentTurningFrame >= turningFrameNum)
                {
                    crossing = false;
                }
                currentTurningFrame++;
                break;
        }

    }
    
}
