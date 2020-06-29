using System;
using UnityEngine;

namespace Simulation {
    public enum Season {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public enum Weather {
        Sunny,
        Raining,
        Cloudy,
        Windy,
        Storm,
        Blizzard,
        Sandstorm
    }
    
    public enum Time {
        Day,
        Sunset,
    }

    [Serializable] 
    public class Environment {
        public bool fog;
        public FogMode fogMode = FogMode.ExponentialSquared;
        public float fogDensity = 0.01f;
        public float linearFogStart = 0.0f;
        public float linearFogEnd = 300.0f;
        public float haloStrength = 0.5f;
        public float flareStrength = 1.0f;
        
        public Utils.Distribution timeRange;
        private Time _time;
        
        public void BuildEnvironment() {
            RenderSettings.ambientLight = UnityEngine.Color.white;
            RenderSettings.ambientIntensity = 2;
            _time = (Time) Utils.SelectProperty(timeRange.range);
            RenderSettings.skybox = _time == Time.Day ? Resources.Load<Material>("Skybox/Materials/Skybox_Daytime") : Resources.Load<Material>("Skybox/Materials/Skybox_Sunset");
        }
    }
}