using UnityEngine;
using System.Collections;

public class Autowalk : MonoBehaviour 
{
    // counts up until the right position is reached
    float countUntil = 0;

    // public variables which are changed by a special focused Object
    public Vector3 positionObject;
    public Vector3 endPoint;
    // is true if the correct object is focused (maybe an int for several)
    public bool correctObjFocused = false;

    // if the correct Object is focused goTowards() is called which moves camera to that Object
    public void Update()
    {
        if (correctObjFocused)
        {
            // counts float up to avoid "while-forever" because of floats
            if (countUntil < 1)
            {
                goTowards(endPoint);
            } else
            {
                countUntil = 0;
                correctObjFocused = false;
            }
        }
    } 

    // POS NOT YET NEEDED, ENDPOINT HARDCODED
    // Calculates the endpoint of the path and goes there
    public void goTowards(Vector3 pos)
    {
        countUntil += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pos, countUntil * 0.1f);
       
    }
}
