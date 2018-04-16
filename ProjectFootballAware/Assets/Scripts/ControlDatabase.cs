using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlDatabase : MonoBehaviour
{
    FirebaseDatabase mDatabase;
    public string myUsername;
    public GameObject prefabButton;
    public RectTransform ParentPanel;
    public bool waitingAnswer;
    public GameObject imageWaiting;
    public static ControlDatabase cb;
    public Match myMatch;
    public GameObject ball;
    public string partnerName;

    public void Awake()
    {
        if (cb == null)
        {
            cb = this;
            DontDestroyOnLoad(this);

        }
        else if (cb != this)
        {
            Destroy(this);
        }
    }



    // Use this for initialization
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://footbalaware.firebaseio.com/");

        // Get the root reference location of the database.
        mDatabase = FirebaseDatabase.DefaultInstance;

        myUsername = "";
        connnectToLobby();
        waitingAnswer = false;

        Input.location.Start();

    }

    public void writeNewUser()
    {
        //Obtiene el nombre del usuario
        string key = GameObject.Find("InputField").GetComponent<InputField>().text;

        //Si el username no esta vacio permite conectarse
        if (myUsername.Equals(""))
        {

            myUsername = key;
            GameObject.Find("username").GetComponent<Text>().text = myUsername;
            FootballUser user = new FootballUser(key);
            string json = JsonUtility.ToJson(user);

            //actualizar datos
            //mDatabase.RootReference.Child("users").Child(userId).Child("username").SetValueAsync("pepe");

            //Generar Key unica
            //string key = mDatabase.RootReference.Child("users").Push().Key;

            //agregar datos
            mDatabase.RootReference.Child("users").Child(key).SetRawJsonValueAsync(json);

            //Una vez que nos conectamos debemos abrir un canal de comunicacion para recibir llamadas de los demas
            //leemos las llamadas que recibamos de otros
            FirebaseDatabase.DefaultInstance
          .GetReference("users").Child(key).ValueChanged += MyHandleValueChanged;
            FirebaseDatabase.DefaultInstance
    .GetReference("users").Child(key)
    .GetValueAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            Debug.Log("Can not connect with " + key);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary = (Dictionary<string, object>)snapshot.Value;
            string latitud = Input.location.lastData.latitude.ToString();
            string longitud = Input.location.lastData.longitude.ToString();
            string altitud = Input.location.lastData.altitude.ToString();
            mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("latitud").SetValueAsync(latitud);
            mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("longitud").SetValueAsync(longitud);
            mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("altitud").SetValueAsync(altitud);

            waitingAnswer = true;
        }
    });
        }
    }

    public void askUserToPlay(string id)
    {

        if (myUsername.Equals(""))
        {
            Debug.Log("You dont have an user yet");
            return;
        }
        //obtenemos el nombre del jugador que esta cerca
        string userName = GameObject.Find("InputField").GetComponent<InputField>().text;

        Debug.Log("Trying to play with" + id);

        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(id)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              Debug.Log("Can not connect with " + id);
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Dictionary<string, object> dictionary = new Dictionary<string, object>();
              dictionary = (Dictionary<string, object>)snapshot.Value;

              //si esa persona no ha sido invitada aun
              if (dictionary["invited"].ToString().Equals("no") || dictionary["playing"].ToString().Equals("no"))
              {
                  mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("whoInvited").SetValueAsync(myUsername);
                  mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("invited").SetValueAsync("yes");
                  Debug.Log("You have invited " + dictionary["username"].ToString());

                  //Nos desconectamos del lobi y establecemos los particpantes de la partida
                  disconnectFromLobby();
                  string player1 = myUsername;
                  string player2 = dictionary["username"].ToString();
                  partnerName = dictionary["username"].ToString();


                  //Generar Key unica
                  string key = mDatabase.RootReference.Child("matches").Push().Key;

                  //Dejamos la referencia de la partida actual en cada usuario
                  mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("playing").SetValueAsync(key);
                  //mDatabase.RootReference.Child("users").Child(dictionary[myUsername].ToString()).Child("playing").SetValueAsync(key);


                  //Crea y envia la partida
                  myMatch = new Match(player1, player2, key);
                  myMatch.isball = myUsername;
                  sendMatch(myMatch);

                  //Se abre la escena del juego
                  SceneManager.LoadScene("Football Field");

                  Debug.Log("el usuario " + dictionary["whoInvited"] + " te invito a jugar");
              }
              else
              {
                  Debug.Log("This person has been invited or is playing " + dictionary["username"].ToString());
              }

          }
      });


        //actualizar datos
        //mDatabase.RootReference.Child("users").Child(userName).Child("email").SetValueAsync("pepepe");


    }



    public void leave()
    {

        //Limpiamos el registro de nuestro propio usuario
        mDatabase.RootReference.Child("users").Child(myUsername).Child("invited").SetValueAsync("no");
        mDatabase.RootReference.Child("users").Child(myUsername).Child("whoInvited").SetValueAsync("none");
        mDatabase.RootReference.Child("users").Child(myUsername).Child("playing").SetValueAsync("no");

        //Cambiamos a la escena principal del titulo y nos conectamos devuelta al loby para ser visible para otros usuarios
        SceneManager.LoadScene("Title Screen");

        //Referenciamos el metodo de crear al nuevo boton creado
        UnityEngine.Events.UnityAction action1 = () => { this.writeNewUser(); };
        GameObject.FindGameObjectWithTag("btnConnect").GetComponent<Button>().onClick.AddListener(action1);

        connnectToLobby();


    }
    public void connnectToLobby()
    {
        //Leer un solo usuario
        /*  FirebaseDatabase.DefaultInstance
        .GetReference("users").Child(userID).ValueChanged += HandleValueChanged;*/

        //leer la lista completa de usuarios
        FirebaseDatabase.DefaultInstance
      .GetReference("users").ValueChanged += HandleValueChangedGroups;
    }

    public void disconnectFromLobby()
    {

        //leer la lista completa de usuarios
        FirebaseDatabase.DefaultInstance
      .GetReference("users").ValueChanged -= HandleValueChangedGroups;

    }



    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        dictionary = (Dictionary<string, object>)args.Snapshot.Value;
        string name = (string)dictionary["username"];

        //string y=  args.Snapshot.Key;
        //string x = args.Snapshot.GetRawJsonValue();

        //FootballUser user=
        //Debug.Log(""+user.username);


        GameObject.Find("textData").GetComponent<Text>().text = name;

        // Do something with the data in args.Snapshot
    }


    void MyHandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //Transformamos nuestros datos en un diccionario de datos
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary = (Dictionary<string, object>)args.Snapshot.Value;

        //si alguien nos invito a jugar lo veremos aqui
        if (dictionary["invited"].ToString().Equals("yes") && !dictionary["playing"].ToString().Equals("no"))
        {

            disconnectFromLobby();
            string player1 = dictionary["whoInvited"].ToString();
            string player2 = myUsername;
            partnerName = dictionary["whoInvited"].ToString();

            //Obtenemos la llave unica del match
            string key = dictionary["playing"].ToString();

            //string key = mDatabase.RootReference.Child("maches").Push().Key;
            //Crea y envia la partida
            myMatch = new Match(player1, player2, key);
            myMatch.isball = dictionary["whoInvited"].ToString();

            sendMatch(myMatch);

            //Se abre la escena del juego
            SceneManager.LoadScene("Football Field");

            Debug.Log("el usuario " + dictionary["whoInvited"] + " te invito a jugar");
        }

    }

    void HandleValueChangedGroups(object sender, ValueChangedEventArgs args)
    {

        GameObject[] objects = GameObject.FindGameObjectsWithTag("dynamicUser");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        //Texto colocado dentro de la lista
        string playerLists = "Players ready to play : \n";

        string latitud, altitud, longitud;



        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //El metodo nos devuelve un diccionario de los datos que le hemos solicitado
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary = (Dictionary<string, object>)args.Snapshot.Value;

        float startbuttonpos = 93.725f;
        //recorre el diccionario encontrado
        foreach (var item in dictionary)
        {
            //Se debe hacer otro diccionario para separar los atributos de los propios objectos
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item.Value;
            altitud = dictionary2["altitud"].ToString();
            latitud = dictionary2["latitud"].ToString();
            longitud = dictionary2["longitud"].ToString();

            if (float.Parse(altitud) > (Input.location.lastData.altitude + 3) &&
                float.Parse(longitud) > (Input.location.lastData.longitude + 5) &&
                float.Parse(latitud) > (Input.location.lastData.latitude + 5))
            {

            }
            else if (float.Parse(altitud) < (Input.location.lastData.altitude - 3) &&
                      float.Parse(longitud) < (Input.location.lastData.longitude - 5) &&
                      float.Parse(latitud) < (Input.location.lastData.latitude - 5))
            {

            }
            else
            {

                playerLists += item.Key + "\n";

                //Inicializamos una copia del objecto
                GameObject goButton = Instantiate(prefabButton);

                //le cambiamos el nombre a dicha copia para que sea distinta a las demas
                goButton.name = "btn" + item.Key;

                //posicionamos el objecto en la parte adecuada
                goButton.GetComponent<RectTransform>().position = new Vector2(25f, startbuttonpos);
                startbuttonpos -= 80f;
                goButton.transform.SetParent(ParentPanel, false);

                //Cambiamos el nombre del boton al nombre real
                goButton.GetComponentInChildren<Text>().text = item.Key.ToString();

                goButton.GetComponent<Button>().onClick.RemoveAllListeners();
                // goButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(item.Key));
                //goButton.transform.localScale = new Vector3(1, 1, 1);

                UnityEngine.Events.UnityAction action1 = () => { this.askUserToPlay(item.Key); };
                goButton.GetComponent<Button>().onClick.AddListener(action1);

                //Button tempButton = goButton.GetComponent<Button>();
                //tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
            }
            // Se actualiza la lista de nombres
            GameObject.Find("playersList").GetComponent<Text>().text = playerLists;
        }
    }

    public void ButtonClicked(string buttonNo)
    {
        Debug.Log("Button clicked = " + buttonNo);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendMatch(Match match)
    {
        string json = JsonUtility.ToJson(match);
        //agregar datos
        mDatabase.RootReference.Child("matches").Child(match.id).SetRawJsonValueAsync(json);

        //Una vez que nos conectamos debemos abrir un canal de comunicacion para recibir llamadas de los demas
        //leemos las llamadas que recibamos de otros
        FirebaseDatabase.DefaultInstance
      .GetReference("matches").Child(match.id).ValueChanged += MyHandleValueChangedMatch;
    }

    void MyHandleValueChangedMatch(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //Transformamos nuestros datos en un diccionario de datos
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary = (Dictionary<string, object>)args.Snapshot.Value;


       if (dictionary["isball"].ToString().Equals(myUsername))  {
            myMatch.isball = myUsername;
            float vectorx = float.Parse(dictionary["vectorX"].ToString());
            float vectory = float.Parse(dictionary["vectorY"].ToString());
            float directionX = float.Parse(dictionary["directionX"].ToString());
            float directionY = float.Parse(dictionary["directionY"].ToString());
            float force = float.Parse(dictionary["force"].ToString());
            Debug.Log("Pelota recibida: vector x " + vectorx + " vector y" + vectory);

            //haz algo con los datos
            if (!dictionary["player1Score"].ToString().Equals("5") || !dictionary["player2Score"].Equals("5"))
            {
                if (vectory != 0 && vectorx != 0)
                {
                    Debug.Log("Pelota recibida: vector x " + vectorx + " vector y" + vectory);
                    GameControl.instance.InstantiateBall(vectorx, vectory, directionX, directionY, force);
                }
            }
        }
    }

    public void UpdateMatch(float x, float y, float directionX, float directionY, float force)
    {
        myMatch.isball = partnerName;
        myMatch.vectorX = x;
        myMatch.vectorY = y;
        myMatch.directionX = directionX;
        myMatch.directionY = directionY;
        myMatch.force = force;
        //myMatch.player1Score = score1;
        //myMatch.player2Score = score2;

        string json = JsonUtility.ToJson(myMatch);

        //actualizar datos
        mDatabase.RootReference.Child("matches").Child(myMatch.id).SetRawJsonValueAsync(json);
    }

    public void OnGUI()
    {

    }
}
