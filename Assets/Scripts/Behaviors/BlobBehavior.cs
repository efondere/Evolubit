using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BlobBehavior : MonoBehaviour
{
    private List<GameObject> m_predatorList = new List<GameObject>();
    private GameObject       m_closestFood;

    public RectTransform m_startingZone;

    private float m_foodLevel = 0.2f;

    public GenomeManager m_genome;

    private Vector2 GetRandomDirectionVector()
    {
        Vector2 random = Random.insideUnitCircle.normalized;
        if (Mathf.Abs(transform.position.y) > 4.25f)
        {
            random += new Vector2(0, -Mathf.Pow(transform.position.y / 4.45f, 5));
        }
        if (Mathf.Abs(transform.position.x) > 8.13f)
        {
            random += new Vector2(-Mathf.Pow(transform.position.x / 8.33f, 5), 0);
        }
        return random.normalized;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("BodySprite").GetComponent<SpriteRenderer>().color = m_genome.getColor();
        transform.Find("Sensor").GetComponent<CircleCollider2D>().radius = m_genome.getSightRadius();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(0, 0, 0);

        Color debugColor = Color.black;
        
        if (m_predatorList.Count > 0) // check for predators
        {
            debugColor = Color.red;
            foreach (GameObject predator in m_predatorList)
            {
                Vector3 predatorForce = transform.position - predator.transform.position;
                predatorForce /= predatorForce.sqrMagnitude;
                direction += predatorForce;
            }
        }
        else if (m_foodLevel >= 0.998f) // return to base
        {
            debugColor = Color.yellow;
            direction = new Vector3(m_startingZone.position.x - transform.position.x, 0);
        }
        else if (m_closestFood != null) // check for food
        {
            debugColor = Color.green;
            direction = m_closestFood.transform.position - transform.position;
        }
        else // move randomly
        {
            debugColor = Color.white;
            direction = GetComponent<Rigidbody2D>().velocity * m_genome.getMovementFactor() + GetRandomDirectionVector();
        }

        direction.Normalize();

        if (false /*Simulation.isDayOver()*/)
        {
            direction += (m_startingZone.position - transform.position).normalized;
            direction.Normalize();
        }

        Debug.DrawRay(transform.position, direction, debugColor, 0.001f);

        GetComponent<Rigidbody2D>().velocity = direction * m_genome.getSpeed();

        if (transform.position.y > 4.45f)
            GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, 4.45f));
        else if (transform.position.y < -4.45f)
            GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, -4.45f));
        if (transform.position.x > 8.33f)
            GetComponent<Rigidbody2D>().MovePosition(new Vector2(8.33f, transform.position.y));
        else if (transform.position.x < -8.33f)
            GetComponent<Rigidbody2D>().MovePosition(new Vector2(-8.33f, transform.position.y));
    }

    public void eat(float amount)
    {
        m_foodLevel += amount;
        if (m_foodLevel > 1.0f)
            m_foodLevel = 1.0f;

        m_closestFood = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            collision.gameObject.SetActive(false);
            eat(0.5f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    public void OnSensorTriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (m_closestFood != null)
            {
                Vector3 currentClosestFood = m_closestFood.transform.position - transform.position;
                Vector3 newFood = collision.transform.position - transform.position;

                if (newFood.sqrMagnitude < currentClosestFood.sqrMagnitude)
                {
                    m_closestFood = collision.gameObject;
                }
            }
            else
            {
                m_closestFood = collision.gameObject;
            }
        }

        if (collision.gameObject.CompareTag("Predator"))
        {
            m_predatorList.Add(collision.gameObject);
        }
    }

    public void OnSensorTriggerStay(Collider2D collision)
    {
        
    }

    public void OnSensorTriggerExit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Debug.Log(name + " has detected a food that left his sensor");
            if (m_closestFood != null)
            {
                if (collision.gameObject.GetInstanceID() == m_closestFood.GetInstanceID())
                {
                    m_closestFood = null;
                }
            }
        }

        if (collision.gameObject.CompareTag("Predator"))
        {
            m_predatorList.Remove(collision.gameObject);
        }
    }
}
