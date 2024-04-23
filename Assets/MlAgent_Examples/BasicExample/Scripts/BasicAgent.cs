using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BasicAgent : Agent
{
    private Rigidbody rgb;
    public Transform BigBall;
    public Transform SmallBall;
    public float force = 40f;
    void Start()
    {
        rgb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y<0f)
        {
            rgb.angularVelocity = Vector3.zero;
            rgb.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, 0.5f, 0f);
        }
        BigBall.localPosition = new Vector3(Random.value*13.5f-8, 0.5f, Random.value * 13.5f - 8);
        SmallBall.localPosition = new Vector3(Random.value * 13.5f - 8, 0.23f, Random.value * 13.5f - 8);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Konum
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(BigBall.localPosition);
        sensor.AddObservation(SmallBall.localPosition);

        //Hýz
        sensor.AddObservation(rgb.velocity.x);
        sensor.AddObservation(rgb.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rgb.AddForce(controlSignal * force);

        float bigBallDistance = Vector3.Distance(transform.localPosition, BigBall.localPosition);
        float smallBallDistance = Vector3.Distance(transform.localPosition, SmallBall.localPosition);

        if (bigBallDistance<1f)
        {
            SetReward(2f);
            EndEpisode();
        }
        if (smallBallDistance<1f)
        {
            SetReward(0.1f);
            EndEpisode();
        }
        if (transform.localPosition.y<-0.1f)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}
