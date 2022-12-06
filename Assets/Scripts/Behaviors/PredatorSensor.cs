using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorSensor : MonoBehaviour
{
    private PredatorBehavior m_predatorBehavior;

    private void Awake()
    {
        m_predatorBehavior = transform.parent.GetComponent<PredatorBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blob"))
        {
            if (!m_predatorBehavior.m_potentialTargets.Contains(other.gameObject))
            {
                m_predatorBehavior.m_potentialTargets.Add(other.gameObject);
            }

            if (m_predatorBehavior.m_currentTarget == null && m_predatorBehavior.isInWanderZone(other.transform.position))
            {
                m_predatorBehavior.m_currentTarget = other.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_predatorBehavior.m_currentTarget != null && m_predatorBehavior.m_currentTarget.GetInstanceID() == collision.GetInstanceID())
        {
            m_predatorBehavior.m_currentTarget = null;
        }
        if (m_predatorBehavior.m_potentialTargets.Contains(collision.gameObject))
        {
            m_predatorBehavior.m_potentialTargets.Remove(collision.gameObject);
        }
    }

}
