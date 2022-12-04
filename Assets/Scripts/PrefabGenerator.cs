using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{
  
    [Header("In the same order as the floats below")]
    public GameObject[] objectsToSpawn;
    [Space]
    public int blobCount;
    public int enemyCount;
    public int foodCount;
    [Space]
    public GameObject[] spawnArea;
    private int[] countList;
    // Start is called before the first frame update
    void Start()
    {
        FillFloatArray();
        CreatePool();

    }
    void FillFloatArray ()
    {
        countList = new int[3];
        countList[0] = blobCount;
        countList[1] = enemyCount;
        countList[2] = foodCount;
    }


    void CreatePool()
    {
        for (int a = 0; a < 3; a++)
        {
            for (int i = 0; i < countList[a]; i++)
            {
                PlaceObj(a);
            }
        }
    }

    void PlaceObj(int a)
    {
        Debug.Log("aaa");
        bool stopLoop = false;
        RectTransform area = spawnArea[a].GetComponent<RectTransform>();
        for (int i = 0; stopLoop == false; i++) {
            Vector3 spawnPos = area.transform.position+ new Vector3(Random.Range(area.rect.xMax, area.rect.xMin), Random.Range(area.rect.yMin, area.rect.yMax));
            Debug.Log("Colliders"+ Physics2D.OverlapCircle(spawnPos, 3));
            if (Physics2D.OverlapCircle(spawnPos, 3) == null)
            {
                Debug.Log("overlapisfalse");
                Instantiate(objectsToSpawn[a], spawnPos, Quaternion.identity);
                stopLoop = true;
            }
        }

    }
}
