using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/*
public class GenomeData
{
    const int GENOME_SIZE = 20;
    const int CODON_SIZE = 13;
    const int MAX_LIFE = 10;
    const bool MUTATIONS_ENABLED = false;

    BitArray genome = new BitArray(GENOME_SIZE * 2 * CODON_SIZE);

    public float weight;
    public float strength;
    public Color color = Color.white;
    public float senseStrength;
    public uint  lifetime;

    int bitArrayToInt(BitArray array, int start = 0, int end = 32)
    {
        if (array.Length - start > 32) return 0;
        int[] result = new int[1];

        if (start == 0 && end == 32)
        {
            array.CopyTo(result, 0);
        }
        else
        {
            BitArray newArray = new BitArray(end - start);
            for (int i = 0; i < newArray.Length; ++i)
            {
                newArray.Set(i, array.Get(start + i));
            }
            newArray.CopyTo(result, 0);
        }

        return result[0];
    }

    private void computePhenotype()
    {
        // operations (bitwise or?)
        // 00: set
        // 01: add
        // 10: subtract
        // 11: multiply

        // parameters
        // weight: 0 - 3
        // strength: 4 - 7
        // color: 8 - 10
        // senseStrength: 11 - 15
        // lifetime: 15 - 16

        for (int i = 0; i < GENOME_SIZE * 2; ++i)
        {
            BitArray op = new BitArray(2);
            BitArray parameter = new BitArray(5);
            BitArray value = new BitArray(CODON_SIZE - 2 - 5);
            for (int copyIndex = 0; copyIndex < 2; copyIndex++)
            {
                op.Set(copyIndex, genome.Get(i * CODON_SIZE + copyIndex));
            }
            for (int copyIndex = 0; copyIndex < 5; copyIndex++)
            {
                parameter.Set(copyIndex, genome.Get(i * CODON_SIZE + 2 + copyIndex));
            }
            for (int copyIndex = 0; copyIndex < CODON_SIZE - 2 - 5; copyIndex++)
            {
                value.Set(copyIndex, genome.Get(i * CODON_SIZE + 2 + 5 + copyIndex));
            }

            int opInt = bitArrayToInt(op);
            int parameterInt = bitArrayToInt(parameter);

            if (opInt == 0)
            {
                if (parameterInt < 6)
                {
                    weight = (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 12)
                {
                    strength = (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 18)
                {
                    int r = bitArrayToInt(value, 0, 2);
                    int g = bitArrayToInt(value, 2, 4);
                    int b = bitArrayToInt(value, 4, 6);
                    color = new Color((float)(r) / 3.0f, (float)(g) / 3.0f, (float)(b) / 3.0f);
                }
                else if (parameterInt < 24)
                {
                    senseStrength = (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 32)
                {
                    lifetime = (uint)((float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5) * MAX_LIFE);
                }
            }
            if (opInt == 1) // add
            {
                if (parameterInt < 6)
                {
                    weight += (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 12)
                {
                    strength += (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 18)
                {
                    int r = bitArrayToInt(value, 0, 2);
                    int g = bitArrayToInt(value, 2, 4);
                    int b = bitArrayToInt(value, 4, 6);
                    color += new Color((float)(r) / 3.0f, (float)(g) / 3.0f, (float)(b) / 3.0f);
                }
                else if (parameterInt < 24)
                {
                    senseStrength += (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 32)
                {
                    lifetime = (uint)(((float)lifetime / MAX_LIFE + (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5)) * MAX_LIFE);
                }
            }
            if (opInt == 2) // subtract
            {
                if (parameterInt < 6)
                {
                    weight -= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                    if (weight < 0.0f) weight = 0.0f;
                }
                else if (parameterInt < 12)
                {
                    strength -= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                    if (strength < 0.0f) strength = 0.0f;
                }
                else if (parameterInt < 18)
                {
                    int r = bitArrayToInt(value, 0, 2);
                    int g = bitArrayToInt(value, 2, 4);
                    int b = bitArrayToInt(value, 4, 6);
                    color -= new Color((float)(r) / 3.0f, (float)(g) / 3.0f, (float)(b) / 3.0f);
                    color.a = 1.0f;
                }
                else if (parameterInt < 24)
                {
                    senseStrength -= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                    if (senseStrength < 0.12f) senseStrength = 0.12f;
                }
                else if (parameterInt < 32)
                {
                    lifetime = (uint)(((float)lifetime / MAX_LIFE - (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5)) * MAX_LIFE);
                    if (lifetime < 1) lifetime = 1;
                }
            }
            if (opInt == 3) // multiply
            {
                if (parameterInt < 6)
                {
                    weight *= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 12)
                {
                    strength *= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 18)
                {
                    int r = bitArrayToInt(value, 0, 2);
                    int g = bitArrayToInt(value, 2, 4);
                    int b = bitArrayToInt(value, 4, 6);
                    color *= new Color((float)(r) / 3.0f, (float)(g) / 3.0f, (float)(b) / 3.0f);
                }
                else if (parameterInt < 24)
                {
                    senseStrength *= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
                }
                else if (parameterInt < 32)
                {
                    lifetime = (uint)(((float)lifetime / MAX_LIFE * (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5)) * MAX_LIFE);
                }
            }
        }
    }

    public GenomeData()
    {
        System.Random random = new System.Random();

        for (int i = 0; i < GENOME_SIZE * 2 * CODON_SIZE; ++i)
        {
            genome.Set(i, (random.Next() % 2 == 0) ? false : true);
        }

        computePhenotype();
    }

    GenomeData(BitArray _genome)
    {
        genome = _genome;
        computePhenotype();
    }

    internal static GenomeData Reproduce(GenomeData parent1, GenomeData parent2)
    {
        throw new NotImplementedException();
    }
}*/

