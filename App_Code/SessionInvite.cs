using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
public class SessionInvite
{
    public int sessionID { get; set; }
    public string leaderUsername { get; set; }
    public string title { get; set; }
    public string tag { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
}