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

    private float m_foodLevel = SimulationManager.parameters.initialFoodLevel;

    public GenomeManager m_genome;

    private int m_daysRemaining;

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

    private Vector2 GetRandomDirectionVector(Rect rect, Vector2 origin)
    {
        const float TRIGGER_FACTOR = 0.95f;
        float xMax = rect.width / 2.0f;
        float yMax = rect.height / 2.0f;

        Vector2 random = Random.insideUnitCircle.normalized;
        if (Mathf.Abs(transform.position.x - origin.x) > xMax * TRIGGER_FACTOR)
        {
            random += new Vector2(-Mathf.Pow((transform.position.x - origin.x) / xMax, 5), 0);
        }
        if (Mathf.Abs(transform.position.y - origin.y) > yMax * TRIGGER_FACTOR)
        {
            random += new Vector2(0, -Mathf.Pow((transform.position.y - origin.y) / yMax, 5));
        }
        return random.normalized;
    }

    private bool isInStartingZone()
    {
        Vector2 localPosition = transform.position - m_startingZone.transform.position;
        if (localPosition.x < m_startingZone.rect.xMin || localPosition.x > m_startingZone.rect.xMax
            || localPosition.y < m_startingZone.rect.yMin || localPosition.y > m_startingZone.rect.yMax)
        {
            return false;
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("BodySprite").GetComponent<SpriteRenderer>().color = m_genome.getColor();
        transform.Find("Sensor").GetComponent<CircleCollider2D>().radius = m_genome.getSightRadius();

        m_daysRemaining = (int)m_genome.getLifetime();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(0, 0);

        Color debugColor = Color.black;
        
        if (m_predatorList.Count > 0) // check for predators
        {
            debugColor = Color.red;
            foreach (GameObject predator in m_predatorList)
            {
                Vector2 predatorForce = transform.position - predator.transform.position;
                predatorForce /= predatorForce.sqrMagnitude;
                direction += predatorForce;
            }
        }
        else if (m_foodLevel >= 0.998f) // return to base
        {
            debugColor = Color.yellow;
            //direction = new Vector2(m_startingZone.position.x - transform.position.x, 0);
            direction = GetComponent<Rigidbody2D>().velocity * m_genome.getMovementFactor() + GetRandomDirectionVector(m_startingZone.rect, m_startingZone.position);
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

        if (SimulationManager.isEvening())
        {
            direction += GetRandomDirectionVector(m_startingZone.rect, m_startingZone.position).normalized;
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
            Destroy(collision.gameObject);
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

    public void die()
    {
        Destroy(gameObject);
    }

    public void onDayOver()
    {
        if (!isInStartingZone())
        {
            die();
        }
        m_daysRemaining--;
        if (m_daysRemaining < 0)
        {
            die();
        }
        m_foodLevel -= m_genome.getDailyHunger();
        if (m_foodLevel <= 0.0f)
        {
            die();
        }
    }
}