class GenomeData
{
    public float[] weight = new float[2];
    public float[] strength = new float[2];
    public float[] senseStrength = new float[2];
    public Color[] color = new Color[2];
    public uint[] lifetime = new uint[2];

    public GenomeData()
    {
        for (int i = 0; i < 2; i++)
        {
            weight[i] = UnityEngine.Random.Range(0.1f, 1.0f);
            strength[i] = UnityEngine.Random.Range(0.1f, 1.0f);
            senseStrength[i] = UnityEngine.Random.Range(0.1f, 1.0f);
            color[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            lifetime[i] = (uint)Mathf.Max(UnityEngine.Random.value * 5.0f, 1.0f);
        }
    }

    public static GenomeData Reproduce(GenomeData parent1, GenomeData parent2)
    {
        GenomeData data = new GenomeData();
        data.weight[0] = parent1.weight[UnityEngine.Random.Range(0, 2)];
        data.weight[1] = parent2.weight[UnityEngine.Random.Range(0, 2)];
        data.strength[0] = parent1.strength[UnityEngine.Random.Range(0, 2)];
        data.strength[1] = parent2.strength[UnityEngine.Random.Range(0, 2)];
        data.senseStrength[0] = parent1.senseStrength[UnityEngine.Random.Range(0, 2)];
        data.senseStrength[1] = parent2.senseStrength[UnityEngine.Random.Range(0, 2)];
        data.color[0] = parent1.color[UnityEngine.Random.Range(0, 2)];
        data.color[1] = parent2.color[UnityEngine.Random.Range(0, 2)];
        data.lifetime[0] = parent1.lifetime[UnityEngine.Random.Range(0, 2)];
        data.lifetime[1] = parent2.lifetime[UnityEngine.Random.Range(0, 2)];
        data.applyMutations();
        return data;
    }

    private float randomMutationValue()
    {
        if (UnityEngine.Random.value >= 0.05f)
        {
            return 0.0f;
        }

        float a = 0.05f;
        float k = -a / 2.0f;
        float c = 1.0f;
        float x = UnityEngine.Random.Range(-1.0f, 1.0f);

        return (a / (1 + Mathf.Exp(-c * x))) + k;
    }

    private void applyMutations()
    {
        for (int i = 0; i < 2; i++)
        {
            weight[i] += randomMutationValue();
            strength[i] += randomMutationValue();
            senseStrength[i] += randomMutationValue();
            color[i] += new Color(randomMutationValue(), randomMutationValue(), randomMutationValue());
            if (UnityEngine.Random.value < 0.05)
            {
                int change = UnityEngine.Random.Range(-1, 2);
                if (change == -1) lifetime[i]--;;
                if (change == 1) lifetime[i]++;;
            }
        }
    }

    public float getWeight()
    {
        return (weight[0] + weight[1]) / 2.0f;
    }
    public float getStrength()
    {
        return (strength[0] + strength[1]) / 2.0f;
    }
    public float getSenseStength()
    {
        return (senseStrength[0] + senseStrength[1]) / 2.0f;
    }
    public Color getColor()
    {
        Vector3 color1 = new Vector3(color[0].r, color[0].g, color[0].b);
        Vector3 color2 = new Vector3(color[1].r, color[1].g, color[1].b);
        Vector3 average = color1 + color2;
        average /= 2.0f;

        return new Color(Mathf.Min(average.x, 1.0f), Mathf.Min(average.y, 1.0f), Mathf.Min(average.z, 1.0f));
    }
    public uint getLifetime()
    {
        return (uint)Mathf.CeilToInt((lifetime[0] + lifetime[1]) / 2.0f);
    }
}

public class GenomeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        m_genomeData = new GenomeData();
    }

    void CreateOffspring(GenomeData parent1, GenomeData parent2)
    {
        m_genomeData = GenomeData.Reproduce(parent1, parent2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getSightRadius()
    {
        return m_genomeData.getSenseStength() * 5;
    }

    public float getSpeed()
    {
        return (((Mathf.Pow(m_genomeData.getStrength(), 2.0f) - Mathf.Pow(m_genomeData.getWeight(), 2.0f)) / 2.0f) + 0.5f) * 10.0f;
    }

    public float getMovementFactor()
    {
        return ((m_genomeData.getSenseStength() + m_genomeData.getWeight()) / 2.0f) * 50.0f / getSpeed();
    }

    public Color getColor()
    {
        return m_genomeData.getColor();
    }

    public uint getLifetime()
    {
        return m_genomeData.getLifetime();
    }

    public float getDailyHunger()
    {
        return m_genomeData.getWeight() * 0.7f;
    }

    private GenomeData m_genomeData;
}
