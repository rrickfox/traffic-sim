using UnityEngine;
using Cameras;
using DataTypes;

public class CarStats : MonoBehaviour
{
    public Transform freeCamera;
    public TMPro.TMP_Text speedText;
    public GameObject panel;

    FreeCamera _freeCam;
    Car _car;
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
            _visible = true;
            panel.SetActive(true);
            _car = (Car) _freeCam._targetCar.GetComponent<LinkedBehaviour>().data;
        }

        if (!_freeCam._following)
        {
            _visible = false;
            panel.SetActive(false);
        }

        if (_visible)
            speedText.text = Mathf.Round(Utility.Conversion.KilometersPerHourFromUPTU(_car.speed) * 10) / 10f + " km/h";
    }
}
