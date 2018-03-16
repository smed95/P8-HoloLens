using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindowAR : MonoBehaviour {

    //This struct type is the type recieved error/warning messages are converted to
    //We can also make messages of this type.

    //The console only displays Log messages through HandleLog
    //So any msgs we create and exception messages through Application.logMessageReceived.

    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    private Text text;
    List<Log> logs = new List<Log>();
    GameObject debugWindow;
    GameObject debugCanvas;
    GameObject debugError;
    GameObject debugConsole;
    GameObject panel;
   

    bool showing;

   


    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        {LogType.Assert, Color.white },
        {LogType.Error, Color.red },
        {LogType.Exception, Color.red },
        {LogType.Log, Color.white },
        {LogType.Warning, Color.yellow }
    };

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        Debug.Log("DebugConsole enabled");

        debugWindow = GameObject.Find("DebugText");
        debugCanvas = GameObject.Find("DebugCanvas");
        debugConsole = GameObject.Find("DebugConsole");
       
        text = debugWindow.GetComponent<Text>();
        
        showing = true;

        Debug.Log("Debugging Enabled");
        Debug.Log("Log count is " + logs.Count);
        ClearDebugConsole();
        //Note Vuforia Init is called after this, on the first init, which fills the console.
        //Then Display is Opaque and StartVuforia.
    }



    private void OnDisable()
    {
        Debug.Log("DebugConsole disabled");
        //LogMessages are no longer with the Handlelog delegate when inactive. 
        Application.logMessageReceived -= HandleLog;
        showing = false;
    }


    private void Update()
    {
        //Finally works, maybe do this depending on text size, not log count.
        //Bugs when using values fetched from a field
        if(logs.Count > 12)
        {
            ClearDebugConsole();
        }
    }

    public void ClearDebugConsole()
    {
        logs.Clear();
        text.text = "";
    }

    public void ToggleDebugConsole()
    {
        //SetActive calls will call OnEnable/OnDisable.
        if(showing == false)
        {
            debugConsole.SetActive(true);
        }
        else if(showing == true)
        {
            debugConsole.SetActive(false);
        }      
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if(showing == false) return; 
        text.text = "";
     
        //Adds the HandleLog params to a new log object
        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type
        });

        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];
            //Previous approach with text color to most recent error msg type color.
            //text.color = logTypeColors[log.type];
            
            
            //Color is now added for each individual error/warning msg 
            var color = ColorUtility.ToHtmlStringRGB(logTypeColors[log.type]);
            text.text += "<color=#" + color + "> " + "\n" + log.message + "</color>";
            text.text += "<color=#" + color + "> " + "\n " + log.stackTrace + "</color>";
        }
    }

    public void ShowLogNoTrace()
    {
        //Only difference is removed the line containing stackTrace.
        text.text = "";
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            var color = ColorUtility.ToHtmlStringRGB(logTypeColors[log.type]);
            text.text += "<color=#" + color + "> " + "\n" + log.message + "</color>";
        }
    }

    public void IncrementFontSize()
    {
        
        int fontSize = text.fontSize;
        fontSize++;
        text.fontSize = fontSize;
    }

    public void DecrementFontSize()
    {
        int fontSize = text.fontSize;
        fontSize--;
        text.fontSize = fontSize;
    }
    
    public void GenerateRandomMessage()
    {
        
        string str;
        int i = Random.Range(1, 20);
        str = Random.Range(0.0001f, 9999.0f).ToString();
        if(i >= 1 && i <= 5)
        {
            HandleLog(str, "CalledFrom GenerateRandomMessage(); ", LogType.Exception);
        }
        if( i >= 6 && i <= 8)
        {
            HandleLog("Debug msg", "CalledFrom GenerateRandomMessage(); ", LogType.Log);
        }
        if(i >= 9 && i <= 11)
        {
            HandleLog("Error msg", "CalledFrom GenerateRandomMessage(); ", LogType.Error);
        }
        if(i >= 12 && i <= 14)
        {
            HandleLog("Warning Msg", "CalledFrom GenerateRandomMessage(); ", LogType.Warning);
        }
        if(i >= 15 && i <= 20)
        {
            HandleLog("Assertion Msg", "CalledFrom GenerateRandomMessage(); ", LogType.Assert);
        }
    }
}
