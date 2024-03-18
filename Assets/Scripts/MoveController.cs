using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float timeToReachTarget = 0.5f;
    Vector3 startPos;
    public Vector3 targetPos;
    float startTime;
    float journeyLength;

    public void StartMoving()
    {
        startPos = transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos, targetPos);
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        float elapsedTime = 0;
        while (elapsedTime < timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;
            float fractionOfJourney = elapsedTime / timeToReachTarget;
            transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
            yield return null;
        }
    }
}
