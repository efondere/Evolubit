using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehavior : MonoBehaviour
{
    [HideInInspector] public bool attack;
    [HideInInspector] public GameObject target;
    public blobDetection blobDetection;
    public Rigidbody2D rb;
    public float wanderSpeed;
    public float chaseSpeed;
    public float startTimeBeforeChangeDir;
    private float timeBeforeChangeDir;
    private RectTransform wanderArea;
    private Vector3 wanderDir;
    [HideInInspector] public GameObject savedTarget;
    [HideInInspector] public List<GameObject> otherTargets;

    private void Start()
    {
        wanderArea = new RectTransform();
        wanderArea = GameObject.FindGameObjectWithTag("WanderArea").GetComponent<RectTransform>();
        otherTargets = new List<GameObject>();

    }

    private bool isInWanderZone()
    {
        Vector2 localPosition = transform.position - wanderArea.position;
        if (localPosition.x < wanderArea.rect.xMin || localPosition.x > wanderArea.rect.xMax
            || localPosition.y < wanderArea.rect.yMin || localPosition.y > wanderArea.rect.yMax)
        {
            return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        if (!isInWanderZone())
        {
            if (attack)
            {
                ChooseOtherTarget(target);
            }
            else
            {
                wanderDir = new Vector3(Random.Range(wanderArea.rect.xMax, wanderArea.rect.xMin), Random.Range(wanderArea.rect.yMax, wanderArea.rect.yMin)).normalized;
                timeBeforeChangeDir = startTimeBeforeChangeDir;
            }
        }

        if (attack)
        {
            Attack();
        }
        else
        {
            Wander();
        }
    }
    void Attack()
    {
        Vector3 attackDir = (target.transform.position - this.transform.position).normalized;
        Debug.DrawLine(this.transform.position, this.transform.position+(target.transform.position - this.transform.position).normalized);
        rb.MovePosition(this.transform.position + chaseSpeed * Time.fixedDeltaTime * attackDir);
    }

     private void OnTriggerEnter2D(Collider2D collision)
     {
         if (collision.gameObject.CompareTag("Blob")) {
            
            ChooseOtherTarget(collision.gameObject);

            Destroy(collision.gameObject);

         }
     }
    void Wander()
    {
        if (timeBeforeChangeDir <= 0)
        {
            wanderDir = new Vector3(Random.Range(wanderArea.rect.xMax, wanderArea.rect.xMin), Random.Range(wanderArea.rect.yMax, wanderArea.rect.yMin)).normalized;
            timeBeforeChangeDir = startTimeBeforeChangeDir;
        }
        else
        {
            rb.MovePosition(this.transform.position + wanderSpeed * Time.fixedDeltaTime * wanderDir);
            timeBeforeChangeDir -= Time.fixedDeltaTime;
        }

    }

    public void ChooseOtherTarget(GameObject target)
    {
        if (target == savedTarget)
        {
            if (otherTargets.Contains(savedTarget))
            {
                otherTargets.Remove(savedTarget);
            }
            float minDist = 1000;
            for (int i = 0; i < otherTargets.Count; i++)
            {
                float dist = Vector3.Distance(gameObject.transform.position, otherTargets[i].transform.position);

                if (otherTargets[i] != savedTarget && dist <= minDist)
                {
                    savedTarget = otherTargets[i];
                    minDist = dist;
                }
            }

            //if have successfully assigned target
            if (savedTarget != target)
            {
                target = savedTarget;
                attack = true;
            }
            else
            {
                attack = false;
            }


        }
        else if (otherTargets.Contains(target))
        {
            otherTargets.Remove(target);
        }
    }
}
