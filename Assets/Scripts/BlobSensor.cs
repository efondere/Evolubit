using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSensor : MonoBehaviour
{
    private BlobBehavior m_parentBehavior;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        m_parentBehavior = transform.parent.GetComponent<BlobBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_parentBehavior.OnSensorTriggerEnter(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        m_parentBehavior.OnSensorTriggerStay(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_parentBehavior.OnSensorTriggerExit(collision);
    }
}
