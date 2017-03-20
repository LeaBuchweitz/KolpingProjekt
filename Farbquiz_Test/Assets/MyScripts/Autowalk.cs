using UnityEngine;
using System.Collections;

public class Autowalk : MonoBehaviour 
{
    // counts up until the right position is reached
    float countUntil = 0;

    // public variables which are changed by a special focused Object
    public Vector3 positionObject;
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
                goTowards(positionObject);
            }
        }
    } 

    // POS NOT YET NEEDED, ENDPOINT HARDCODED
    // Calculates the endpoint of the path and goes there
    public void goTowards(Vector3 pos)
    {

        Vector3 endpoint = new Vector3(-3.3f, 0.8f, 3.3f);
        Debug.Log("Endpunkt " + GameObject.Find("3-Grauflaeche fuer Simultan").GetComponent<Transform>().position);
        //Vector3 endpoint = new Vector3(pos.x -2f, pos.y + 0.1f, pos.z + 2f);

        countUntil += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endpoint, countUntil * 0.1f);
       
    }
}
