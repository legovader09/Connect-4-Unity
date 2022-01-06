using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Session
{
    public static Connection playerConn { get; set; } = null;
    public static bool isHost { get; set; } = false;
}
