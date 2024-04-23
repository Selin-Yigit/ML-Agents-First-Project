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
            //ajan�n platformun s�n�rlar�ndan d���p d��medi�inin kontrol� yap�ld�.
            rgb.angularVelocity = Vector3.zero;
            rgb.velocity = Vector3.zero;
            //Ajan d��t�kten sonra yeniden platformun �st�ne yerle�ir.
            transform.localPosition = new Vector3(0, 1f, 0);
        }
        //hedefin platformdaki yeri de�i�tirildi.
        target.localPosition = new Vector3(Random.value * 8.5f - 4, 1f, Random.value * 8.5f - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Baz� �nemli verilerin g�r�nt�lenmesi ve kaydedilmesi sa�lan�r.

        //Ajan ve target'�n pozisyon bilgileri.S�rekli de�i�en bilgiler oldu�u i�in tutulmal�d�r.
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);
        //Ajan�n h�z bilgileri -> x ve z eksenlerinde hareket sa�lan�r.
        sensor.AddObservation(rgb.velocity.x);
        sensor.AddObservation(rgb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Burada aksiyonlar bulunur. Ajan�m�z�n x ve z eksenine uygulanacak ivme aksiyonlar�m�z olacak.
        //Bir aksiyon array'i tutaca��z ve bu array ajan�m�za farkl� �ekillerde ivme uygulayacak.
        //�vme bir continuous aksiyondur ��nk� s�rekli bir hareket sa�lan�r.
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rgb.AddForce(controlSignal * carpan);

        //Ajan�n hedefle aras�ndaki mesafe fark�na g�re �d�llendirme ve cezaland�rma
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
        //Buradaki ama� yazd���m�z sistemi do�rulamak. Bu sebeple klavyedeki tu�lar� kullanarak ajan� hareket ettirdik.
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
