using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    public Transform leftBound;
    public Transform rightBound;
    float lastUpdated;
    public float speed;

    public Transform destination;
    float upperLimit;
    float lowerLimit;
    float destinationBound;
    // Start is called before the first frame update
    void Start()
    {
        destination = rightBound;
        upperLimit = leftBound.position.y + .3f;
        lowerLimit = leftBound.position.y - .3f;
        destinationBound = upperLimit;
        lastUpdated = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (destination == null)
            return;

        if (Time.time - lastUpdated > 3)
        {
            destination = destination == rightBound ? leftBound : rightBound;
            transform.position = destination.position;
            lastUpdated = Time.time;
        }
        if (Mathf.Abs(transform.position.y - destinationBound) < 0.1)
        {
            if (destinationBound == upperLimit)
                destinationBound = lowerLimit;
            else
                destinationBound = upperLimit;
        }
        transform.position = Vector2.MoveTowards(transform.position, new(destination.position.x, destinationBound), speed * Time.deltaTime);

    }
}
