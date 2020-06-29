using System;
using UnityEngine;

namespace Simulation {
    [Serializable]
    public class CameraDistribution {
        public Vector3[] angleRange;
        public Vector3[] positionRange;
        public float fov;
        private Cam _cam;

        public void BuildCamera(GameObject parent) {
            var angle = Utils.SelectVector3(angleRange);
            var position = Utils.SelectVector3(positionRange);
            _cam = new Cam(angle, position, fov, parent);
        }
    }

    public class Cam {
        private Vector3 _angle;
        private Vector3 _position;
        private float _fov;

        private GameObject _instance;

        public Cam(Vector3 angle, Vector3 position, float fov, GameObject parent) {
            _angle = angle;
            _position = position;
            _fov = fov;
            Instantiate(parent);
        }

        private void Instantiate(GameObject parent) {
            _instance = new GameObject {name = "Generated Camera"};
            _instance.AddComponent<Camera>();
            _instance.transform.parent = parent.transform;
            _instance.transform.localPosition = _position;
            _instance.transform.localRotation = Quaternion.Euler(_angle);
            _instance.GetComponent<Camera>().fieldOfView = _fov;
        }
    }
}