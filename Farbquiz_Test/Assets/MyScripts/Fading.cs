using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fading : MonoBehaviour {

    private List<GameObject> obj;

    private Color startColor;
    private Color transparency;

    private float duration;
    private float interval;
    private float SetIntervalBack;
    private float time;

    private List<int> fadingState;
    public int orderCounter;

	// initializes default-problem-colors, counter (time) for 100% alpha (duration = 1.0f), sets fadingOut to false and helper to show the object at once
	void Start () {

        startColor = Color.red;
        transparency = Color.clear;

        duration = 1.0f;
        time = 0.0f;

        obj = new List<GameObject>();

        fadingState = new List<int>();
        fadingState.Add(3);

        // sets the order of all the animations (start: -1)
        orderCounter = 0;

    }

    // calls FadeOut() if fadingOut == true 
    void Update () {

            switch(fadingState[0])
            {
                case 0: // fadingState = 0 calls FadeOut()
                    {
                        // fades out if object not invisible
                        if (time < duration)
                        {
                            FadeOut();
                            time += Time.deltaTime;
                        }

                        // counts order up and stops fading
                        if (time >= duration)
                        {
                            // if there should be a waiting-interval between some objects interval is the waiting time in seconds
                            if (interval > 0)
                            {
                                interval -= Time.deltaTime;
                            }
                            else
                            {
                                proceedToNextElem();
                            }
                        }
                        break;
                    }
                // fadingState == 1 calls FadeIn()
                case 1:
                    {
                        // fades in if object not invisible
                        if (time < duration)
                        {
                            time += Time.deltaTime;
                            FadeIn();
                        }
                        // counts order up and stops fading
                        if (time >= duration)
                        {
                            // if there should be a waiting-interval between some objects interval is the waiting time in seconds
                            if (interval > 0)
                            {
                                interval -= Time.deltaTime;
                            }
                            else
                            {
                                proceedToNextElem();
                            }
                        }
                        break;
                    }
                case 2: { // Fading several objects at the same time
                        if (time < duration)
                        {
                            time += Time.deltaTime;
                            FadeSeveral();
                        }
                        // counts order up and stops fading
                        if (time >= duration)
                        {
                            orderCounter++;

                            fadingState.Clear();
                            fadingState.Add(3);

                            /*
                            foreach (GameObject elem in obj)
                            {
                                //Destroy(elem);
                            }
                            */
                        }
                        break; }
                case 3: { break; }
                // fadingState = 3 is default value, nothing happens
                default: { Debug.Log("PROBLEM: fadingState ist: " + fadingState); break; }
            }

        }

    // sets the current object which has to faded, the color of it and if it should be faded in or out
    // obj: array of all objects which have to be faded
    // originalColor: the color of (at least the first) object which has to be faded
    // fading: array of fading-instructions, if several objects should be faded the first item is 2 (fading can have less items than obj, if several objects are faded the same way)
    // interval: float of waiting-interval between some objects of obj
    public void NowFade(GameObject[] obj, Color originalColor, int[] fading, int interval)
    {
        startColor = originalColor;
        this.interval = interval;
        SetIntervalBack = interval;

        time = 0.0f;

        // clears the arrays to be clean for the next fading actions
        this.obj.Clear();
        fadingState.Clear();

        for (int i = 0; i < obj.Length; i++)
        {
            this.obj.Insert(i, obj[i]);
        }

        // add objects which has to be faded
        switch (fading[0])
        {
            case 2:
                {// case 2 = Fading several objects at once in or out, so first element of fading is 2, second 1 or 0
                    fadingState.Insert(0, fading[0]);                   
                    Debug.Log("obj: " + obj.Length + " fading: " + fading.Length);
                    for (int i = 1; i <= obj.Length; i++)
                    {
                        // if all objects have the same fading instruction, just get the second fading instruction in the list for every object
                        if(obj.Length != fading.Length-1) { 
                            fadingState.Insert(i, fading[1]);
                            Debug.Log("I fade every Object with: " + fading[1]);
                        } else // if objects and fading instructions have the same size, get all fading instructions
                        {
                            fadingState.Insert(i, fading[i]);
                            Debug.Log("I fade " + obj[i-1] + " with: " + fading[i]);
                        }
                    }
                    break;
                }
            default:
                { Debug.Log("default, no several fading");
                    for (int i = 0; i < obj.Length; i++)
                    {
                        // add info if objects has to be faded in or out
                        fadingState.Insert(i, fading[i]);
                    }
                    break;
                }
        }
        // HELP
        Debug.Log("Jetz wird gefadet: " + fading[0] + " undzwar mit: " + obj[0].name);

    }

    // fades several objects in or out at the same time
    public void FadeSeveral()
    {
        Debug.Log("Several Fade mit : " + this.obj.Count + " Elementen und " + fadingState.Count + " fading-Anweisungen");

        Color FadeOut = Color.Lerp(startColor, transparency, time / duration);
        Color FadeIn = Color.Lerp(transparency, startColor, time / duration);

        // Fade several Objects at the same time
        for (int i = 1; i <= obj.Count; i++)
        {
            if (fadingState[i] == 0)
            {
                obj[i-1].GetComponent<Renderer>().material.color = FadeOut;
            }
            else if (fadingState[i] == 1)
            {
                obj[i-1].GetComponent<Renderer>().material.color = FadeIn;
            }  else
            {
                Debug.Log("SOMETHING IS WRONG WITH SECOND ELEMENT OF FADING-ARRAY? YOU WANT TO FADE IN OR OUT?! " + fadingState[i]);
            }
        }
    }

    // fades object out
    public void FadeOut()
    {
        Debug.Log("Hier ist FadeOut");

        // gets always the first object, because this is the one which is faded now
        obj[0].GetComponent<Renderer>().material.color = Color.Lerp(startColor, transparency, time / duration);

    }

    // fades object in
    public void FadeIn()
    {
        Debug.Log("Hier ist FadeIn");

        // gets always the first object, because this is the one which is faded now
        obj[0].GetComponent<Renderer>().material.color = Color.Lerp(transparency, startColor, time / duration);
    }

    public void proceedToNextElem()
    {
        orderCounter++;

        // removes first fade-int and object in the array to get info if there's another object to fade
        fadingState.RemoveAt(0);
        obj.RemoveAt(0);

        // sets fadingState to default status or starts the next fading process
        if (fadingState.Count == 0)
        {
            fadingState.Add(3);
        }
        else // sets time to fading option back, sets the interval time back and takes back the counter increment which will be made next
        {
            Debug.Log("(PTNE-Fkt) Als nächstes zum faden bereit: " + obj[0] + " Fademodus: " + fadingState[0]);
            time = 0.0f;
            interval = SetIntervalBack;
            orderCounter--;
        }
    }

}
