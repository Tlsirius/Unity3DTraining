using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDriving : MonoBehaviour {

    public float speed = 2;
    public float turningDistance = 1;
    public float turningAngularVelocity = 1;

    private Vector3 previousPos = new Vector3(0, 0, 0);
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 targetPos = new Vector3(0, 0, 0);
    private PathManager pathManager;

    private bool turning = false;
    private Vector3 turningCenter = new Vector3(0,0,0);
    private float turningRadius;
    private Vector3 turningStartPos = new Vector3(0, 0, 0);
    private Vector3 turningEndPos = new Vector3(0, 0, 0);

    enum TurningDirection { Left, Right } ;
    TurningDirection direction;

    float currentAngle=0;

    private float height;

    // Use this for initialization
    void Start()
    {
        height = transform.position.y;
        pathManager = FindObjectOfType<PathManager>();
        startPos = pathManager.startNode.transform.position;
        startPos.y = height;
        targetPos = pathManager.nextNode.transform.position;
        targetPos.y = height;

        transform.position = startPos;
        transform.LookAt(targetPos);

    }

    // Update is called once per frame
    void Update() {

        //Debug.Log("position:" + transform.position.ToString());
        if (!turning)
        {
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= 0.1 * speed||distance<turningDistance)
            {
                getNextNode();
                calculateCenter();
                if(turningRadius>0)
                {
                    turning = true;
                    turn();
                }
                else
                {
                    transform.position = transform.position + transform.forward * speed * Time.deltaTime;  
                }
            }
            else
            {
                transform.position = transform.position + transform.forward * speed * Time.deltaTime;
            }
        }
        else if (turning)
        {
            turn();
        }
    }

    private void calculateCenter()
    {
        turningStartPos = Vector3.MoveTowards(startPos, previousPos, turningDistance);
        turningEndPos = Vector3.MoveTowards(startPos, targetPos, turningDistance);

        //Debug.Log("turningStartPos:" + turningStartPos.ToString());
        //Debug.Log("turningEndPos:"+turningEndPos.ToString());

        Vector3 v = previousPos - startPos;
        Vector3 u = startPos - targetPos;

        //Debug.Log(v.ToString());
        //Debug.Log(u.ToString());

        float a = turningStartPos.x * v.x + turningStartPos.z * v.z;
        float b = turningEndPos.x * u.x + turningEndPos.z * u.z;

        //Debug.Log(a.ToString());
        //Debug.Log(b.ToString());

        if(v.x * u.z - v.z * u.x==0)
        {
            turningCenter = startPos;
            turningRadius = 0;
        }
        else
        {
            turningCenter.x = (a * u.z - b * v.z) / (v.x * u.z - v.z * u.x);
            turningCenter.y = height;
            turningCenter.z = (a * u.x - b * v.x) / (v.z * u.x - u.z * v.x);
            turningRadius = Vector3.Distance(turningStartPos, turningCenter);
        }

        //Debug.Log("turningCenter:" + turningCenter.ToString());


        //Debug.Log("turningRadius:" + turningRadius.ToString());

        float t1 = (startPos.x - turningStartPos.x) * (turningEndPos.z - startPos.z);
        float t2 = (turningEndPos.x - startPos.x) * (startPos.z - turningStartPos.z);
        if(t1>t2)
        {
            direction = TurningDirection.Left;
        }
        else
        {
            direction = TurningDirection.Right;
        }
        currentAngle = 0;
    }

    private void turn()
    {
        float maxAngularVelocity;
        maxAngularVelocity = speed / turningRadius;
        float angle = Vector3.Angle(transform.forward, turningEndPos - startPos);
        if (angle <= Mathf.Min(turningAngularVelocity, maxAngularVelocity) * Time.deltaTime * 180 / Mathf.PI)
        {
            turning = false;
            transform.position = turningEndPos;
            transform.LookAt(targetPos);
            return;
        }

        //Debug.Log("turningAngularVelocity:" + turningAngularVelocity);
        //Debug.Log("maxAngularVelocity:" + maxAngularVelocity);

        if (direction == TurningDirection.Left)
        {
            currentAngle += Mathf.Min(turningAngularVelocity, maxAngularVelocity) * Time.deltaTime;
            transform.Rotate(0, -Mathf.Min(turningAngularVelocity, maxAngularVelocity) * Time.deltaTime * 180 / Mathf.PI, 0);
        }
        else
        {
            currentAngle -= Mathf.Min(turningAngularVelocity, maxAngularVelocity) * Time.deltaTime;
            transform.Rotate(0, Mathf.Min(turningAngularVelocity, maxAngularVelocity) * Time.deltaTime * 180 / Mathf.PI, 0);
        }
        //Debug.Log("forward:" + transform.forward.ToString());
        //Debug.Log("turningEndPos - startPos" + (turningEndPos - startPos).ToString());
        //Debug.Log("angle:" + angle1.ToString());
        //Debug.Log("target angle:" + (turningAngularVelocity * 180 / Mathf.PI).ToString());

        //Debug.Log("angle:" + angle.ToString());
        //Debug.Log("target angle:" + (turningAngularVelocity * 180 / Mathf.PI).ToString());

        float x = turningStartPos.x - turningCenter.x;
        float z = turningStartPos.z - turningCenter.z;

        //Debug.Log("x:" + x.ToString());
        //Debug.Log("z:" + z.ToString());

        float x1 = Mathf.Cos(currentAngle) * x - Mathf.Sin(currentAngle) * z;
        float z1 = Mathf.Sin(currentAngle) * x + Mathf.Cos(currentAngle) * z;


        //Debug.Log("x1:" + x1.ToString());
        //Debug.Log("z1:" + z1.ToString());

        transform.position = new Vector3(turningCenter.x + x1, height, turningCenter.z + z1);
        
        

    }

    private void getNextNode()
    {
        previousPos = startPos;
        startPos = targetPos;
        targetPos = pathManager.nextPos();
        targetPos.y = height;
    }
}
