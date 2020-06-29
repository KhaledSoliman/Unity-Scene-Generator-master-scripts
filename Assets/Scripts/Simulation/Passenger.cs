using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Simulation {
    public enum Gender {
        Male,
        Female
    }

    public enum FacialFeatures {
        Mustache,
        Beard,
        Glasses
    }

    public enum Age {
        Adult,
        Elderly,
        Baby,
        Child
    }

    public enum Distraction {
        Alert,
        Distracted,
        Sleepy
    }


    [Serializable]
    public class PassengerDistribution {
        public Utils.Distribution genderRange;
        public Utils.Distribution ageRange;

        private static List<string> _selectedPassengers = new List<string>();

        private const string PassengerPath = "Prefabs/Passenger/";

        private string[] _filterPath = new string [Enum.GetValues(typeof(Utils.PassengerIndex)).Length];

        public static void Clear() {
            _selectedPassengers.Clear();
        }

        public Passenger SelectAndBuildPassenger(GameObject parent) {
            var gender = (Gender) Utils.SelectProperty(genderRange.range);
            Debug.Log("GENDER: " + gender);
            var age = (Age) Utils.SelectProperty(ageRange.range);
            //Gender Filter
            Utils.FillPath(_filterPath, Utils.PassengerIndex.GENDER, Enum.GetName(typeof(Gender), gender),
                "Gender Specification Error!");
            //Age Filter
            Utils.FillPath(_filterPath, Utils.PassengerIndex.AGE, Enum.GetName(typeof(Age), age),
                "Age Specification Error!");
            var selectedPassenger = Utils.SelectModel(PassengerPath, Path.Combine(_filterPath), _selectedPassengers);
            _selectedPassengers.Add(selectedPassenger);
            selectedPassenger = selectedPassenger.Substring(0, selectedPassenger.LastIndexOf('.'));
            return new Passenger(gender, age, selectedPassenger, parent,
                Path.Combine(PassengerPath, Path.Combine(_filterPath), selectedPassenger));
        }
    }

    public class Passenger {
        private static List<Seat> _takenSeats = new List<Seat>();

        public RGB skinColor; //  white , brown , black
        public FacialFeatures[] facialFeatures; // "mustache" , "beard" , glasses
        public Clothes clothingColors; // RGB 
        public Distraction distractionLevel; // alert , distracted , sleepy
        public int weight; // 0 --> infinity
        public int height; // 0 --> infinity


        private Gender _gender; // (male :0, female :1)
        private Age _age; // 0 --> infinity
        private string _selectedPassenger;
        private GameObject _instance;

        public Passenger(Gender gender, Age age, string selectedPassenger, GameObject parent, string assetPath) {
            _gender = gender;
            _age = age;
            _selectedPassenger = selectedPassenger;
            Instantiate(parent, assetPath);
        }

        public void AssignSeat(Seat seat) {
            _instance.transform.localPosition = seat.position;
            _instance.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }

        public void ApplyAnimation(string controllerPath) {
            var animator = _instance.gameObject.GetComponent<Animator>();
            Debug.Log(controllerPath);
            animator.runtimeAnimatorController = Resources.Load(controllerPath) as RuntimeAnimatorController;
        }

        private void Instantiate(GameObject parent, string assetPath) {
            _instance = UnityEngine.Object.Instantiate(Resources.Load(assetPath), parent.transform, true) as GameObject;
            if (_instance == null) throw new Exception("Unable to load car asset");
            _instance.name = _selectedPassenger;
        }
    }

    [Serializable]
    public class Clothes {
        public RGB tshirt;
        public RGB pants;
    }
}