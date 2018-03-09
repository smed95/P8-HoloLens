
using System;
using UnityEngine;
using UnityEngine.UI;



public class DebugLog : MonoBehaviour
{


    private Text debugText;

    // Use this for initialization
    void Start()
    {
        debugText = GetComponent<Text>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleDebugLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleDebugLog;
    }



    void HandleDebugLog(string logString, string stackTrace, LogType type)
    {
        // In the first frames, the text has not been initialized
        // and therefore throws a NullReferenceException, which we ignore.

        try
        {
            debugText.text = logString;


        }
      
        catch(NullReferenceException e)
        {

        }

    }


}
