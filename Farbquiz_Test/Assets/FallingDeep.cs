using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FallingDeep : MonoBehaviour {

    //hi

    GameObject defaultY;
    GameObject cam;
    GameObject[] schollen;
    GameObject[] canvas;
    Vector3[] schollenPosition;
    Vector3 camPosition;
    Vector3 endJump;
    float yPosition;
    float timer;
    float timerToAnswer;
    bool correctAnswer;
    bool jump;
    string canvasObj;
    string scholleObj;

	// Use this for initialization
	void Start () {

        // reference to one Scholle
        defaultY = GameObject.Find("KameraScholle");

        // gets reference and position to cam
        cam = GameObject.Find("CardboardMain");
        camPosition = cam.GetComponent<Transform>().position;

        // sets timer and begin of timer
        timer = timerToAnswer = 0.0f;
        correctAnswer = true;
        jump = false;
        canvasObj = "Canvas";
        scholleObj = "Scholle";

    }

    // Update is called once per frame
    void Update()
    {

        // checks y position if Schollen have already fallen deep enough
        yPosition = defaultY.GetComponent<Transform>().position.y;

        // starts timer for falling __________________________________________ WRONG _________________________________________________________________________________________________________________
        if (!correctAnswer)
        {
            timer += Time.deltaTime;
        }

        // start falling of all Schollen  _____________________________ FALLING __________________________________________________
        if (timer > 1.8)
        {
            foreach (GameObject objct in schollen)
            {
                objct.GetComponent<Rigidbody>().isKinematic = false;
                GameObject.Find(canvasObj).GetComponentInChildren<Text>().color = Color.red;
            }
        }

        // starts cam falling
        if (timer > 1.95)
        {
            cam.GetComponent<Rigidbody>().isKinematic = false;
        }

        if (yPosition < -40) //_____________________________________________________ RESET ____________________________________________
        {
            // resets positions of Schollen and isKinematic to stop them falling
            for (int i = 0; i < schollen.Length; i++)
            {
                schollen[i].GetComponent<Rigidbody>().isKinematic = true;
                schollen[i].GetComponent<Transform>().position = schollenPosition[i];
            }

            // resets cam position and isKinematic to stop it from falling
            cam.GetComponent<Rigidbody>().isKinematic = true;
            cam.GetComponent<Transform>().position = camPosition;

            // disables the EventTrigger so that the answer can't be chosen anymore
            canvas = GameObject.FindGameObjectsWithTag("Canvas");
            foreach (GameObject obj in canvas)
            {
                obj.GetComponent<EventTrigger>().enabled = false;
            }

            // reset
            correctAnswer = true;
            timer = 0.0f;
            canvasObj = "Canvas";
        }
        //________________________________________________________________________________________________________________________________________________________________________________________________

        if (jump) //____________________________________ RIGHT ___________________________________________________________________________________________________________________________________________
        {
            timerToAnswer += Time.deltaTime;
        }

        if (timerToAnswer > 1.8f) //____________________________________ JUMPING _______________________________________
        {
            // calls function in CalculateJumpParab to animate jump to correct Scholle
            cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(endJump.x, endJump.y + 0.5f, endJump.z));

            // reset 
            canvasObj = "Canvas";
            scholleObj = "Scholle";
            timerToAnswer = 0.0f;
            jump = false;

            // disables the EventTrigger so that the answer can't be chosen anymore
            canvas = GameObject.FindGameObjectsWithTag("Canvas");
            foreach (GameObject obj in canvas)
            {
                obj.GetComponent<EventTrigger>().enabled = false;
            }
        }
        //_________________________________________________________________________________________________________________________________________________________________________________________________
    }

    public void gravityOn(string correct_Name)
    {
        // gets all possible Schollen everytime gravityOn is called
        schollen = GameObject.FindGameObjectsWithTag("Scholle");

        // gets position of all Schollen only if they're not falling
        if(schollen[0].GetComponent<Rigidbody>().isKinematic)
        {
            // gets all positions to all Schollen
            schollenPosition = new Vector3[schollen.Length];
            for (int i = 0; i < schollen.Length; i++)
            {
                schollenPosition[i] = schollen[i].GetComponent<Transform>().position;
            }
        }

        // sets correctness of answer
        if(correct_Name.StartsWith("1"))
        {
            // sets string of current object of Canvas and Scholle
            canvasObj += correct_Name.ToCharArray().GetValue(1).ToString();
            scholleObj += correct_Name.ToCharArray().GetValue(1).ToString();
            // searches current object in Schollen and gets index
            for(int i = 0; i < schollen.Length; i++)
            {
                if(schollen[i].name.Equals(scholleObj))
                {
                    // gets position of the object (at Index i) for end of Jump
                    endJump = schollenPosition[i];
                }
            }

            // starts timerToAnswer to start jump
            jump = true;           

            //Debug.Log("Wir können gleich springen");
        } else
        {
            // sets string of current object and starts begin for timer
            canvasObj += correct_Name;
            correctAnswer = false;
        }

        // sets color of the answer to green
        GameObject.Find(canvasObj).GetComponentInChildren<Text>().color = Color.green;
    }

    public void gravityOff()
    {
        // sets timer only back if Schollen are not falling
        if(schollen[0].GetComponent<Rigidbody>().isKinematic)
        {
            correctAnswer = true;
            jump = false;
            timer = timerToAnswer = 0.0f;
            GameObject.Find(canvasObj).GetComponentInChildren<Text>().color = new Color(206, 206, 206) ;
            canvasObj = "Canvas";
            scholleObj = "Scholle";
        }
    }
}
