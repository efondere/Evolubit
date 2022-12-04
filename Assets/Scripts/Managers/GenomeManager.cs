using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GenomeData
{
    const int GENOME_SIZE = 50;
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
                }
                else if (parameterInt < 12)
                {
                    strength -= (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5);
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
                }
                else if (parameterInt < 32)
                {
                    lifetime = (uint)(((float)lifetime / MAX_LIFE - (float)bitArrayToInt(value) / Mathf.Pow(2, CODON_SIZE - 2 - 5)) * MAX_LIFE);
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
}

public class GenomeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        m_genomeData = new GenomeData();
    }

    void Setup(GenomeData parent1, GenomeData parent2)
    {
        m_genomeData = GenomeData.Reproduce(parent1, parent2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getSightRadius()
    {
        return Mathf.Min(m_genomeData.senseStrength * 5, 1.0f);
    }

    public float getSpeed()
    {
        // find better function
        return Mathf.Max(m_genomeData.strength / m_genomeData.weight, 2.0f);
    }

    public float getMovementFactor()
    {
        return ((m_genomeData.senseStrength + m_genomeData.weight) / 2.0f) * 50.0f;
    }

    public Color getColor()
    {
        return m_genomeData.color;
    }

    private GenomeData m_genomeData;
}
