using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public class SimulationParameters
    {
        public float minWeight = 0.2f; // this range is arbitrary
        public float maxWeight = 0.8f;

        public float minStrength = 0.4f; // this range is arbitrary
        public float maxStrength = 1.0f;

        public float minSenseStrength = 0.1f;
        public float maxSenseStrength = 1.0f;

        public float dayLength = 15.0f; // in seconds

        public float initialFoodLevel = 0.4f; // [0.0f, 1.0f]
        public float reproductionCost = 0.05f;

        public float minRed = 0.0f;
        public float maxRed = 1.0f;
        public float minGreen = 0.0f;
        public float maxGreen = 1.0f;
        public float minBlue = 0.0f;
        public float maxBlue = 1.0f;

        public float reproductionProximity = 0.3f;
        public float twinProbability = 0.4f;

        public float mutationProbability = 0.1f; // [0.0f, 1.0f], applies to EACH copy of the parameters
        public float mutationAmplitude = 0.05f; // [0.0f, 0.5f], max mutation change
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

            float averageWeight = 0.0f;
            float averageStrength = 0.0f;
            float averageSenseStrength = 0.0f;
            Vector3 averageColor = new Vector3(0.0f, 0.0f, 0.0f);
            float averageLifetime = 0.0f;

            BlobBehavior[] blobBehaviors = FindObjectsOfType(typeof(BlobBehavior)) as BlobBehavior[];
            foreach (BlobBehavior blobBehavior in blobBehaviors)
            {
                averageWeight += blobBehavior.getGenomeData().getWeight();
                averageStrength += blobBehavior.getGenomeData().getStrength();
                averageSenseStrength += blobBehavior.getGenomeData().getSenseStength();
                Color color = blobBehavior.getGenomeData().getColor();
                averageColor += new Vector3(color.r, color.g, color.b);
                averageLifetime += blobBehavior.getGenomeData().getLifetime();
                blobBehavior.onDayOver();
            }

            averageWeight /= blobBehaviors.Length;
            averageStrength /= blobBehaviors.Length;
            averageSenseStrength /= blobBehaviors.Length;
            averageColor /= blobBehaviors.Length;
            averageLifetime /= blobBehaviors.Length;

            Debug.Log("Daily statistics report - W: " + averageWeight + ", S: " + averageStrength + ", SS: " + averageSenseStrength + ", C: " + averageColor + ", L: " + averageLifetime);

            foodGenerator.restock();
        }
    }

    public static bool isEvening()
    {
        return s_dayTime >= 0.8f;
    }
}
