using Cameras;
using DataTypes;
using UnityEngine;

namespace Interface
{
    public class CarStats : MonoBehaviour
    {
        public Transform freeCamera;
        public TMPro.TMP_Text speedText;
        public TMPro.TMP_Text frontDistanceText;
        public GameObject panel;

        private FreeCamera _freeCam;
        private Car _car;
        private bool _visible = false;

        // Start is called before the first frame update
        private void Start()
        {
            _freeCam = freeCamera.GetComponent<FreeCamera>();
        }

        // Update is called once per frame
        private void Update()
        {
            // if freeCam is following a car, then display the stats otherwise not
            if (_freeCam._following)
            {
                if (!_visible)
                {
                    _visible = true;
                    panel.SetActive(true);
                    _car = (Car) _freeCam._targetCar.GetComponent<LinkedBehaviour>().data;
                }
            }
            else
            {
                _visible = false;
                panel.SetActive(false);
            }

            // display speed of car
            if (_visible)
            {
                speedText.text = Mathf.Round((float) _car.speed.KilometersPerHour * 10) / 10f + " km/h";
                var frontCar = _car.GetFrontCar();
                if(frontCar != null)
                    frontDistanceText.text = Mathf.Round((float) ((frontCar.positionOnRoad - _car.positionOnRoad) - ((_car.length + frontCar.length) / 2) - _car.bufferDistance).Meters * 10f) / 10f + " m";
            }
        }
    }
}
