using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationService : MonoBehaviour
{
    public Text locationMessage;
    IEnumerator Start()
    {
        //while (!UnityEditor.EditorApplication.isRemoteConnected)
        //{
        //    Debug.Log("Unity Remote no está conectado");
        //    yield break;
        //}

        if (!Input.location.isEnabledByUser)
        {
            //Debug.Log("Servicio no activado");
            yield break;
        }

        Input.location.Start(10.0f, 10.0f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            //Debug.Log("Se acabó el tiempo");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //Debug.Log("No se puede determinar la ubicación");
            yield break;
        }
        else
        {
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        locationMessage.text = "Latitud: " + Input.location.lastData.latitude.ToString()
                + "Longitud: " + Input.location.lastData.longitude.ToString()
                + "Altitud: " + Input.location.lastData.altitude.ToString()
                + "Exactitud horizontal: " + Input.location.lastData.horizontalAccuracy.ToString()
                + "Tiempo: " + Input.location.lastData.timestamp.ToString();
        //Debug.Log("Latitud: " + Input.location.lastData.latitude
        //        + "Longitud: " + Input.location.lastData.longitude
        //        + "Altitud: " + Input.location.lastData.altitude
        //        + "Exactitud horizontal: " + Input.location.lastData.horizontalAccuracy
        //        + "Tiempo: " + Input.location.lastData.timestamp);
    }
}
