using System;
using System.Collections;
using UnityEngine;

namespace Simulation {
    public class RGB {
        int R;
        int G;
        int B;
    }

    [Serializable]
    public class SceneDefinition : ScriptableObject {
        public int images;
        public CarDistribution carRange;
        public Environment environment;

        public void Load(string aJson) {
            JsonUtility.FromJsonOverwrite(aJson, this);
        }

        public void BuildScene() {
            PassengerDistribution.Clear();
            carRange.SelectAndBuildCar();
            carRange.FillPassengers();
            carRange.BuildCamera();
            carRange.BuildLight();
            environment.BuildEnvironment();
        }
    }
}