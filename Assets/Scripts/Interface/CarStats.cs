using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameras;
using DataTypes;

public class CarStats : MonoBehaviour
{
    public Transform freeCamera;
    public TMPro.TMP_Text speedText;
    public GameObject panel;

    FreeCamera _freeCam;
    CarBehaviour _car;
    bool _visible = false;

    // Start is called before the first frame update
    void Start()
    {
        _freeCam = freeCamera.GetComponent<FreeCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_freeCam._following && !_visible)
        {
            Debug.Log("now");
            _visible = true;
            panel.SetActive(true);
            _car = _freeCam._targetCar.GetComponent<CarBehaviour>();
        }

        if (!_freeCam._following)
        {
            _visible = false;
            panel.SetActive(false);
        }

        if (_visible)
            speedText.text = Mathf.Round(Utility.Conversion.KilometersPerHourFromUPTU(_car._data.speed) * 10) / 10f + " km/h";
    }
}
