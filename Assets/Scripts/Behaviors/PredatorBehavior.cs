using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehavior : MonoBehaviour
{
    private PredatorSensor m_sensor;
    [HideInInspector] public List<GameObject> m_potentialTargets = new List<GameObject>();
    [HideInInspector] public GameObject m_currentTarget;
    private RectTransform m_wanderArea;
    
    public RectTransform m_startingZone;
    public float wanderSpeed;
    public float chaseSpeed;

    //[HideInInspector] public bool attack;
    //public Rigidbody2D rb;
    //public float startTimeBeforeChangeDir;
    //private float timeBeforeChangeDir;
    //private Vector3 wanderDir;
    //[HideInInspector] public GameObject savedTarget;
    //[HideInInspector] public List<GameObject> otherTargets;

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

    private void Start()
    {
        m_sensor = gameObject.GetComponentInChildren<PredatorSensor>();

        m_wanderArea = GameObject.FindGameObjectWithTag("WanderArea").GetComponent<RectTransform>();
        //otherTargets = new List<GameObject>();
    }

    public bool isInWanderZone(Vector2 position)
    {
        Vector2 localPosition = position - new Vector2(m_wanderArea.position.x, m_wanderArea.position.y);

        if (localPosition.x < m_wanderArea.rect.xMin || localPosition.x > m_wanderArea.rect.xMax
            || localPosition.y < m_wanderArea.rect.yMin || localPosition.y > m_wanderArea.rect.yMax)
        {
            return false;
        }
    
        return true;
    }

    private void findNewTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = -1.0f;
        foreach (GameObject potentialTarget in m_potentialTargets)
        {
            if (potentialTarget == null)
            {
                continue;
            }
            if (!isInWanderZone(potentialTarget.transform.position)
                || (m_currentTarget != null && potentialTarget.GetInstanceID() == m_currentTarget.GetInstanceID()))
            {
                continue;
            }

            if (closestTarget == null)
            {
                closestTarget = potentialTarget;
                closestDistance = (closestTarget.transform.position - transform.position).sqrMagnitude;
            }
            else if ((potentialTarget.transform.position - transform.position).sqrMagnitude < closestDistance)
            {
                closestTarget = potentialTarget;
                closestDistance = (potentialTarget.transform.position - transform.position).sqrMagnitude;
            }
        }

        m_currentTarget = closestTarget;
    }

    private void FixedUpdate()
    {
        Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity + GetRandomDirectionVector(m_wanderArea.rect, m_wanderArea.position);
        newVelocity = newVelocity.normalized * wanderSpeed;

        if (SimulationManager.isEvening())
        {
            newVelocity = GetRandomDirectionVector(m_startingZone.rect, m_startingZone.position);
            newVelocity = newVelocity.normalized * wanderSpeed;
        }
        else if (m_currentTarget != null)
        {
            if (!isInWanderZone(m_currentTarget.transform.position))
            {
                findNewTarget();
            }
            else
            {
                newVelocity = m_currentTarget.transform.position - transform.position;
                newVelocity = newVelocity.normalized * chaseSpeed;
            }
        }

        GetComponent<Rigidbody2D>().velocity = newVelocity;

        for (int i = m_potentialTargets.Count - 1; i >= 0; i--)
        {
            if (m_potentialTargets[i] == null)
            {
                m_potentialTargets.RemoveAt(i);
            }
        }

        transform.rotation = Quaternion.AngleAxis(180.0f - Mathf.Atan2(GetComponent<Rigidbody2D>().velocity.y, GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg, Vector3.forward);

            //if (!isInWanderZone())
            //{
            //    if (attack)
            //    {
            //        ChooseOtherTarget(target);
            //    }
            //    else
            //    {
            //        wanderDir = new Vector3(Random.Range(wanderArea.rect.xMax, wanderArea.rect.xMin), Random.Range(wanderArea.rect.yMax, wanderArea.rect.yMin)).normalized;
            //        timeBeforeChangeDir = startTimeBeforeChangeDir;
            //    }
            //}
            //
            //if (attack)
            //{
            //    Attack();
            //}
            //else
            //{
            //    Wander();
            //}
        }
    //void Attack()
    //{
    //    Vector3 attackDir = (target.transform.position - this.transform.position).normalized;
    //    Debug.DrawLine(this.transform.position, this.transform.position+(target.transform.position - this.transform.position).normalized);
    //    rb.MovePosition(this.transform.position + chaseSpeed * Time.fixedDeltaTime * attackDir);
    //}

     private void OnTriggerEnter2D(Collider2D collision)
     {

        if (collision.gameObject.CompareTag("Blob"))
        {
            if (m_currentTarget != null && m_currentTarget.GetInstanceID() == collision.GetInstanceID())
            {
                m_currentTarget = null;
            }
            if (m_potentialTargets.Contains(collision.gameObject))
            {
                m_potentialTargets.Remove(collision.gameObject);
            }

            Destroy(collision.gameObject);
        }
     }
    //void Wander()
    //{
    //    if (timeBeforeChangeDir <= 0)
    //    {
    //        wanderDir = new Vector3(Random.Range(wanderArea.rect.xMax, wanderArea.rect.xMin), Random.Range(wanderArea.rect.yMax, wanderArea.rect.yMin)).normalized;
    //        timeBeforeChangeDir = startTimeBeforeChangeDir;
    //    }
    //    else
    //    {
    //        rb.MovePosition(this.transform.position + wanderSpeed * Time.fixedDeltaTime * wanderDir);
    //        timeBeforeChangeDir -= Time.fixedDeltaTime;
    //    }
    //
    //}

    //public void ChooseOtherTarget(GameObject target)
    //{
    //    if (target == savedTarget)
    //    {
    //        if (otherTargets.Contains(savedTarget))
    //        {
    //            otherTargets.Remove(savedTarget);
    //        }
    //        float minDist = 1000;
    //        for (int i = 0; i < otherTargets.Count; i++)
    //        {
    //            float dist = Vector3.Distance(gameObject.transform.position, otherTargets[i].transform.position);
    //
    //            if (otherTargets[i] != savedTarget && dist <= minDist)
    //            {
    //                savedTarget = otherTargets[i];
    //                minDist = dist;
    //            }
    //        }
    //
    //        //if have successfully assigned target
    //        if (savedTarget != target)
    //        {
    //            target = savedTarget;
    //            attack = true;
    //        }
    //        else
    //        {
    //            attack = false;
    //        }
    //
    //
    //    }
    //    else if (otherTargets.Contains(target))
    //    {
    //        otherTargets.Remove(target);
    //    }
    //}
}
