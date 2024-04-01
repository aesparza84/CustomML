using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BirdAgent : Agent
{
    private Vector3 targetPosition;
    private Vector3 StartingPositon;

    [SerializeField] private Transform Rings;
    private Transform[] RingArray;

    private int CurrentRingIndex;
    private int MaxRingIndex;

    [SerializeField] private float ForceMulitplier;
    private Rigidbody agentRb;

    private float Pitch;
    private float Yaw;
    private float MaxPitch;
    void Start()
    {
        StartingPositon = transform.position;

        CurrentRingIndex = 0;

        if (Rings != null)
        {
            RingArray = new Transform[Rings.childCount];
            for (int i = 0; i < Rings.childCount; i++)
            {
                RingArray[i] = Rings.GetChild(i);
            }
        }

        //If the array is populated and not null
        if (RingArray.Length>0)
        {
            MaxRingIndex = RingArray.Length - 1;
        }

        //Reference the rigidbody that will move the agent
        if (TryGetComponent<Rigidbody>(out Rigidbody r))
        {
            agentRb = r;
        }

        MaxPitch = 80;
        Pitch = 0.0f;
        Yaw = 0.0f;
    }

    public override void OnEpisodeBegin()
    {
        //Reset the bird to the start
        gameObject.transform.position = StartingPositon;
        
        //Set the target to be the First ring in array
        CurrentRingIndex = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //TODO: Add observations about the learning environment
       
        //Agents current position
        sensor.AddObservation(gameObject.transform.position);
    }


    //Bird agent controls used from ML_Hummingbird tutorial
    //https://learn.unity.com/tutorial/onactionreceived?uv=2019.3&courseId=5e470160edbc2a15578b13d7&projectId=5ec830c4edbc2a44309bf21c#
    /// <summary>
    /// Index 0: x vector (1 right, -1 left)
    /// Index 1: y vector (1 up, -1 down)
    /// Index 2: z vector (1 forward, -1 backwards)
    /// Index 3: Pitch (1 pitch up, -1 pitch Down)
    /// Index 4: Yaw (1 turn right, -1 turn left)
    /// </summary>
    /// <param name="actions"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        //TODO: Add inputs for the 'bird' to fly

        //Set the input vector for movement
        Vector3 vectorInput = Vector3.zero;
        vectorInput.x = actions.ContinuousActions[0];
        vectorInput.y = actions.ContinuousActions[1];
        vectorInput.z = actions.ContinuousActions[2];
        
        //Add force to agents Rigidbody
        agentRb.AddForce(vectorInput * ForceMulitplier);

        //Current rotation
        Vector3 currentRotation = gameObject.transform.rotation.eulerAngles;
            
        float newPitch = actions.ContinuousActions[3];
        float newYaw = actions.ContinuousActions[4];

        //Smooth rotation
        Pitch = Mathf.MoveTowards(Pitch, newPitch, 2.0f * Time.fixedDeltaTime);
        Yaw = Mathf.MoveTowards(Yaw, newYaw, 2.0f * Time.fixedDeltaTime);

        float pitch = currentRotation.x + Pitch * Time.fixedDeltaTime * 100.0f;
        if (pitch > 180f)
        {
            pitch -= 360.0f;
        }
        pitch = Mathf.Clamp(pitch, -MaxPitch, MaxPitch);

        float yaw = currentRotation.y + Yaw * Time.fixedDeltaTime * 100.0f;

        //apply pitch and yaw
        gameObject.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //TODO: Add heuristic inputs for testing control
        var continuousActionsOut = actionsOut.ContinuousActions;

        Vector3 right = Vector3.zero;
        Vector3 up = Vector3.zero;
        Vector3 forward = Vector3.zero;

        float pitch = 0.0f;
        float yaw = 0.0f;

        if (Input.GetKey(KeyCode.A))
        {
            right = -transform.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            right = transform.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            forward = transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forward = -transform.forward;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            up = -transform.up;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            up = transform.up;
        }

        if (Input.GetKey(KeyCode.Comma))
        {
            yaw = -1.0f;   
        }
        else if (Input.GetKey(KeyCode.Period))
        {
            yaw = 1.0f;
        }

        if (Input.GetKey(KeyCode.Semicolon))
        {
            pitch = 1.0f;
        }
        else if (Input.GetKey(KeyCode.Slash))
        {
            pitch = -1.0f;
        }

        Vector3 directionVector = (forward + up + right).normalized;

        continuousActionsOut[0] = directionVector.x;
        continuousActionsOut[1] = directionVector.y;
        continuousActionsOut[2] = directionVector.z;
        continuousActionsOut[3] = pitch;
        continuousActionsOut[4] = yaw;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enterd");       
    }
}
