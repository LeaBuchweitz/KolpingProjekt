using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FallingDeep : MonoBehaviour {

    private GameObject defaultY;
    private GameObject cam;
    private GameObject fade;
    private GameObject light;
    private GameObject[] schollen;
    private GameObject[] explain;
    private List<GameObject> allQuestions;
    private List<GameObject> schollenHilfe;
    private List<GameObject> explainHelp;
    private List<GameObject> helpForExpl;

    private Vector3[] schollenPosition;
    private Vector3 camPosition;
    private Vector3 endJump;
    private Quaternion camRotation;
    private Quaternion[] schollenRotation;
    private Transform thisQuestion;
    private GameObject[] contrasts;

    private AudioClip applause;

    private float yPosition;
    private float timer;
    private float timerFall;
    private float timerToAnswer;
    private float explainIn;
    private float timerExplanation;
    private int onlyOnce;

    private bool correctAnswer;
    private bool jump;
    private bool fallingStarted;
    private bool explainStarted;
    private bool nowExplain;

    private string canvasObj;
    private string scholleObj;

	// Use this for initialization
	void Start () {

        //GameObject.Find("1-Hell-Dunkel-Scholle-Frage-Antw-Bild"), GameObject.Find("2-Komplemantaer-Scholle-Frage-Antw-Bild"), 

        // reference to all question prefabs
        allQuestions = new List<GameObject> { GameObject.Find("3-Simultan-Scholle-Frage-Antw-Bild") ,
                                              GameObject.Find("4-Unbunt-Bunt-Scholle-Frage-Antw-Bild"), GameObject.Find("5-Farbe-an-sich-Scholle-Frage-Antw-Bild"), GameObject.Find("6-Warm-Kalt-Scholle-Frage-Antw-Bild"),
                                              GameObject.Find("7-Quantitaet-Scholle-Frage-Antw-Bild"), GameObject.Find("8-Qualitaet-Scholle-Frage-Antw-Bild") };

        // reference to one Scholle
        defaultY = GameObject.Find("Start");
        fade = GameObject.Find("Fading");
        //light = GameObject.Find("Directional Light");
        contrasts = GameObject.FindGameObjectsWithTag("Kontrast");

        // gets reference and position to cam
        cam = GameObject.Find("CardboardMain");
        camPosition = cam.GetComponent<Transform>().position;
        camRotation = cam.GetComponent<Transform>().rotation;

        // sets timer and begin of timer
        timer = timerToAnswer = timerFall = explainIn = timerExplanation = 0.0f;
        onlyOnce = 0;
        correctAnswer = true;
        jump = false;
        fallingStarted = false;
        explainStarted = nowExplain = false;
        canvasObj = "Canvas";
        scholleObj = "Scholle";

        fade.GetComponent<AudioSource>().Play();

        schollenHilfe = new List<GameObject>();
        explainHelp = new List<GameObject>();
        helpForExpl = new List<GameObject>();
        thisQuestion = null;

        // makes every question apart of the first invisible
        for(int i = 1; i < allQuestions.Count; i++) 
        {
            foreach(Transform child in allQuestions[i].transform)
            {
                if(child.CompareTag("Canvas"))
                {
                    child.GetComponent<Canvas>().enabled = false;
                    child.GetComponent<EventTrigger>().enabled = false;
                } else
                {
                    child.GetComponent<Renderer>().enabled = false;
                    child.GetComponent<Collider>().enabled = false;
                }
            }
        }

        // makes all contrasts invisible 
        for (int i = 0; i < contrasts.Length; i++)
        {
            contrasts[i].GetComponent<Canvas>().enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*
        // fades in all the sourrounding after intro
        if (!fade.GetComponent<AudioSource>().isPlaying && onlyOnce == 0)
        {
            onlyOnce = 1;

            GameObject[] obj = new GameObject[] { GameObject.Find("DunkleKugel") };
            int[] fadingMode = new int[] { 0 };
            Color color = obj[0].GetComponent<Renderer>().material.color;
            fade.GetComponent<Fading>().NowFade(obj, color, fadingMode, 1);
        }
        */

        // checks y position if Schollen have already fallen deep enough
        yPosition = cam.GetComponent<Transform>().position.y;

        
        // starts timer for falling __________________________________________ WRONG _________________________________________________________________________________________________________________
        if (!correctAnswer)
        {
            timer += Time.deltaTime;
        }

        if (fallingStarted)
        {
            timerFall += Time.deltaTime;
        }

        if(explainStarted)
        {
            timerExplanation += Time.deltaTime;
        }
        
        // starts jump although answer is wrong __________________________________ STARTS FALLING WITH A JUMP _______________________________________________________
        if (timer > 1.8)
        {
            // calls function in CalculateJumpParab to animate jump to correct Scholle
            cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(endJump.x, endJump.y + 0.7f, endJump.z),thisQuestion, scholleObj);

            foreach(Transform obj in thisQuestion)
            {
                // disables the eventTrigger
                if (obj.GetComponent<Canvas>() != null)
                {
                    obj.GetComponent<EventTrigger>().enabled = false;
                }
            }

            fallingStarted = true;

            reset();
        }

        // start falling of all Schollen  _____________________________ FALLING __________________________________________________
        if (timerFall > 2.0)
        {
            // sets color of the answer to red
            foreach (Transform obj in thisQuestion)
            {
                if (obj.name.Equals(canvasObj))
                {
                    obj.GetComponentInChildren<Text>().color = Color.red;
                }
            }

            // starts falling of every scholle
            foreach (GameObject objct in schollen)
            {
                objct.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        // starts cam falling
        if (timerFall > 2.3)
        {
            cam.GetComponent<Rigidbody>().isKinematic = false;
            fallingStarted = false;
            timerFall = 0.0f;
        }


        if (yPosition < -200) //_____________________________________________________ RESET ____________________________________________
        {
            // resets positions of Schollen and isKinematic to stop them falling
            for (int i = 0; i < schollen.Length; i++)
            {
                schollen[i].GetComponent<Rigidbody>().isKinematic = true;
                schollen[i].GetComponent<Transform>().position = schollenPosition[i];
                schollen[i].GetComponent<Transform>().rotation = schollenRotation[i];
            }

            // resets cam position and isKinematic to stop it from falling
            Vector3 overStart = new Vector3(0, 0.5f, 0);
            cam.GetComponent<Rigidbody>().isKinematic = true;
            cam.GetComponent<Transform>().position = defaultY.GetComponent<Transform>().position + overStart;
            cam.GetComponent<Transform>().rotation = camRotation;

            // sets color of the answer back to normal
            foreach (Transform obj in thisQuestion)
            {
                if (obj.name.Equals(canvasObj))
                {
                    obj.GetComponentInChildren<Text>().color = new Color(206, 206, 206);
                }
            }

            canvasObj = "Canvas";

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
            cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(endJump.x, endJump.y + 0.7f, endJump.z), thisQuestion, "1"+scholleObj);

            // sets color of the answer back to normal
            foreach (Transform obj in thisQuestion)
            {
                // disables the eventTrigger
                if(obj.GetComponent<Canvas>() != null)
                {
                    obj.GetComponent<EventTrigger>().enabled = false;
                }
                
                if (obj.name.Equals(canvasObj))
                {
                    obj.GetComponentInChildren<Text>().color = new Color(206, 206, 206);
                } 
            }

            explainStarted = true;

            reset();
            canvasObj = "Canvas";
        }

        if(timerExplanation > 1.5f)
        {

            explainStarted = false;
            timerExplanation = 0.0f;

            explanation();

            // gets rid of the current question in the list and scales up the next
            if (allQuestions != null)
            {
                allQuestions.RemoveAt(0);
                Debug.Log("allQuestions: " + allQuestions.Count + " nämlich: " + allQuestions[0].name);

                // next question is right beside, do not make the question directly appear
                if (allQuestions[0] != null && (allQuestions[0].GetComponentInChildren<Renderer>().enabled == false || allQuestions[0].GetComponentInChildren<Canvas>().enabled == false) && !allQuestions[0].name.StartsWith("5"))
                {
                    foreach (Transform child in allQuestions[0].transform)
                    {
                        if (child.CompareTag("Canvas"))
                        {
                            child.GetComponent<Canvas>().enabled = true;
                            child.GetComponent<EventTrigger>().enabled = true;
                        }
                        else
                        {
                            if (!child.CompareTag("Explanation"))
                            {
                                child.GetComponent<Renderer>().enabled = true;
                                child.GetComponent<Collider>().enabled = true;
                            }
                        }
                    }
                }
            }
        }

        //_________________________________________________________________________________________________________________________________________________________________________________________________
        // _________________________________________________ EXPLANATIONS _________________________________________________________________________________________________________________________________

        if(nowExplain)
        {
            explainIn += Time.deltaTime;
        }

        if(explainIn > 2.0f)
        {
            nowExplain = false;
            explainIn = 0.0f;
            explain2();
        }
        //___________________________________________________________________________________________________________________________________________________________________________________________________
    }

    // RESET GAME PARTS _________________________________________________________________
    public void reset()
    {
        // sets all values back to start
        scholleObj = "Scholle";
        timerToAnswer = timer = 0.0f;
        jump = false;
        correctAnswer = true;
    }

    // START GAZE ON ANSWER ______________________________________________________________
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
            endOfGame();
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
            schollenRotation = new Quaternion[schollen.Length];
            for (int i = 0; i < schollen.Length; i++)
            {
                schollenPosition[i] = schollen[i].GetComponent<Transform>().position;
                schollenRotation[i] = schollen[i].GetComponent<Transform>().rotation;
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
        foreach(Transform obj in thisQuestion)
        {
            if(obj.name.Equals(canvasObj))
            {
                obj.GetComponentInChildren<Text>().color = Color.green;
            }
        }
    }

    // GAZE NO LONGER ON ANSWER __________________________________________________________________
    public void gravityOff()
    {
        // sets timer only back if Schollen are not falling
        if(schollen[0].GetComponent<Rigidbody>().isKinematic)
        {
            correctAnswer = true;
            jump = false;
            timer = timerToAnswer = 0.0f;
            // resets color to black in a questionElems of the current question
            foreach (Transform obj in thisQuestion)
            {
                if (obj.name.Equals(canvasObj))
                {
                    obj.GetComponentInChildren<Text>().color = new Color(206, 206, 206);
                }
            }

            canvasObj = "Canvas";
            scholleObj = "Scholle";
        }
    }

    // RESET GAME COMPLETELY ____________________________________________________________________
    private void resetGame()
    {
        // reference to all question prefabs
        allQuestions = new List<GameObject> { GameObject.Find("1-Hell-Dunkel-Scholle-Frage-Antw-Bild"), GameObject.Find("2-Komplemantaer-Scholle-Frage-Antw-Bild"), GameObject.Find("3-Simultan-Scholle-Frage-Antw-Bild") ,
                                              GameObject.Find("4-Unbunt-Bunt-Scholle-Frage-Antw-Bild"), GameObject.Find("5-Farbe-an-sich-Scholle-Frage-Antw-Bild"), GameObject.Find("6-Warm-Kalt-Scholle-Frage-Antw-Bild"),
                                              GameObject.Find("7-Quantitaet-Scholle-Frage-Antw-Bild"), GameObject.Find("8-Qualitaet-Scholle-Frage-Antw-Bild") };

        // sets timer and begin of timer
        timer = timerToAnswer = timerFall = 0.0f;
        correctAnswer = true;
        jump = false;
        fallingStarted = false;
        canvasObj = "Canvas";
        scholleObj = "Scholle";

        schollenHilfe = new List<GameObject>();
        thisQuestion = null;

        // reset question 1 to visible and eventtrigger enabled
        foreach (Transform child in allQuestions[0].transform)
        {
            if(child.GetComponent<Canvas>() != null)
            {
                child.GetComponent<EventTrigger>().enabled = true;
                child.GetComponent<Canvas>().enabled = true;
            } else
            {
                child.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    // EXPLAIN THE CORRECT ANSWER ____________________________________________________________________
    private void explanation()
    {
        Debug.Log("DIE ERKLÄRFRAGE IST: " + thisQuestion.name);

        Color color = Color.cyan;
        int[] fadingMode = null;
        foreach(Transform child in thisQuestion.transform)
        {
            if(child.CompareTag("Explanation"))
            {
                explainHelp.Add(child.gameObject);
                Debug.Log(child.name);
                child.GetComponent<Renderer>().enabled = true;
                color = child.GetComponent<Renderer>().material.color;
                child.GetComponent<Renderer>().material.color = Color.clear;
            }
        }

        // Explanation for the 3 Question
        if (thisQuestion.name.StartsWith("3"))
        {
            Debug.Log("Bin bei der 3. Frage");
            GameObject chess = null;
            foreach (Transform obj in thisQuestion)
            {
                if (obj.name.Equals("BildSchattenfelderSimultan"))
                {
                    chess = obj.gameObject;
                }
            }
            chess.gameObject.tag = "Explanation";
            chess.GetComponent<Renderer>().enabled = true;
            chess.GetComponent<Autowalk>().correctObjFocused = true;
            chess.GetComponent<Autowalk>().endPoint = new Vector3(-3.3f, 0.8f, 3.3f);
            chess.GetComponent<Autowalk>().positionObject = chess.transform.position;

            StartCoroutine(waitToFadeOut(new GameObject[] { chess, explainHelp[0] }, chess.GetComponent<Renderer>().material.color, new int[] { 2, 0 }));
        }

        // Explanation for 4 Question
        if(thisQuestion.name.StartsWith("4"))
        {
            GameObject answer = null;
            foreach (Transform obj in thisQuestion)
            {
                if (obj.name.StartsWith("A ") || obj.name.StartsWith("B ") || obj.name.StartsWith("C "))
                {
                    answer = obj.gameObject;
                    answer.gameObject.tag = "Explanation";
                    answer.GetComponent<Autowalk>().correctObjFocused = true;
                    answer.GetComponent<Autowalk>().endPoint = new Vector3(answer.transform.position.x + 1.7f, answer.transform.position.y, answer.transform.position.z);
                    answer.GetComponent<Autowalk>().positionObject = answer.transform.position;
                    helpForExpl.Add(obj.gameObject);
                }
            }
            GameObject sol1 = GameObject.Find("4 zu Scholle B Warmweiß-neutralWeiß");
            sol1.GetComponent<Transform>().position += new Vector3(1.7f,0,0);
            helpForExpl.Add(sol1);
            GameObject sol2 = GameObject.Find("4 zu Scholle C Kaltes Schwarz-neutrales Schwarz");
            sol2.GetComponent<Transform>().position += new Vector3(1.7f,0, 0);
            helpForExpl.Add(sol2);

            StartCoroutine(explain4());
        }

        // Fades in all the explanation elements
        if(explainHelp.Count != 0)
        {
            explain = explainHelp.ToArray();
            fadingMode = new int[] { 2, 1 };
            fade.GetComponent<Fading>().NowFade(explain, color, fadingMode, 1);
            Debug.Log("ICH BIN NICHT 0");
        }

        // Explanation for the 2 Question
        if (thisQuestion.name.StartsWith("2"))
        {
            nowExplain = true;
            Debug.Log("ICH Fang mit 2 an");
        }

        Debug.Log("ICH BIN JETZT AM ENDE");
    }

    // END OF THE GAME __________________________________________________________________________
    private void endOfGame()
    {
        // jump back into the middle
        cam.GetComponent<CalculateJumpParab>().calculateLocalParab(camPosition, new Vector3(defaultY.GetComponent<Transform>().position.x, defaultY.GetComponent<Transform>().position.y + 0.7f,
            defaultY.GetComponent<Transform>().position.z), thisQuestion, scholleObj);

        fade.GetComponent<AudioSource>().Play();
    }

    // EXPLANATION FOR THE 2 QUESTION ____________________________________________________________
    private void explain2()
    {
        explainHelp.RemoveAt(1);
        explain = explainHelp.ToArray();
        Color color = explain[0].GetComponent<Renderer>().material.color;
        int[] fadingMode = new int[] { 0 };
        fade.GetComponent<Fading>().NowFade(explain, color, fadingMode, 1);

        StartCoroutine(waitToFadeOut(new GameObject[] { GameObject.Find("2 Streifen Komplementaer Simultan") },color,fadingMode));
    }

    // EXPLANATION FOR THE 4 QUESTION _________________________________________________________________
    private IEnumerator explain4()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(waitToFadeOut(helpForExpl.ToArray(), Color.black , null));


        // now fade in next question
        foreach (Transform child in allQuestions[0].transform)
        {
            if (child.CompareTag("Canvas"))
            {
                child.GetComponent<Canvas>().enabled = true;
                child.GetComponent<EventTrigger>().enabled = true;
            }
            else
            {
                if (!child.CompareTag("Explanation"))
                {
                    child.GetComponent<Renderer>().enabled = true;
                    child.GetComponent<Collider>().enabled = true;
                }
            }
        }
        StopCoroutine("explain4");

    }

    // FADING OUT SOME ELEMENTS OF EXPLANATION ______________________________________________________
    IEnumerator waitToFadeOut(GameObject[] toFade, Color color, int[] fadingMode)
    {
        Debug.Log("Ich wurde aufgerufen");
        yield return new WaitForSeconds(2.5f);
        Debug.Log("2 Sek gewartet");
        fade.GetComponent<Fading>().NowFade(toFade, color, fadingMode, 1);
        explainHelp.Clear();
        StopCoroutine("waitToFadeOut");
    }
}
