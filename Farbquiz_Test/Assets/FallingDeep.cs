using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FallingDeep : MonoBehaviour {

    private GameObject defaultY;
    private GameObject cam;
    private GameObject[] schollen;
    private GameObject[] canvas;
    private List<GameObject> allQuestions;
    private List<GameObject> schollenHilfe;

    private Vector3[] schollenPosition;
    private Vector3 camPosition;
    private Vector3 endJump;
    private Quaternion camRotation;
    private Transform thisQuestion;

    private float yPosition;
    private float timer;
    private float timerFall;
    private float timerToAnswer;

    private bool correctAnswer;
    private bool jump;
    private bool fallingStarted;

    private string canvasObj;
    private string scholleObj;

	// Use this for initialization
	void Start () {

        // reference to all question prefabs
        allQuestions = new List<GameObject> { GameObject.Find("1-Hell-Dunkel-Scholle-Frage-Antw-Bild"), GameObject.Find("2-Komplemantaer-Scholle-Frage-Antw-Bild"), GameObject.Find("3-Simultan-Scholle-Frage-Antw-Bild") };//,
            // GameObject.Find("4-Unbunt-Bunt-Scholle-Frage-Antw-Bild"), GameObject.Find("5-Farbe-an-sich-Scholle-Frage-Antw-Bild"), GameObject.Find("6-Warm-Kalt-Scholle-Frage-Antw-Bild"),
            // GameObject.Find("7-Quantitaet-Scholle-Frage-Antw-Bild"), GameObject.Find("8-Qualitaet-Scholle-Frage-Antw-Bild") };

        // reference to one Scholle
        defaultY = GameObject.Find("Start");

        // gets reference and position to cam
        cam = GameObject.Find("CardboardMain");
        camPosition = cam.GetComponent<Transform>().position;
        camRotation = cam.GetComponent<Transform>().rotation;

        // sets timer and begin of timer
        timer = timerToAnswer = timerFall = 0.0f;
        correctAnswer = true;
        jump = false;
        fallingStarted = false;
        canvasObj = "Canvas";
        scholleObj = "Scholle";

        schollenHilfe = new List<GameObject>();
        thisQuestion = null;

        // makes every question apart of the first invisible
        for(int i = 1; i < allQuestions.Count; i++) 
        {
            allQuestions[i].GetComponent<Transform>().localScale = new Vector3 (0,0,0);
            allQuestions[i].GetComponentInChildren<EventTrigger>().enabled = false;
        }

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

        if (fallingStarted)
        {
            timerFall += Time.deltaTime;
        }
        
        // starts jump although answer is wrong __________________________________ STARTS FALLING WITH A JUMP _______________________________________________________
        if (timer > 1.8)
        {
            // calls function in CalculateJumpParab to animate jump to correct Scholle
            cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(endJump.x, endJump.y + 0.5f, endJump.z),thisQuestion);

            fallingStarted = true;

            reset();
        }

        // starts cam falling
        if (timerFall > 2.3)
        {
            cam.GetComponent<Rigidbody>().isKinematic = false;
            fallingStarted = false;
            timerFall = 0.0f;
        }

        // start falling of all Schollen  _____________________________ FALLING __________________________________________________
        if (timerFall > 2.0)
        {
            foreach (GameObject objct in schollen)
            {
                objct.GetComponent<Rigidbody>().isKinematic = false;
                Debug.Log("Fällt: " + objct);
                GameObject.Find(canvasObj).GetComponentInChildren<Text>().color = Color.red;
            }
        }

        if (yPosition < -200) //_____________________________________________________ RESET ____________________________________________
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
            cam.GetComponent<Transform>().rotation = camRotation;

            reset();
            canvasObj = "Canvas";

            // TODO!
            resetGame();
        }
        //________________________________________________________________________________________________________________________________________________________________________________________________

        if (jump) //____________________________________ RIGHT ___________________________________________________________________________________________________________________________________________
        {
            timerToAnswer += Time.deltaTime;
        }

        if (timerToAnswer > 1.8f) //____________________________________ JUMPING _______________________________________
        {
            // calls function in CalculateJumpParab to animate jump to correct Scholle
            cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(endJump.x, endJump.y + 0.5f, endJump.z), thisQuestion);

            reset();
            canvasObj = "Canvas";

            // gets rid of the current question in the list and scales up the next
            if(allQuestions != null)
            {
                allQuestions.RemoveAt(0);
                Debug.Log("allQuestions: " + allQuestions.Count + " nämlich: " + allQuestions[0].name);
                if(allQuestions[0] != null && allQuestions[0].transform.localScale == new Vector3(0, 0, 0))
                {
                    allQuestions[0].transform.localScale = new Vector3(1, 1, 1);
                    allQuestions[0].GetComponentInChildren<EventTrigger>().enabled = true;
                }
            }
        }

        //_________________________________________________________________________________________________________________________________________________________________________________________________
    }

    public void reset()
    {
        // sets all values back to start
        scholleObj = "Scholle";
        timerToAnswer = timer = 0.0f;
        jump = false;
        correctAnswer = true;
    }

    // Gaze is on this Object
    public void gravityOn(string correct_Name)
    {
        // gets current cam position because of starting point of jump
        camPosition = cam.GetComponent<Transform>().position;
        camRotation = cam.GetComponent<Transform>().rotation;

        // checks if there is another question in the list to answer if there isn't you won!
        if (allQuestions != null)
        {
            thisQuestion = allQuestions[0].transform;
            Debug.Log("Nächste Frage mit " + thisQuestion.name + " Object");
        } else
        {
            Debug.Log("Game is over and won!");
        }

        // gets all possible Schollen everytime gravityOn is called
        foreach(Transform child in thisQuestion)
        {
            if(child.CompareTag("Scholle"))
            {
                schollenHilfe.Add(child.gameObject);
            }
        }
        // Startscholle is added because of falling together
        schollenHilfe.Add(defaultY);

        schollen = schollenHilfe.ToArray();

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
            
            // starts timerToAnswer to start jump
            jump = true;           

            //Debug.Log("Wir können gleich springen");
        } else
        {
            // sets string of current object and starts begin for timer
            canvasObj += correct_Name;
            scholleObj += correct_Name;
            correctAnswer = false;
        }

        // searches current object in Schollen and gets index
        for (int i = 0; i < schollen.Length; i++)
        {
            if (schollen[i].name.Equals(scholleObj))
            {
                // gets position of the object (at Index i) for end of Jump
                endJump = schollenPosition[i];
            }
        }

        // sets color of the answer to green
        GameObject.Find(canvasObj).GetComponentInChildren<Text>().color = Color.green;
    }

    // Gaze no longer on this object
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

    public void resetGame()
    {
        // reference to all question prefabs
        allQuestions = new List<GameObject> { GameObject.Find("1-Hell-Dunkel-Scholle-Frage-Antw-Bild"), GameObject.Find("2-Komplemantaer-Scholle-Frage-Antw-Bild"), GameObject.Find("3-Simultan-Scholle-Frage-Antw-Bild") }; //,
                                            // GameObject.Find("4-Unbunt-Bunt-Scholle-Frage-Antw-Bild"), GameObject.Find("5-Farbe-an-sich-Scholle-Frage-Antw-Bild"), GameObject.Find("6-Warm-Kalt-Scholle-Frage-Antw-Bild"),
                                            // GameObject.Find("7-Quantitaet-Scholle-Frage-Antw-Bild"), GameObject.Find("8-Qualitaet-Scholle-Frage-Antw-Bild") };
    }
}
