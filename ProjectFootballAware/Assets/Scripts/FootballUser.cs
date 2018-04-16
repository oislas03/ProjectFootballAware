using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballUser
{
    public string username;
    public string invited;
    public string playing;
    public string whoInvited;
    public string latitud;
    public string longitud;
    public string altitud;

    public FootballUser()
    {
    }

    public FootballUser(string username)
    {
        this.username = username;
        this.invited = "no";
        this.playing = "no";
        this.whoInvited = "none";
        this.latitud = "0";
        this.longitud = "0";
        this.altitud = "0";


    }
}

