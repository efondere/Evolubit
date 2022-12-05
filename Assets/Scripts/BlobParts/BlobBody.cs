using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBody : MonoBehaviour
{
    private BlobBehavior m_blobBehavior;

    // Start is called before the first frame update
    void Start()
    {
        m_blobBehavior = transform.parent.GetComponent<BlobBehavior>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            collision.gameObject.SetActive(false);
            m_blobBehavior.eat(0.5f);
        }
    }
}
