using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MyAgent : Agent
{
    private Rigidbody rgb;
    public Transform target;
    public float carpan = 6f;
    void Start()
    {
        rgb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if(transform.localPosition.y <0)
        {
            //ajanýn platformun sýnýrlarýndan düþüp düþmediðinin kontrolü yapýldý.
            rgb.angularVelocity = Vector3.zero;
            rgb.velocity = Vector3.zero;
            //Ajan düþtükten sonra yeniden platformun üstüne yerleþir.
            transform.localPosition = new Vector3(0, 1f, 0);
        }
        //hedefin platformdaki yeri deðiþtirildi.
        target.localPosition = new Vector3(Random.value * 8.5f - 4, 1f, Random.value * 8.5f - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Bazý önemli verilerin görüntülenmesi ve kaydedilmesi saðlanýr.

        //Ajan ve target'ýn pozisyon bilgileri.Sürekli deðiþen bilgiler olduðu için tutulmalýdýr.
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);
        //Ajanýn hýz bilgileri -> x ve z eksenlerinde hareket saðlanýr.
        sensor.AddObservation(rgb.velocity.x);
        sensor.AddObservation(rgb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Burada aksiyonlar bulunur. Ajanýmýzýn x ve z eksenine uygulanacak ivme aksiyonlarýmýz olacak.
        //Bir aksiyon array'i tutacaðýz ve bu array ajanýmýza farklý þekillerde ivme uygulayacak.
        //Ývme bir continuous aksiyondur çünkü sürekli bir hareket saðlanýr.
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rgb.AddForce(controlSignal * carpan);

        //Ajanýn hedefle arasýndaki mesafe farkýna göre ödüllendirme ve cezalandýrma
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        if(distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Buradaki amaç yazdýðýmýz sistemi doðrulamak. Bu sebeple klavyedeki tuþlarý kullanarak ajaný hareket ettirdik.
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
