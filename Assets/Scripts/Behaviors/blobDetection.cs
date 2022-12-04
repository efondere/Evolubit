using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blobDetection : MonoBehaviour
{
    public PredatorBehavior predatorBehavior;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blob"))
        {
  
            if (predatorBehavior.otherTargets.Count == 0)
            {
                predatorBehavior.savedTarget = other.gameObject;
                Debug.Log(predatorBehavior.savedTarget.name);
            }
            else
            {
                predatorBehavior.otherTargets.Add(other.gameObject);
            }
            predatorBehavior.target = predatorBehavior.savedTarget;
            predatorBehavior.attack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        predatorBehavior.ChooseOtherTarget(other.gameObject);
    }

}
