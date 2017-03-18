using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CalculateJumpParab : MonoBehaviour {

    private Vector3 down;
    private Vector3 quarter;
    private Vector3 middle;
    private Vector3 threeQuarter;
    private Vector3 land;

    private GameObject cam;
    private GameObject startJump;
    private GameObject goDown;
    private GameObject quarterJump;
    private GameObject halfJump;
    private GameObject threeQuarterJump;
    private GameObject landing;
    private GameObject endJump;
    private GameObject img;

    private List<Transform> questionElems;

    private Transform thisQuestion;

    private float timer;

    private bool disappear;

    private string scholle;

	// Use this for initialization
	void Start () {

        // Get Reference to camera for FollowSpline()
        cam = GameObject.Find("CardboardMain");

        // Get references to the EmptyObjects (children) for spline locations
        startJump = GameObject.Find("1startJump");
        goDown = GameObject.Find("2GoDown");
        quarterJump = GameObject.Find("3QuarterJump");
        halfJump = GameObject.Find("4HalfJump");
        threeQuarterJump = GameObject.Find("5ThreeQuarterJump");
        landing = GameObject.Find("6Landing");
        endJump = GameObject.Find("7endJump");

        timer = 0.0f;
        disappear = false;

        thisQuestion = null;
        questionElems = new List<Transform>();
		
	}
	
	// Update is called once per frame
	void Update () {

        // starts timer to delete question and answers
        if (disappear)
        {
            timer += Time.deltaTime;
        }

        if (timer > 2.3f)
        {
            // reset 
            disappear = false;
            timer = 0.0f;

            // destroys every questionElems object from the current question
            foreach (Transform child in thisQuestion)
            {
                // keeps correct scholle
                if(child.name.Equals(scholle))
                {
                    continue;
                }
                if(child.GetComponent<Canvas>() != null)
                {
                    child.GetComponent<Canvas>().enabled = false;
                } else
                {
                    child.GetComponent<Renderer>().enabled = false;
                }
            }
        }
	}

    public void calculateLocalParab(Vector3 start, Vector3 end, Transform currentQuestion, string scholle)
    {
        thisQuestion = currentQuestion;
        this.scholle = scholle;

        Debug.Log("Start: " + start + " Ende: " + end);

        // Richtungsvektor end - start
        Vector3 direction = new Vector3(Mathf.Abs(end.x) - Mathf.Abs(start.x), Mathf.Abs(end.y) - Mathf.Abs(start.y), Mathf.Abs(end.z) - Mathf.Abs(start.z));
        Debug.Log("Richtungsvektor: " + direction);

        // In die Knie gehen und von da aus springen
        down = new Vector3(start.x, start.y - 0.2f, start.z);

        // Viertel der Animation: start-Vektor + (0.25 * Richtungsvektor) -- halbe Höhe des Sprungs: y + 1.0
        quarter = new Vector3(start.x + (0.25f * direction.x), start.y + (0.25f * direction.y) + 1f, start.z + (0.25f * direction.z));
        //Debug.Log("Viertel: " + quarter);

        // Mittelpunkt der Animation: start-Vektor + (0.5 * Richtungsvektor) --- max Höhe des Sprungs: y + 1.3
        middle = new Vector3(start.x + (0.5f * direction.x), start.y + (0.5f * direction.y) + 1.3f, start.z + (0.5f * direction.z));
        //Debug.Log("Mittelpunkt des Paraboloids: " + middle);

        // Dreiviertel der Animation: start-Vektor + (0.75 * Richtungsvektor) -- halbe Höhe des Sprungs: y + 1.0
        threeQuarter = new Vector3(start.x + (0.75f * direction.x), start.y + (0.75f * direction.y) + 1f, start.z + (0.75f * direction.z));
        //Debug.Log("3/4: " + threeQuarter);

        // Nachfedern bei der Landung
        land = new Vector3(end.x, end.y - 0.1f, end.z);

        // add calculated locations to the EmptyObjects
        // start- und end-Höhe des Sprungs auf y + 0.5 (schon in den Parametern)
        startJump.GetComponent<Transform>().position = start;
        goDown.GetComponent<Transform>().position = down;
        quarterJump.GetComponent<Transform>().position = quarter;
        halfJump.GetComponent<Transform>().position = middle;
        threeQuarterJump.GetComponent<Transform>().position = threeQuarter;
        landing.GetComponent<Transform>().position = land;
        endJump.GetComponent<Transform>().position = end;

        // start animation
        cam.GetComponent<SplineController>().FollowSpline();

        // starts timer for begin to make the question and answers disappear
        disappear = true;
    }


}
