using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindowAR : MonoBehaviour {

    //This struct type is the type recieved error/warning messages are converted to
    //We can also make messages of this type.
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

    bool showing = false;

    //clearCount is supposed to set limit of amount of log items shown
    public int clearCount = 5;


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

        debugWindow = GameObject.Find("DebugText");
        debugCanvas = GameObject.Find("DebugCanvas");
        debugConsole = GameObject.Find("DebugConsole");
        text = debugWindow.GetComponent<Text>();
        showing = true;

        Debug.Log("Debugging Enabled");
    }

    private void OnDisable()
    {
        //LogMessages are no longer with the Handlelog delegate when inactive. 
        Application.logMessageReceived -= HandleLog;
    }


    private void Update()
    {
        //Finally works, maybe do this depending on text size, not log count.
        if(logs.Count > 9)
        {
            ClearDebugConsole();
        }
        if(Input.GetKeyDown("2"))
        {
            GenerateRandomMessage();
        }
        if(Input.GetKeyDown("3"))
        {
            ClearDebugConsole();
        }
        if (Input.GetKeyDown("4"))
        {
            OpenDebugConsole();
        }
        if (Input.GetKeyDown("5"))
        {
            CloseDebugConsole();
        }
    }


    public void OpenDebugConsole()
    {
        Debug.Log("Entered OpenKeyword");
        debugConsole.SetActive(true);
        showing = true;
    }

    public void CloseDebugConsole()
    {
        Debug.Log("Entered CloseKeyword");
        debugConsole.SetActive(false);
        showing = false;

    }

    public void ClearDebugConsole()
    {
        logs.Clear();
        text.text = "";
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if(showing == false)
        {
            return;
        }
        string nullstring = "";
        text.text = nullstring;
        string temp = text.text;

        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type
        });

        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];
            text.color = logTypeColors[log.type];
            text.text += temp + "\n" + log.message;
            text.text += "\n " + log.stackTrace;
        }
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
