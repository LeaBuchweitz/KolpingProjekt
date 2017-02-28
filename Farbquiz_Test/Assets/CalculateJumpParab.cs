using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateJumpParab : MonoBehaviour {

    private Vector3 quarter;
    private Vector3 middle;
    private Vector3 threeQuarter;

    private GameObject cam;
    private GameObject startJump;
    private GameObject quarterJump;
    private GameObject halfJump;
    private GameObject threeQuarterJump;
    private GameObject endJump;
    private GameObject[] canvas;

    private float timer;
    private bool disappear;

	// Use this for initialization
	void Start () {

        // Get Reference to camera for FollowSpline()
        cam = GameObject.Find("CardboardMain");

        // Get references to the EmptyObjects (children) for spline locations
        startJump = GameObject.Find("1startJump");
        quarterJump = GameObject.Find("2QuarterJump");
        halfJump = GameObject.Find("3HalfJump");
        threeQuarterJump = GameObject.Find("4ThreeQuarterJump");
        endJump = GameObject.Find("5endJump");

        timer = 0.0f;
        disappear = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
        // starts timer to delete question and answers
        if(disappear)
        {
            timer += Time.deltaTime;
        }

        if(timer > 3.0f)
        {
            // gets every component of the question and answers
            canvas = GameObject.FindGameObjectsWithTag("Canvas");
            // deletes every question component
            foreach(GameObject obj in canvas)
            {
                Destroy(obj);
            }

            // reset 
            disappear = false;
            timer = 0.0f;
        }
	}

    public void calculateLocalParab(Vector3 start, Vector3 end)
    {
        //Debug.Log("Start: " + start + " Ende: " + end);

        // Richtungsvektor end - start
        Vector3 direction = new Vector3(end.x + start.x, end.y - start.y, end.z - start.z);
        //Debug.Log("Richtungsvektor: " + direction);

        // Mittelpunkt der Animation: start-Vektor + (0.5 * Richtungsvektor) --- max Höhe des Sprungs: y + 1.3
        middle = new Vector3(start.x + (0.5f * direction.x), start.y + (0.5f * direction.y) + 1.3f, start.z + (0.5f * direction.z));
        //Debug.Log("Mittelpunkt des Paraboloids: " + middle);

        // Viertel der Animation: start-Vektor + (0.25 * Richtungsvektor) -- halbe Höhe des Sprungs: y + 1.0
        quarter = new Vector3(start.x + (0.25f * direction.x), start.y + (0.25f * direction.y) +  1f, start.z + (0.25f * direction.z));
        //Debug.Log("Viertel: " + quarter);

        // Dreiviertel der Animation: start-Vektor + (0.75 * Richtungsvektor) -- halbe Höhe des Sprungs: y + 1.0
        threeQuarter = new Vector3(start.x + (0.75f * direction.x), start.y + (0.75f * direction.y) + 1f, start.z + (0.75f * direction.z));
        //Debug.Log("3/4: " + threeQuarter);

        // add calculated locations to the EmptyObjects
        // start- und end-Höhe des Sprungs auf y + 0.5 (schon in den Parametern)
        startJump.GetComponent<Transform>().position = start;
        quarterJump.GetComponent<Transform>().position = quarter;
        halfJump.GetComponent<Transform>().position = middle;
        threeQuarterJump.GetComponent<Transform>().position = threeQuarter;
        endJump.GetComponent<Transform>().position = end;

        // start animation
        cam.GetComponent<SplineController>().FollowSpline();

        // starts timer for begin to make the question and answers disappear
        disappear = true;
    }
}
