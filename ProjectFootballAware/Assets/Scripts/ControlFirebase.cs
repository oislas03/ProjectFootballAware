using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;

public class ControlFirebase : MonoBehaviour
{
    FirebaseDatabase mDatabase;
    public string myUsername;
    public GameObject prefabButton;
    public RectTransform ParentPanel;
    public bool waitingAnswer;
    public GameObject imageWaiting;

   
    // Use this for initialization
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://footbalaware.firebaseio.com/");

        // Get the root reference location of the database.
        mDatabase = FirebaseDatabase.DefaultInstance;

        myUsername = "";
        readData("0");
        waitingAnswer = false;

    }

    public void writeNewUser()
    {
        //Obtiene el nombre del usuario
        string key = GameObject.Find("InputField").GetComponent<InputField>().text;

        //Si el username no esta vacio permite conectarse
        if (myUsername.Equals(""))
        {

            myUsername = key;
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
        }
    }

    public void askUserToPlay(string id)
    {
        //obtenemos el nombre del jugador que esta cerca
        string userName = GameObject.Find("InputField").GetComponent<InputField>().text;

        Debug.Log("Trying to play with"+ id);

        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(id)
      .GetValueAsync().ContinueWith(task => {
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
              if (dictionary["invited"].ToString().Equals("no")|| dictionary["playing"].ToString().Equals("no"))
              {
                  mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("whoInvited").SetValueAsync(myUsername);
                  mDatabase.RootReference.Child("users").Child(dictionary["username"].ToString()).Child("invited").SetValueAsync("yes");
                  Debug.Log("You have invited "+ dictionary["username"].ToString());

                  waitingAnswer = true;
              }
              else {
                  Debug.Log("This person has been invited or is playing " + dictionary["username"].ToString());
              }

          }
        });


        //actualizar datos
        //mDatabase.RootReference.Child("users").Child(userName).Child("email").SetValueAsync("pepepe");


    }

    public void readData(string userID)
    {
        //Leer un solo usuario
        /*  FirebaseDatabase.DefaultInstance
        .GetReference("users").Child(userID).ValueChanged += HandleValueChanged;*/

        //leer la lista completa de usuarios
        FirebaseDatabase.DefaultInstance
      .GetReference("users").ValueChanged += HandleValueChangedGroups;
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
        if (dictionary["invited"].ToString().Equals("yes")) {

            Debug.Log("el usuario " + dictionary["whoInvited"]+" te invito a jugar");
        }

    }

    void HandleValueChangedGroups(object sender, ValueChangedEventArgs args)
    {
        //Texto colocado dentro de la lista
        string playerLists = "Players ready to play : \n";




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


            playerLists += item.Key + "\n";


            //Inicializamos una copia del objecto
            GameObject goButton = Instantiate(prefabButton);
            
            //le cambiamos el nombre a dicha copia para que sea distinta a las demas
            goButton.name = "btn"+item.Key;
            
           

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



    public void ButtonClicked(string buttonNo)
    {
        Debug.Log("Button clicked = " + buttonNo);
    }

    // Update is called once per frame
    void Update()
    {

        

    }

    public void OnGUI()
    {
        if (waitingAnswer) {
            imageWaiting.SetActive(true);
            
        }
    }
}
