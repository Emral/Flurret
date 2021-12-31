using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugExtensions
{
    // Start is called before the first frame update
    public static void Log(this object o, string title = "Log")
    {
        Debug.Log(title + ": " + o.ToString());
    }

    public static void Log(params object[] o)
    {
        string s = string.Empty;

        foreach (object obj in o)
        {
            s += obj.ToString() + " ; ";
        }

        Debug.Log(s);
    }
}
