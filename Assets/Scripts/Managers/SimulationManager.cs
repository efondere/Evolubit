using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public class SimulationParameters
    {
        public float minWeight = 0.1f; // this range is arbitrary
        public float maxWeight = 1.0f;

        public float minStrength = 0.1f; // this range is arbitrary
        public float maxStrength = 1.0f;

        public float dayLength = 10.0f; // in seconds

        public float initialFoodLevel = 0.2f; // [0.0f, 1.0f]

        public float mutationProbability = 0.01f; // [0.0f, 1.0f], applies to EACH copy of the parameters
        public float mutationAmplitude = 0.1f; // [0.0f, 0.5f], max mutation change
    };

    public static SimulationParameters parameters = new SimulationParameters();

    private static uint s_simulationDay;
    private static float s_dayTime;

    public PrefabGenerator foodGenerator;

    // Start is called before the first frame update
    void Start()
    {
        s_simulationDay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        s_dayTime = Time.timeSinceLevelLoad / parameters.dayLength - s_simulationDay;

        if (s_dayTime >= 1.0f)
        {
            s_simulationDay++;
            s_dayTime -= 1.0f;

            BlobBehavior[] blobBehaviors = FindObjectsOfType(typeof(BlobBehavior)) as BlobBehavior[];
            foreach (BlobBehavior blobBehavior in blobBehaviors)
            {
                blobBehavior.onDayOver();
            }

            foodGenerator.restock();
        }
    }

    public static bool isEvening()
    {
        return s_dayTime >= 0.8f;
    }
}
