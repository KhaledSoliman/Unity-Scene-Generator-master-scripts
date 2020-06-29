using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

namespace Simulation {
    public enum Type {
        Coupe,
        Sedan,
        SUV,
        Hatchback,
        Convertible
    }

    public enum Accessory {
        Tissue,
        Coffee,
        BabySeat,
        Bag
    }

    public enum Color {
        Red
    }

    public enum Texture {
        Leather,
        Fabric
    }

    [Serializable]
    public class Seat {
        public Vector3 position;
    }

    [Serializable]
    public class AbsolutePosition {
        public Seat[] seats;
        public CameraDistribution cameraRange;

        public void Load(string aJson) {
            JsonUtility.FromJsonOverwrite(aJson, this);
        }
    }


    [Serializable]
    public class LightDistribution {
        public Utils.Distribution intensityRange;
        private Light _light;

        public void BuildLight(GameObject parent) {
            var intensity = Utils.SelectProperty(intensityRange.range);
            _light = new Light(intensity, parent);
        }
    }

    public class Light {
        private float _intensity;
        private GameObject _instance;

        public Light(float intensity, GameObject parent) {
            _intensity = intensity;
            Instantiate(parent);
        }

        public void Instantiate(GameObject parent) {
            _instance = new GameObject("Generated Light");
            _instance.AddComponent<UnityEngine.Light>();
            _instance.GetComponent<UnityEngine.Light>().color = UnityEngine.Color.white;
            _instance.GetComponent<UnityEngine.Light>().type = LightType.Directional;
            _instance.GetComponent<UnityEngine.Light>().intensity = _intensity;
            _instance.transform.parent = parent.transform;
            _instance.transform.position = new Vector3(0, 0, 0);
        }
    }


    [Serializable]
    public class CarDistribution {
        public PassengerDistribution passengerRange;
        public Utils.Distribution numPassengersRange;
        public Utils.Distribution typeRange;
        public Utils.Distribution colorRange;
        public LightDistribution lightRange;


        private const string CarPath = "Prefabs/Car/";
        private Car _car;
        private Camera _camera;

        public void SelectAndBuildCar() {
            var filterPath = new string[Enum.GetValues(typeof(Utils.CarIndex)).Length];
            var type = (Type) Utils.SelectProperty(typeRange.range);
            var color = (Color) Utils.SelectProperty(colorRange.range);
            var numPassengers = Utils.SelectProperty(numPassengersRange.range);
            Debug.Log("Type: " + type + ", Color: " + color + ", Numpass: " + numPassengers);
            //Type Filter
            Utils.FillPath(filterPath, Utils.CarIndex.Type, Enum.GetName(typeof(Type), type),
                "Car Type Specification Error!");
            //Color Filter
            Utils.FillPath(filterPath, Utils.CarIndex.Color, Enum.GetName(typeof(Color), color),
                "Car Type Specification Error!");
            var selectedCar = Utils.SelectModel(CarPath, Path.Combine(filterPath));
            selectedCar = selectedCar.Substring(0, selectedCar.LastIndexOf('.'));
            Debug.Log("Model of Car: " + selectedCar);
            _car = new Car(type, color, numPassengers, selectedCar,
                Path.Combine(CarPath, Path.Combine(filterPath), selectedCar));
        }

        public void BuildLight() {
            lightRange.BuildLight(_car.Instance);
        }

        public void BuildCamera() {
            _car.BuildCamera();
        }

        public void FillPassengers() {
            var driver = passengerRange.SelectAndBuildPassenger(_car.Instance);
            _car.AssignSeating(driver, true);
            foreach (var i in Enumerable.Range(1, _car.NumPassengers - 1)) {
                var passenger = passengerRange.SelectAndBuildPassenger(_car.Instance);
                _car.AssignSeating(passenger);
            }
        }
    }

    public class Car {
        public int NumPassengers { get; }
        public GameObject Instance { get; private set; }

        public List<Tuple<Passenger, Seat>> Passengers => _passengers;

        //public Camera camera;
        // public Accessory[] carAccessories; //objects in car ie:tissue box
        private Type _type;
        private Color _color;
        private string _selectedCar;
        private AbsolutePosition _positions;
        private List<Tuple<Passenger, Seat>> _passengers = new List<Tuple<Passenger, Seat>>();

        public Car(Type type, Color color, int numPassengers, string selectedCar, string assetPath) {
            _type = type;
            _color = color;
            NumPassengers = numPassengers;
            _selectedCar = selectedCar;
            LoadAbsolutePositions();
            Instantiate(assetPath);
        }

        private void LoadAbsolutePositions() {
            var targetFile = Resources.Load<TextAsset>("Config/" + _selectedCar);
            _positions = new AbsolutePosition();
            _positions.Load(targetFile.text);
        }

        public void AssignSeating(Passenger passenger, bool driver = false) {
            var seatPosition = driver ? 0 : _passengers.Count;
            passenger.AssignSeat(_positions.seats[seatPosition]);
            var filterPath = new string[2];
            filterPath[0] = "Animators";
            filterPath[1] = driver ? "Driver" : "Passenger";
            var selectedAnimation = Utils.SelectModel("Prefabs/Animations/", Path.Combine(filterPath));
            selectedAnimation = selectedAnimation.Substring(0, selectedAnimation.LastIndexOf('.'));
            passenger.ApplyAnimation(Path.Combine("Prefabs/Animations/", Path.Combine(filterPath),
                selectedAnimation));
            var seating = new Tuple<Passenger, Seat>(passenger, _positions.seats[seatPosition]);
            _passengers.Add(seating);
        }

        private void Instantiate(string assetPath) {
            Instance = UnityEngine.Object.Instantiate(Resources.Load(assetPath)) as GameObject;
            if (Instance == null) throw new Exception("Unable to load car asset");
            Instance.name = _selectedCar;
            Instance.transform.position = new Vector3(0, 0, 0);
        }

        public void BuildCamera() {
            _positions.cameraRange.BuildCamera(Instance);
        }
    }
}