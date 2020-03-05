using Cameras;
using DataTypes;
using UnityEngine;

namespace Interface
{
    public class CarStats : MonoBehaviour
    {
        public Transform freeCamera;
        public TMPro.TMP_Text speedText;
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
}
