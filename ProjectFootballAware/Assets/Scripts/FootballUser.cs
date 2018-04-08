using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballUser
{
    public string username;
    public string invited;
    public string playing;
    public string whoInvited;

    public FootballUser()
    {
    }

    public FootballUser(string username)
    {
        this.username = username;
        this.invited = "no";
        this.playing = "no";
        this.whoInvited = "none";
        
    }
}
