using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Steamworks;
using System.IO;
using System.Reflection;
using System;
using Random = UnityEngine.Random;

//SOME NOTES FOR SAM 
//FUNCTIONS WITH PUNRPC ON TOP ARE CALLED ON ALL COMPUTERS
//this is the game manager script it handles most things

//ayy sam
//bich
//bich
//bich
//bich
//bich
//bich
//gimme my background back, muffin

public class GameManager : Photon.PunBehaviour
{
    //this are all the variables

    //this is this script but static so it can be accessed by other scripts
    public static GameManager instance;

    //the localplayer's gameobject
    public GameObject localPlayer;

    //default spawnpoint if none found
    GameObject defaultSpawnPoint;

    //player number id
    int playerNum = 0;

    // list of only as big as the amount of players connected
    public List<Player> players = new List<Player>();

    //always 4 in size even if there's a player to fill the slot
    public List<Player> playersFull = new List<Player>();

    //list of weapons to spawn (all the weapons in the game)
    public List<GameObject> weapons = new List<GameObject>();

    //number of players dead (obviously m8)
    public int playersDead = 0;

    public int playersDeadTeam1 = 0;
    public int playersDeadTeam2 = 0;


    //the current round, can go above max round because all 4 players could win 9 rounds when the max is 10
    public int round;
    //basically a scoreboard with a stupid name, starts at the beginning of the round and when player presses tb
    public GameObject roundStartText;
    public GameObject roundStart2V2Text;


    //the text where you but your name in the mainmenu
    public Text nameText;

    //weather or not we're resetting after a round is complete
    public bool resetting = false;

    //list of player scores 0 is player 1's score and 4 is player 4's score
    public int[] playerScores;


    //list of team scores 0 is team 1's score and 2 is team 2's score
    public int[] teamScores;

    //the win scoreboard (the one that says losers on it)
    public GameObject win;

    //a list of sorted ids from least to greatest least is the 1st player and greatest is the last player.
    public List<int> actorSortedId = new List<int>();

    // the crosshair
    public RectTransform crossHair;

    public Image crossHairIMG;

    //countdown text duh
    public Text countDowntext;

    //countdown 
    public int countDown = 4;

    //max number of rounds set in lobby.
    public int numberOfRounds = 5;

    //the template for the kill feed, spawn everytime someone kills another, and edit from there.
    public GameObject killFeed;

    //the round winner gameobject 
    public GameObject roundWinnerGO;
    public GameObject twovtwoRoundWinnerGO;
    //list of player spawnpoints
    public List<Transform> spawnPoints = new List<Transform>();
    //weather or not the round is over
    bool roundOver = false;

    //another sorted id thing
    public List<int> playerNums = new List<int>();

    //the menu object, comes up when your press esc
    public GameObject menu;

    //weather or not the game is focused
    public bool isFocused;

    //connection text shown in menu in newer build not on kong
    public Text connectionText;

    //are we reconnecting to the servers, not the game room.
    public bool reconnecting;

    public int hat;
    public int backpack;

    public bool musicOn;
    public bool soundOn;
    public GameObject credits;
    public GameObject controls;

    public Button exitMatchButton;
    public GameObject lobbyMenuMusic;
    public GameObject lobbyMusicPrefab;

    public AudioClip winMusic;
    public AudioSource winAU;

    public Sprite disconnectedSprite;
    public bool ending;
    public Text eliminated;
    public bool startingNextRound;
    public InputField chatInput;
    public GameObject chatItem;
    public bool IsChatting;

    public Color player1Color;
    public Color player2Color;
    public Color player3Color;
    public Color player4Color;

    public string player1ColorHex;
    public string player2ColorHex;
    public string player3ColorHex;
    public string player4ColorHex;

    public Button eu;
    public Button us;
    public Button asia;
    public Button sa;

    public bool connectingToRegionEu;
    public bool connectingToRegionUs;
    public bool connectingToRegionAsia;
    public bool connectingToRegionSA;
    public Transform mouse;

    public int deaths;
    public int kills;

    public Text scoreTextst;
    public Text privateGameCode;

    public enum CreateGameType { Private, Public };
    public CreateGameType currentCreateGameType;
    public Text createGameTypeText;

    public enum JoinGameType { Random, Private };
    public JoinGameType joinGameType;
    public Text joinGameTypeText;
    public GameObject joinViaCodeOBJ;
    public GameObject joinViaCodeOBJOUTLINE;

    public int resolutionType;
    public Text resolutionText;
    public GameObject leaderBoardItem;
    public Transform leaderBoardT;
    public Color ACTIVELEADERBOARDCOLOR;
    public Color inactiveleaderboardColor;

    public List<GameObject> leaderBoards;

    public GameObject leaderBoard;

    public Color youColor;

    public Image crossHairImg;

    public List<GameObject> gores;

    public List<int> defids = new List<int>();

    string saveFile;
    public GameData gameData = new GameData();

    public bool visible;

    public bool gunGame;

    public bool twovtwo;


    public void readFile()
    {
        // Does the file exist?
        if (File.Exists(saveFile))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFile);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            gameData = JsonUtility.FromJson<GameData>(fileContents);
        }
    }

    public void writeFile()
    {
        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson(gameData);
        //File.Create(saveFile);
        // Write JSON to file.
        File.WriteAllText(saveFile, jsonString);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Discord()
    {
        SteamFriends.OpenWebOverlay("https://discord.gg/9j2K2wTf9Z");
    }
    public void Insta()
    {
        SteamFriends.OpenWebOverlay("https://www.instagram.com/kontrollablegames");
    }
    public void YT()
    {
        SteamFriends.OpenWebOverlay("https://www.youtube.com/channel/UC5CuXFmDjOC1Irps_6538qA");
    }
    public void ActivateLeaderboard(int i)
    {
        foreach (GameObject go in leaderBoards)
        {
            foreach (Transform t in go.transform.Find("LEADERBOARDLAYOUT"))
            {
                Destroy(t.gameObject);
            }
            go.transform.Find("LEADERBOARDLAYOUT").gameObject.SetActive(false);
            go.transform.Find("ARROW").gameObject.SetActive(false);
            go.GetComponent<Image>().color = inactiveleaderboardColor;
        }
        leaderBoards[i].transform.Find("LEADERBOARDLAYOUT").gameObject.SetActive(true);
        leaderBoards[i].transform.Find("ARROW").gameObject.SetActive(true);
        leaderBoards[i].GetComponent<Image>().color = ACTIVELEADERBOARDCOLOR;
        leaderBoardT = leaderBoards[i].transform.Find("LEADERBOARDLAYOUT").transform;
        GetLeaderboards(leaderBoards[i].transform.name);
    }
    async void GetLeaderboards(string leaderboardname)
    {
        var leaderboard = await Steamworks.SteamUserStats.FindLeaderboardAsync(leaderboardname);
        SteamId[] steamid = new SteamId[1];
        steamid[0] = SteamClient.SteamId;
        var result = await leaderboard.Value.GetScoresAsync(10);

        var result2 = await leaderboard.Value.GetScoresForUsersAsync(steamid);

        if (result != null && result2 != null)
        {
            foreach (var e_ in result2)
            {
                GameObject newValmine = Instantiate(leaderBoardItem, Vector3.zero, Quaternion.identity);
                newValmine.transform.Find("USERNAME").transform.GetComponent<Text>().color = youColor;
                newValmine.transform.Find("USERNAME").transform.GetComponent<Text>().text = "You";
                newValmine.transform.Find("USERNAME").transform.Find("SCOREPANEL").transform.Find("SCORE").GetComponent<Text>().text = e_.Score.ToString();
                newValmine.transform.Find("RANKINGPANEL").transform.Find("RANKING").GetComponent<Text>().text = "#" + e_.GlobalRank.ToString();
                newValmine.transform.SetParent(leaderBoardT, false);
            }

            foreach (var e in result)
            {
                GameObject newVal = Instantiate(leaderBoardItem, Vector3.zero, Quaternion.identity);
                newVal.transform.Find("USERNAME").transform.GetComponent<Text>().text = e.User.Name;
                newVal.transform.Find("USERNAME").transform.Find("SCOREPANEL").transform.Find("SCORE").GetComponent<Text>().text = e.Score.ToString();
                newVal.transform.Find("RANKINGPANEL").transform.Find("RANKING").GetComponent<Text>().text = "#" + e.GlobalRank.ToString();
                newVal.transform.SetParent(leaderBoardT, false);
            }
        }
    }
    async void UpdateLeaderboards(string leaderboardname, int score)
    {
        var leaderboard = await Steamworks.SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardname,
                                                                    Steamworks.Data.LeaderboardSort.Ascending,
                                                                    Steamworks.Data.LeaderboardDisplay.Numeric);
        var result = leaderboard.Value.ReplaceScore(score);
    }
    public void ChangeGameType()
    {
        if (currentCreateGameType == CreateGameType.Public)
        {
            currentCreateGameType = CreateGameType.Private;
            createGameTypeText.text = "Private";
        }
        else if (currentCreateGameType == CreateGameType.Private)
        {
            currentCreateGameType = CreateGameType.Public;
            createGameTypeText.text = "Public";
        }
    }
    public void ChangeResolution(bool up)
    {
        if (up)
        {
            resolutionType++;
            if (resolutionType > 2)
            {
                resolutionType = 0;
            }
        }
        else
        {
            resolutionType--;
            if (resolutionType < 0)
            {
                resolutionType = 2;
            }
        }

        if (resolutionType == 2)
        {
            Screen.SetResolution(852, 420, Screen.fullScreen);
            resolutionText.text = "852x420";
        }
        if (resolutionType == 1)
        {
            Screen.SetResolution(1280, 720, Screen.fullScreen);
            resolutionText.text = "1280x720";
        }
        if (resolutionType == 0)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
            resolutionText.text = "1920x1080";
        }
    }
    public void FullScreen(Text text)
    {
        if (Screen.fullScreen)
        {
            text.text = "Fullscreen: Off";
        }
        else
        {
            text.text = "Fullscreen: On";
        }
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);

    }
    public void ChangeJoinType()
    {
        if (joinGameType == JoinGameType.Random)
        {
            joinGameType = JoinGameType.Private;
            joinGameTypeText.text = "Private";
        }
        else if (joinGameType == JoinGameType.Private)
        {
            joinGameType = JoinGameType.Random;
            joinGameTypeText.text = "Random";
        }
    }
    public void Credits()
    {
        menu.SetActive(false);
        controls.SetActive(false);
        credits.SetActive(true);
    }
    public void Leaderboards()
    {
        menu.SetActive(false);
        controls.SetActive(false);
        credits.SetActive(false);
        leaderBoard.SetActive(true);
        ActivateLeaderboard(0);
    }
    public void CloseLeaderboards()
    {
        menu.SetActive(false);
        controls.SetActive(false);
        credits.SetActive(false);
        leaderBoard.SetActive(false);
    }
    public void Controls()
    {
        menu.SetActive(false);
        credits.SetActive(false);
        controls.SetActive(true);
    }
    public void CloseCredits()
    {
        credits.SetActive(false);
    }
    public void CloseControls()
    {
        controls.SetActive(false);
    }
    //called when game loses focus and regains it
    void OnApplicationFocus(bool hasFocus)
    {
        isFocused = hasFocus;
        if (!hasFocus)
        {
            IsChatting = false;
            chatInput.text = string.Empty;
            chatInput.gameObject.SetActive(false);
            //menu.SetActive(true);
        }
        if (hasFocus)
        {

        }
    }

    //called when game starts
    void Awake()
    {
        //not important
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        saveFile = Application.persistentDataPath + "/gamedata.json";

        print(saveFile);
        //not important
        //PlayerPrefs.DeleteAll();

        //don't destroy this object when loading levels maps lobby etc
        DontDestroyOnLoad(gameObject);
        //set instance
        instance = this;
        //so if the masterclient loads a level all players follow
        PhotonNetwork.automaticallySyncScene = true;
        //defualt spawn point 
        defaultSpawnPoint = new GameObject("Default SpawnPoint");
        defaultSpawnPoint.transform.position = new Vector3(0, 0.5f, 0);
        defaultSpawnPoint.transform.SetParent(transform, false);

        lobbyMenuMusic = Instantiate(lobbyMusicPrefab, transform.position, transform.rotation) as GameObject;
        lobbyMenuMusic.name = "MusicManager";
        DontDestroyOnLoad(lobbyMenuMusic);


    }

    public void Options()
    {
        menu.SetActive(true);
    }
    public void Resume()
    {
        menu.SetActive(false);
    }
    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }
    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
    public void OptionsMusic(Text t)
    {
        musicOn = !musicOn;
        gameData.music = musicOn;
        writeFile();
        if (musicOn)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name == "MusicManager")
                {
                    aud.mute = false;
                }
            }
            t.text = "Music: ON";
        }
        else
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name == "MusicManager")
                {
                    aud.mute = true;
                }
            }
            t.text = "Music: OFF";
        }
    }
    public void OptionsSound(Text t)
    {
        soundOn = !soundOn;
        gameData.sound = soundOn;
        writeFile();

        if (soundOn)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name != "MusicManager")
                {
                    aud.mute = false;
                }
            }
            t.text = "Sound: ON";
        }
        else
        {

            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name != "MusicManager")
                {
                    aud.mute = true;
                }
            }
            t.text = "Sound: OFF";
        }

    }

    //also called at the start of the game
    void Start()
    {
        readFile();
        Application.targetFrameRate = 120;
        //GetLeaderboards("Losses");

        musicOn = gameData.music;
        soundOn = gameData.sound;

        if (musicOn)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name == "MusicManager")
                {
                    aud.mute = false;
                }
            }
        }
        else
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name == "MusicManager")
                {
                    aud.mute = true;
                }
            }
        }


        if (soundOn)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name != "MusicManager")
                {
                    aud.mute = false;
                }
            }
        }
        else
        {

            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name != "MusicManager")
                {
                    aud.mute = true;
                }
            }
        }


        //connect to the servers
        PhotonNetwork.ConnectUsingSettings("steamv1.9");
    }
    //caled when join game pressed
    public void JoinGame()
    {
        if (privateGameCode.text != "")
        {
            PhotonNetwork.JoinRoom(privateGameCode.text.ToUpper());
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    void OnPhotonRandomJoinFailed()
    {
        CreateGame();
    }
    public void EnableJoinViaCode()
    {
        joinViaCodeOBJ.gameObject.SetActive(!joinViaCodeOBJ.activeInHierarchy);
        joinViaCodeOBJOUTLINE.gameObject.SetActive(!joinViaCodeOBJOUTLINE.activeInHierarchy);
        if (!joinViaCodeOBJ.activeInHierarchy)
        {
            privateGameCode.transform.parent.GetComponent<InputField>().text = "";
            privateGameCode.text = "";
        }
    }
    //called when create game pressed
    public void CreateGame()
    {
        string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char c = st[Random.Range(0, st.Length)];

        //don't mind these two lines
        int roomNameI = PhotonNetwork.countOfRooms;
        string roomName = c + roomNameI.ToString() + Random.Range(0, 999).ToString() + Random.Range(0, 9).ToString();

        //set max players and creat/join room
        RoomOptions ro = new RoomOptions();
        if (currentCreateGameType == CreateGameType.Private)
        {
            ro.IsVisible = false;
        }
        ro.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, null);
    }
    //called when we join a room
    public override void OnJoinedRoom()
    {
        //if we're the host load the lobby scene
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        // set default name name not set
        if (nameText.text == string.Empty)
        {
            PhotonNetwork.player.NickName = "Noob " + PhotonNetwork.player.ID.ToString();
        }
        //else set player name and change all of thier name texts to it
        else
        {
            PhotonNetwork.player.NickName = nameText.text;
            int np = 2;
            bool nameExists;
            do
            {
                nameExists = false;
                foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
                {
                    if (p.NickName == PhotonNetwork.player.NickName)
                    {
                        nameExists = true;
                        break;
                    }
                }
                if (nameExists)
                {
                    PhotonNetwork.player.NickName = nameText.text + np.ToString();
                    np++;
                }
            } while (nameExists);
        }

        Debug.Log("Joined Room");
    }
    //if we've dissconnected or the game is over load menu scene
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }
    public void ThrowWeaponPressed()
    {
        if (localPlayer)
        {
            localPlayer.GetComponent<PlayerPickUp>().throwweaponpressed = true;
        }
    }
    //so if a host discconects during countdonw it doesn't get stuck
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            if (countDown > -1)
            {
                countDown = 4;
                if (PhotonNetwork.isMasterClient)
                {
                    StartCoroutine("CountDown");
                }
            }
            if (!twovtwo)
            {
                if (playersDead > 0 && playersDead == players.Count && !roundOver && !resetting)
                {
                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
            }
            else
            {
                if (playersDead > 0 && playersDead == players.Count && !roundOver && !resetting)
                {
                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
            }
        }
    }

    //to help sort player ids
    public int SortByID2(int p1, int p2)
    {
        return p1.CompareTo(p2);
    }
    //function used when your press esc and press exit
    public void ExitGame()
    {
        ending = false;
        AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource aud in audios)
        {
            if (aud.name == "MusicManager")
            {
                aud.gameObject.SetActive(true);
            }
        }
        Time.timeScale = 1f;
        StopAllCoroutines();
        exitMatchButton.interactable = false;
        menu.SetActive(false);
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        localPlayer = null;
        roundOver = false;
        players.Clear();
        spawnPoints.Clear();
        playersDead = 0;

        playersDeadTeam1 = 0;
        playersDeadTeam2 = 0;
        actorSortedId.Clear();
        playersFull.Clear();
        playerNums.Clear();
        round = 1;
        resetting = false;
        for (int i = 0; i < playerScores.Length; i++)
        {
            playerScores[i] = 0;
        }
        teamScores[0] = 0;
        teamScores[1] = 0;
        countDown = 4;
        GameObject hud = GameObject.Find("HUD");
        if (hud != null)
        {
            Transform ammoAndShield = hud.transform.FindAnyChild<Transform>("AMMO AND SHIELD");
            if (ammoAndShield != null)
            {
                ammoAndShield.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("AMMO AND SHIELD object not found!");
            }
        }
        else
        {
            Debug.LogError("HUD object not found!");
        }
        CancelInvoke("EndGame");
        numberOfRounds = 5;
    }
    public void Reconnect()
    {
        if (!connectingToRegionEu && !connectingToRegionUs && !connectingToRegionSA && !connectingToRegionAsia)
        {
            eu.interactable = false;
            us.interactable = true;
            asia.interactable = true;
            sa.interactable = true;
            PhotonNetwork.ConnectUsingSettings("steamv1.9");
        }
    }
    public override void OnConnectedToMaster()
    {
        if (connectingToRegionEu)
        {
            us.interactable = true;
            asia.interactable = true;
            sa.interactable = true;
        }
        if (connectingToRegionUs)
        {
            eu.interactable = true;
            asia.interactable = true;
            sa.interactable = true;
        }
        if (connectingToRegionAsia)
        {
            us.interactable = true;
            eu.interactable = true;
            sa.interactable = true;
        }
        if (connectingToRegionSA)
        {
            eu.interactable = true;
            us.interactable = true;
            asia.interactable = true;
        }

        if (PhotonNetwork.CloudRegion == CloudRegionCode.eu)
        {
            eu.interactable = false;
            us.interactable = true;
            asia.interactable = true;
            sa.interactable = true;
        }
        if (PhotonNetwork.CloudRegion == CloudRegionCode.us)
        {
            eu.interactable = true;
            us.interactable = false;
            asia.interactable = true;
            sa.interactable = true;
        }
        if (PhotonNetwork.CloudRegion == CloudRegionCode.asia)
        {
            eu.interactable = true;
            us.interactable = true;
            asia.interactable = false;
            sa.interactable = true;
        }
        if (PhotonNetwork.CloudRegion == CloudRegionCode.sa)
        {
            eu.interactable = true;
            us.interactable = true;
            asia.interactable = true;
            sa.interactable = false;
        }

        connectingToRegionEu = false;
        connectingToRegionUs = false;
        connectingToRegionAsia = false;
        connectingToRegionSA = false;
        reconnecting = false;
        CancelInvoke("Reconnect");
    }
    public override void OnDisconnectedFromPhoton()
    {
        if (connectingToRegionEu)
        {
            PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, "steamv1.9");
        }
        if (connectingToRegionUs)
        {
            PhotonNetwork.ConnectToRegion(CloudRegionCode.us, "steamv1.9");
        }
        if (connectingToRegionAsia)
        {
            PhotonNetwork.ConnectToRegion(CloudRegionCode.asia, "steamv1.9");
        }
        if (connectingToRegionSA)
        {
            PhotonNetwork.ConnectToRegion(CloudRegionCode.sa, "steamv1.9");
        }
        ExitGame();
    }
    [PunRPC]
    void Chat(string playername, string text)
    {
        try
        {
            GameObject chat = Instantiate(chatItem, Vector3.zero, Quaternion.identity) as GameObject;
            chat.transform.SetParent(GameObject.Find("HUD").transform.Find("Chat Canvas").transform, false);
            chat.transform.SetSiblingIndex(0);

            chat.GetComponent<Text>().text = text;
            RectTransform rectTransform = chat.GetComponent<RectTransform>();
            rectTransform.Rotate(new Vector3(0, 0, 180));
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (PhotonNetwork.playerList.Length >= i)
                    {
                        if (PhotonPlayer.Find(GameObject.Find("LobbyManager").GetComponent<LobbyManager>().actorSortedId[i - 1]).NickName == playername)
                        {
                            if (i == 1)
                            {
                                chat.GetComponent<Text>().text = "<color=#" + player1ColorHex + ">" + playername + "</color>" + ": " + text;
                            }
                            if (i == 2)
                            {
                                chat.GetComponent<Text>().text = "<color=#" + player2ColorHex + ">" + playername + "</color>" + ": " + text;
                            }
                            if (i == 3)
                            {
                                chat.GetComponent<Text>().text = "<color=#" + player3ColorHex + ">" + playername + "</color>" + ": " + text;
                            }
                            if (i == 4)
                            {
                                chat.GetComponent<Text>().text = "<color=#" + player4ColorHex + ">" + playername + "</color>" + ": " + text;
                            }
                        }

                    }
                }
            }
            else
            {

                foreach (Player p in players)
                {
                    if (p.name == playername)
                    {
                        if (p.GetComponent<SpriteRenderer>().color == player1Color)
                        {
                            chat.GetComponent<Text>().text = "<color=#" + player1ColorHex + ">" + playername + "</color>" + ": " + text;
                        }
                        if (p.GetComponent<SpriteRenderer>().color == player2Color)
                        {
                            chat.GetComponent<Text>().text = "<color=#" + player2ColorHex + ">" + playername + "</color>" + ": " + text;
                        }
                        if (p.GetComponent<SpriteRenderer>().color == player3Color)
                        {
                            chat.GetComponent<Text>().text = "<color=#" + player3ColorHex + ">" + playername + "</color>" + ": " + text;
                        }
                        if (p.GetComponent<SpriteRenderer>().color == player4Color)
                        {
                            chat.GetComponent<Text>().text = "<color=#" + player4ColorHex + ">" + playername + "</color>" + ": " + text;
                        }
                    }
                }
            }
            throw new Exception("Test exception");
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }

    void Update()
    {
        Cursor.visible = false;
        //move crossHair
        var screenPoint = (Vector3)(Input.mousePosition);
        screenPoint.z = 10.0f; //distance of the plane from the camera
        crossHair.position = Camera.main.ScreenToWorldPoint(screenPoint);
        //crossHair.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);// new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        //set connection text
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            connectionText.text = "<color=#FF0000FF>Disconnected</color>";
        }
        if (PhotonNetwork.connectionState == ConnectionState.Connecting)
        {
            connectionText.text = "<color=#FFED00FF>Trying to Connect...</color>";
        }
        if (PhotonNetwork.connectionState == ConnectionState.Connected)
        {
            connectionText.text = "Connected";
        }

        if (PhotonNetwork.connectionState == ConnectionState.Disconnected && reconnecting == false)
        {
            //so if we get Disconnected from the server we can Reconnect automatically
            InvokeRepeating("Reconnect", 1f, 1f);
            reconnecting = true;
        }
        //open menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) && !GameManager.instance.IsChatting)
        {
            IsChatting = false;
            chatInput.text = string.Empty;
            chatInput.gameObject.SetActive(false);
            controls.SetActive(false);
            credits.SetActive(false);
            menu.SetActive(!menu.activeSelf);
        }
        //only do this stuff if we're connected to a game/lobby
        if (!PhotonNetwork.inRoom) return;
        if (Input.GetMouseButtonDown(0))
        {
            IsChatting = false;
            chatInput.text = string.Empty;
            chatInput.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown("return"))
        {
            if (chatInput.gameObject.active)
            {
                IsChatting = false;
                if (chatInput.text != string.Empty)
                {
                    photonView.RPC("Chat", PhotonTargets.All, PhotonNetwork.player.NickName, chatInput.text);
                }
                chatInput.text = string.Empty;
                chatInput.gameObject.SetActive(false);
            }
            else
            {
                IsChatting = true;
                chatInput.gameObject.SetActive(true);
                chatInput.Select();
                chatInput.ActivateInputField();
            }
        }
        //buildIndex 0 is the menu buildIndex 1 is the lobby buildIndex 2+ = game maps
        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            //sort the players by id so the first player will be at the bottom and the last at the top of the list
            players.Sort(SortByID);
            playersFull.Sort(SortByID);

            //scoreboard open
            if (Input.GetKey(KeyCode.Tab) && roundStartText)
            {
                roundStartText.SetActive(true);
                if (gunGame)
                {
                    for (int i = 0; i < playersFull.Count;)
                    {
                        i++;
                        print(i);
                        //if the player exists
                        if (playersFull[i - 1] != null)
                        {
                            for (int i2 = 0; i2 < 4;)
                            {
                                i2++;
                                if (roundStartText.transform.FindAnyChild<Transform>("Player " + i2.ToString()).transform.GetComponent<Text>().text == playersFull[i - 1].transform.name)
                                {
                                    roundStartText.transform.FindAnyChild<Transform>("Player " + i2.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                                }
                            }
                        }
                    }
                }
            }
            else if (resetting != true && roundStartText)
            {
                roundStartText.SetActive(false);
            }
            //if we're the host

        }
    }
    public void PlayerJustDied()
    {
        Debug.LogWarning("PLAYERJUSTDIED");
        if (gunGame)
        {
            foreach (int score in playerScores)
            {
                if (score >= numberOfRounds)
                {
                    Invoke("EveryoneDiedDelay", 2.5f);
                }
            }
        }
        else if (!twovtwo)
        {
            if (!roundOver)
            {
                //this is for a draw
                if (playersDead > 0 && playersDead == players.Count && !roundOver)
                {
                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
                //this is for a win
                else if (playersDead > 0 && playersDead == players.Count - 1 && !roundOver)
                {
                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
            }
        }
        else
        {
            if (!roundOver)
            {
                int numofteam1players = 0;
                int numofteam2players = 0;

                foreach (Player p in players)
                {
                    if (p.id == 1)
                    {
                        numofteam1players++;
                    }
                    if (p.id == 2)
                    {
                        numofteam2players++;
                    }
                }
                Debug.LogWarning(numofteam1players.ToString());
                Debug.LogWarning(numofteam2players.ToString());

                Debug.LogWarning(playersDeadTeam1.ToString());
                Debug.LogWarning(playersDeadTeam2.ToString());


                //this is for a draw
                if ((playersDeadTeam1 > 1 || playersDeadTeam2 > 1) && playersDead == players.Count && !roundOver)
                {
                    Debug.LogWarning("FDAFKD");

                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
                //this is for a win
                else if ((playersDeadTeam1 == numofteam1players || playersDeadTeam2 == numofteam2players) && playersDead <= players.Count - 1 && !roundOver)
                {
                    Debug.LogWarning("FDAJLKF");

                    //delay 2.5 seconds before calling EveryoneDiedDelay so the scoreboards don't show up straight away

                    Invoke("EveryoneDiedDelay", 2.5f);
                    roundOver = true;
                }
            }
        }

    }
    void EveryoneDiedDelay()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Debug.LogWarning("everyone died delay");
            if (gunGame)
            {
                Debug.LogWarning("gun game call dead");

                photonView.RPC("EveryoneDead", PhotonTargets.All, null);
            }
            else
            {
                //this is for a draw
                if (playersDead > 0 && playersDead == players.Count)
                {
                    //EveryoneDead adds score enables scoreboards and waits 5 seconds before calling the nextround
                    photonView.RPC("EveryoneDeadDraw", PhotonTargets.All, null);
                    //so the round is increased when the next round is called
                    Invoke("IncreaseRoundDelay", 3f);
                }
                else
                {
                    //EveryoneDead adds score enables scoreboards and waits 5 seconds before calling the nextround
                    photonView.RPC("EveryoneDead", PhotonTargets.All, null);
                    //so the round is increased when the next round is called
                    Invoke("IncreaseRoundDelay", 3f);
                }
            }
        }
    }

    [PunRPC]
    void EveryoneDeadDraw()
    {
        try
        {
            resetting = true;
            Debug.LogWarning("OH SHIT WUDDUP");
            //this means the round was a draw
            if (playersDead >= players.Count)
            {
                //don't add to anyone's score enable to scoreboards
                roundStartText.gameObject.SetActive(true);
                roundWinnerGO.gameObject.SetActive(true);
                //call the next round in 5 seconds
                Invoke("NextRound", 3f);
                //put up the roundwinner text as a draw
                roundWinnerGO.transform.FindAnyChild<Transform>("Text").GetComponent<Text>().text = "Round " + round.ToString() + " is a " + "<color=#" + "DBDBDBFF" + ">" + "DRAW!" + "</color>";
            }
            //reset the amount of players dead
            playersDead = 0;
            playersDeadTeam1 = 0;
            playersDeadTeam2 = 0;
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    [PunRPC]
    void EveryoneDead()
    {
        try
        {
            resetting = true;
            //is the winner has a weapon throw it
            if (localPlayer.GetComponent<PlayerPickUp>().hasWeapon == true)
            {
                localPlayer.GetComponent<PlayerPickUp>().photonView.RPC("ThrowWeapon", PhotonTargets.All, true, localPlayer.transform.up);
                localPlayer.GetComponent<PlayerMovement>().alteredSpeed = 38;
            }
            //make sure they can't pick up another
            localPlayer.GetComponent<PlayerPickUp>().canPickUp = false;
            //loop through 0 to 4 players even if they're connected or not (playersfull is 4 player slots weather the player connected or not)

            for (int i = 0; i < playersFull.Count;)
            {
                i++;
                print(i);
                //if the player exists
                if (playersFull[i - 1] != null)
                {
                    if (twovtwo)
                    {
                        int teamThatWon = 0;
                        int teamThatLost = 0;

                        int numofteam1players = 0;
                        int numofteam2players = 0;

                        foreach (Player p in players)
                        {
                            if (p.id == 1)
                            {
                                numofteam1players++;
                            }
                            if (p.id == 2)
                            {
                                numofteam2players++;
                            }
                        }

                        if (playersDeadTeam1 != numofteam1players)
                        {
                            teamThatWon = 1;
                            teamThatLost = 2;
                        }
                        else
                        {
                            teamThatWon = 2;
                            teamThatLost = 1;
                        }

                        bool localplayerwon = false;

                        for (int winner = 0; winner < playersFull.Count; winner++)
                        {
                            if (playersFull[winner] != null)
                            {
                                if (playersFull[winner].photonView.isMine)
                                {
                                    if (playersFull[winner].id == teamThatWon)
                                    {
                                        localplayerwon = true;
                                    }
                                }
                            }
                        }

                        string bothWinnerPlayerNames = "";
                        string bothLoserPlayerNames = "";

                        if (teamThatWon == 1)
                        {
                            if (numofteam1players > 1)
                            {
                                bothWinnerPlayerNames = playersFull[0].transform.name + " + " + playersFull[1].transform.name;
                            }
                            else
                            {
                                if (playersFull[0] == null)
                                {
                                    bothWinnerPlayerNames = playersFull[1].transform.name;
                                }
                                else if (playersFull[1] == null)
                                {
                                    bothWinnerPlayerNames = playersFull[0].transform.name;
                                }
                            }

                            if (numofteam2players > 1)
                            {
                                bothLoserPlayerNames = playersFull[2].transform.name + " + " + playersFull[3].transform.name;
                            }
                            else
                            {
                                if (playersFull[2] == null)
                                {
                                    bothLoserPlayerNames = playersFull[3].transform.name;
                                }
                                else
                                {
                                    if (playersFull.Count == 4)
                                    {
                                        if (playersFull[3] == null)
                                        {
                                            bothLoserPlayerNames = playersFull[2].transform.name;
                                        }
                                    }
                                    else
                                    {
                                        bothLoserPlayerNames = playersFull[2].transform.name;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (numofteam2players > 1)
                            {
                                bothWinnerPlayerNames = playersFull[2].transform.name + " + " + playersFull[3].transform.name;
                            }
                            else
                            {
                                if (playersFull[2] == null)
                                {
                                    bothLoserPlayerNames = playersFull[3].transform.name;
                                }
                                else
                                {
                                    if (playersFull.Count == 4)
                                    {
                                        if (playersFull[3] == null)
                                        {
                                            bothWinnerPlayerNames = playersFull[2].transform.name;
                                        }
                                    }
                                    else
                                    {
                                        bothWinnerPlayerNames = playersFull[2].transform.name;
                                    }
                                }
                            }

                            if (numofteam1players > 1)
                            {
                                bothLoserPlayerNames = playersFull[0].transform.name + " + " + playersFull[1].transform.name;
                            }
                            else
                            {
                                if (playersFull[0] == null)
                                {
                                    bothLoserPlayerNames = playersFull[1].transform.name;
                                }
                                else if (playersFull[1] == null)
                                {
                                    bothLoserPlayerNames = playersFull[0].transform.name;
                                }
                            }
                        }


                        //if they're not dead, they they're the one who won
                        if (playersFull[i - 1].id == teamThatWon)
                        {
                            //add to their score

                            teamScores[teamThatWon - 1]++;

                            roundStartText.transform.FindAnyChild<Transform>("Player " + teamThatWon.ToString()).transform.GetComponent<Text>().text = bothWinnerPlayerNames;

                            roundStartText.transform.FindAnyChild<Transform>("Player " + teamThatWon.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = teamScores[teamThatWon - 1].ToString() + "/" + numberOfRounds.ToString();

                            if (teamThatWon == 1)
                            {
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 1.ToString()).transform.GetComponent<Text>().text = bothWinnerPlayerNames;
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 1.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = teamScores[teamThatWon - 1].ToString() + "/" + numberOfRounds.ToString();

                                roundStartText.transform.FindAnyChild<Transform>("Player " + 2.ToString()).transform.GetComponent<Text>().text = bothLoserPlayerNames;
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 2.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = teamScores[teamThatLost - 1].ToString() + "/" + numberOfRounds.ToString();
                            }
                            else
                            {
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 1.ToString()).transform.GetComponent<Text>().text = bothLoserPlayerNames;
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 1.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = teamScores[teamThatWon - 1].ToString() + "/" + numberOfRounds.ToString();

                                roundStartText.transform.FindAnyChild<Transform>("Player " + 2.ToString()).transform.GetComponent<Text>().text = bothWinnerPlayerNames;
                                roundStartText.transform.FindAnyChild<Transform>("Player " + 2.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = teamScores[teamThatLost - 1].ToString() + "/" + numberOfRounds.ToString();

                            }

                            //if their score is less then the max number of rounds (they won the round but not whole game)
                            if (teamScores[teamThatWon - 1] < numberOfRounds)
                            {
                                Debug.Log("Next round");
                                //trun on the leaderboard
                                roundStartText.gameObject.SetActive(true);
                                //turn on the winner text
                                roundWinnerGO.gameObject.SetActive(true);

                                //call the next round in 5 seconds
                                Invoke("NextRound", 3f);

                            }
                            //else they won the round and the whole game
                            else
                            {
                                //set the win leaderboard to active (not the round winner leaderboard)
                                win.SetActive(true);
                                //get the winner's color
                                Color brighterColor1 = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                                //do some converting from rgb to hsv
                                float vh1, vs1, vv1;
                                Color.RGBToHSV(brighterColor1, out vh1, out vs1, out vv1);
                                brighterColor1 = Color.HSVToRGB(vh1, vs1, 1);
                                //win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player " + i.ToString()).gameObject.SetActive(true);

                                //set the win text and colors

                                if (!localplayerwon)
                                {
                                    win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + bothWinnerPlayerNames + "</color>" + " <color=#FFD800FF>WINS!</color>" + " " + teamScores[teamThatWon - 1].ToString() + "/" + numberOfRounds.ToString();
                                }
                                else
                                {
                                    win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + "YOU" + "</color>" + " <color=#FFD800FF>WIN!</color>" + " " + teamScores[teamThatWon - 1].ToString() + "/" + numberOfRounds.ToString();
                                    winAU.PlayOneShot(winMusic, 1f);

                                    AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
                                    foreach (AudioSource aud in audios)
                                    {
                                        if (aud.name == "MusicManager")
                                        {
                                            aud.gameObject.SetActive(false);
                                        }
                                    }

                                    Time.timeScale = .5f;
                                }
                                //set the loser text and colors
                                for (int y = 0; y < 2;)
                                {
                                    y++;
                                    if (y != i)
                                    {
                                        win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString() + " Loser Panel").gameObject.SetActive(true);
                                        win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString()).transform.Find("Score").GetComponent<Text>().text = teamScores[teamThatLost - 1] + "/" + numberOfRounds.ToString();
                                    }
                                }
                                if (localplayerwon)
                                {
                                    /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                    {
                                        KongregateAPIBehaviour.instance.Win();
                                    }*/
                                    Steamworks.SteamUserStats.AddStat("WINS", 1);
                                    Steamworks.SteamUserStats.StoreStats();
                                    //end the game after 10 seconds
                                    Invoke("EndGame", 5f);
                                    ending = true;
                                }
                                else
                                {
                                    /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                    {
                                        KongregateAPIBehaviour.instance.Loses();
                                    }*/
                                    Steamworks.SteamUserStats.AddStat("LOSSES", 1);
                                    Steamworks.SteamUserStats.StoreStats();
                                    //end the game after 10 seconds
                                    Invoke("EndGame", 10f);
                                    ending = true;
                                }


                            }
                            //do the same converting again
                            Color brighterColor = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                            float vh, vs, vv;
                            Color.RGBToHSV(brighterColor, out vh, out vs, out vv);
                            brighterColor = Color.HSVToRGB(vh, vs, 1);
                            //set the color and player name of winner of the round
                            roundWinnerGO.transform.FindAnyChild<Transform>("Text").GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor) + ">" + playersFull[i - 1].transform.name + "</color>" + " <color=#FFD800FF>Wins</color> Round " + round.ToString();
                        }
                    }
                    else if (!gunGame)
                    {
                        //if they're not dead, they they're the one who won
                        if (!playersFull[i - 1].GetComponent<PlayerHealth>().isDead)
                        {
                            //add to their score
                            playerScores[i - 1]++;
                            for (int i2 = 0; i2 < 4;)
                            {
                                i2++;
                                if (roundStartText.transform.FindAnyChild<Transform>("Player " + i2.ToString()).transform.GetComponent<Text>().text == playersFull[i - 1].transform.name)
                                {
                                    roundStartText.transform.FindAnyChild<Transform>("Player " + i2.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                                }
                            }


                            //if their score is less then the max number of rounds (they won the round but not whole game)
                            if (playerScores[i - 1] < numberOfRounds)
                            {
                                Debug.Log("Next round");
                                //trun on the leaderboard
                                roundStartText.gameObject.SetActive(true);
                                //turn on the winner text
                                roundWinnerGO.gameObject.SetActive(true);

                                //call the next round in 5 seconds
                                Invoke("NextRound", 3f);

                            }
                            //else they won the round and the whole game
                            else
                            {
                                //set the win leaderboard to active (not the round winner leaderboard)
                                win.SetActive(true);
                                //get the winner's color
                                Color brighterColor1 = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                                //do some converting from rgb to hsv
                                float vh1, vs1, vv1;
                                Color.RGBToHSV(brighterColor1, out vh1, out vs1, out vv1);
                                brighterColor1 = Color.HSVToRGB(vh1, vs1, 1);
                                //win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player " + i.ToString()).gameObject.SetActive(true);

                                //set the win text and colors
                                win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + playersFull[i - 1].transform.name + "</color>" + " <color=#FFD800FF>WINS!</color>" + " " + playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                                if (playersFull[i - 1].photonView.isMine)
                                {
                                    win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + "YOU" + "</color>" + " <color=#FFD800FF>WIN!</color>" + " " + playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                                    winAU.PlayOneShot(winMusic, 1f);

                                    AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
                                    foreach (AudioSource aud in audios)
                                    {
                                        if (aud.name == "MusicManager")
                                        {
                                            aud.gameObject.SetActive(false);
                                        }
                                    }

                                    Time.timeScale = .5f;
                                }
                                //set the loser text and colors
                                for (int y = 0; y < playersFull.Count;)
                                {
                                    y++;
                                    if (y != i)
                                    {
                                        win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString() + " Loser Panel").gameObject.SetActive(true);
                                        win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString()).transform.Find("Score").GetComponent<Text>().text = playerScores[y - 1] + "/" + numberOfRounds.ToString();

                                    }
                                }
                                if (playersFull[i - 1].photonView.isMine)
                                {
                                    /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                    {
                                        KongregateAPIBehaviour.instance.Win();
                                    }*/
                                    Steamworks.SteamUserStats.AddStat("WINS", 1);
                                    Steamworks.SteamUserStats.StoreStats();
                                    //end the game after 10 seconds
                                    Invoke("EndGame", 5f);
                                    ending = true;
                                }
                                else
                                {
                                    /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                    {
                                        KongregateAPIBehaviour.instance.Loses();
                                    }*/
                                    Steamworks.SteamUserStats.AddStat("LOSSES", 1);
                                    Steamworks.SteamUserStats.StoreStats();
                                    //end the game after 10 seconds
                                    Invoke("EndGame", 10f);
                                    ending = true;
                                }


                            }
                            //do the same converting again
                            Color brighterColor = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                            float vh, vs, vv;
                            Color.RGBToHSV(brighterColor, out vh, out vs, out vv);
                            brighterColor = Color.HSVToRGB(vh, vs, 1);
                            //set the color and player name of winner of the round
                            roundWinnerGO.transform.FindAnyChild<Transform>("Text").GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor) + ">" + playersFull[i - 1].transform.name + "</color>" + " <color=#FFD800FF>Wins</color> Round " + round.ToString();
                        }
                    }
                    else
                    {
                        int playerWhoWonID = 0;
                        for (int scoreid = 0; scoreid < playerScores.Length; scoreid++)
                        {
                            if (playerScores[scoreid] == numberOfRounds)
                            {
                                playerWhoWonID = scoreid;
                            }
                        }
                        //if they're not dead, they they're the one who won
                        if (i - 1 == playerWhoWonID)
                        {

                            //set the win leaderboard to active (not the round winner leaderboard)
                            win.SetActive(true);
                            //get the winner's color
                            Color brighterColor1 = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                            //do some converting from rgb to hsv
                            float vh1, vs1, vv1;
                            Color.RGBToHSV(brighterColor1, out vh1, out vs1, out vv1);
                            brighterColor1 = Color.HSVToRGB(vh1, vs1, 1);
                            //win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player " + i.ToString()).gameObject.SetActive(true);

                            //set the win text and colors
                            win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + playersFull[i - 1].transform.name + "</color>" + " <color=#FFD800FF>WINS!</color>" + " " + playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                            if (playersFull[i - 1].photonView.isMine)
                            {
                                win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor1) + ">" + "YOU" + "</color>" + " <color=#FFD800FF>WIN!</color>" + " " + playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
                                winAU.PlayOneShot(winMusic, 1f);

                                AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
                                foreach (AudioSource aud in audios)
                                {
                                    if (aud.name == "MusicManager")
                                    {
                                        aud.gameObject.SetActive(false);
                                    }
                                }

                                Time.timeScale = .5f;
                            }
                            //set the loser text and colors
                            for (int y = 0; y < playersFull.Count;)
                            {
                                y++;
                                if (y != i)
                                {
                                    win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString() + " Loser Panel").gameObject.SetActive(true);
                                    win.transform.FindAnyChild<Transform>("Losers").transform.FindAnyChild<Transform>("Player " + y.ToString()).transform.Find("Score").GetComponent<Text>().text = playerScores[y - 1] + "/" + numberOfRounds.ToString();

                                }
                            }
                            if (playersFull[i - 1].photonView.isMine)
                            {
                                /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                {
                                    KongregateAPIBehaviour.instance.Win();
                                }*/
                                Steamworks.SteamUserStats.AddStat("WINS", 1);
                                Steamworks.SteamUserStats.StoreStats();
                                //end the game after 10 seconds
                                Invoke("EndGame", 5f);
                                ending = true;
                            }
                            else
                            {
                                /*if (!KongregateAPIBehaviour.instance.IsGuest)
                                {
                                    KongregateAPIBehaviour.instance.Loses();
                                }*/
                                Steamworks.SteamUserStats.AddStat("LOSSES", 1);
                                Steamworks.SteamUserStats.StoreStats();
                                //end the game after 10 seconds
                                Invoke("EndGame", 10f);
                                ending = true;



                            }
                            //do the same converting again
                            Color brighterColor = playersFull[i - 1].GetComponent<SpriteRenderer>().color;
                            float vh, vs, vv;
                            Color.RGBToHSV(brighterColor, out vh, out vs, out vv);
                            brighterColor = Color.HSVToRGB(vh, vs, 1);
                            //set the color and player name of winner of the round
                            roundWinnerGO.transform.FindAnyChild<Transform>("Text").GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(brighterColor) + ">" + playersFull[i - 1].transform.name + "</color>" + " <color=#FFD800FF>Wins</color> Round " + round.ToString();
                        }
                    }
                }
                //update the score text
            }
            //reset the amount of players dead
            playersDead = 0;
            playersDeadTeam1 = 0;
            playersDeadTeam2 = 0;
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    //this takes 5 seconds o get called
    void IncreaseRoundDelay()
    {
        //increases the round
        photonView.RPC("IncreaseRound", PhotonTargets.All, null);
    }
    //called on all players
    [PunRPC]
    void IncreaseRound()
    {
        try
        {
            //add to the number of rounds played
            round++;
            //set the round text
            roundStartText.transform.Find("Text").GetComponent<Text>().text = "Round " + round.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    [PunRPC]
    void NextRoundRPC()
    {
        try
        {
            // Cache result of expensive calls
            Door[] doors = GameObject.FindObjectsOfType<Door>();
            WeaponSpawnPoint[] weaponSpawnPoints = FindObjectsOfType(typeof(WeaponSpawnPoint)) as WeaponSpawnPoint[];
            GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");
            GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");

            foreach (GameObject go in gores)
            {
                Destroy(go);
            }
            gores.Clear();

            foreach (Door doorgo in doors)
            {
                doorgo.health = doorgo.maxHealth;
                doorgo.transform.Find("Door").GetComponent<SpriteRenderer>().sprite = doorgo.doorHealthSprites[0];
                doorgo.GetComponent<Collider2D>().enabled = true;
            }

            if (!startingNextRound)
            {
                startingNextRound = true;
                print("NextRound");
                localPlayer.GetComponent<PlayerMovement>().canMove = false;

                if (PhotonNetwork.isMasterClient)
                {
                    foreach (GameObject go in pickUps)
                    {
                        go.GetComponent<WeaponPickUp>().photonView.RPC("Destroy", PhotonTargets.All, null);
                    }

                    foreach (WeaponSpawnPoint wepSP in weaponSpawnPoints)
                    {
                        int weaponsToSpawn = Random.Range(1, 3);
                        for (int i = 0; i < weaponsToSpawn; i++)
                        {
                            PhotonNetwork.InstantiateSceneObject(weapons[Random.Range(0, weapons.Count)].name, new Vector3(Random.Range(wepSP.minX, wepSP.maxX), Random.Range(wepSP.minY, wepSP.maxY), 0), Quaternion.identity, 0, null);
                            SteamInventory.instance.GetMyItemsInGamePuBLIC();
                        }
                    }
                }

                foreach (Door d in doors)
                {
                    d.toRot = d.normalRot;
                }

                countDown = 4;
                if (PhotonNetwork.isMasterClient)
                {
                    print("Countdown");
                    StartCoroutine("CountDown");
                }

                roundStartText.gameObject.SetActive(false);
                roundWinnerGO.gameObject.SetActive(false);

                resetting = false;
                foreach (Player p in players)
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        p.GetComponent<PlayerHealth>().photonView.RPC("Reset", PhotonTargets.All, null);
                    }
                    p.transform.position = p.GetComponent<Player>().spawnPos;
                    p.transform.rotation = p.GetComponent<Player>().spawnRot;
                }

                foreach (GameObject go in bodies)
                {
                    Destroy(go);
                }

                roundOver = false;
                playersDead = 0;
                playersDeadTeam1 = 0;
                playersDeadTeam2 = 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    public void Eu()
    {
        PhotonNetwork.Disconnect();
        eu.interactable = false;
        us.interactable = false;
        asia.interactable = false;
        sa.interactable = false;
        connectingToRegionEu = true;
    }
    public void Us()
    {
        PhotonNetwork.Disconnect();
        eu.interactable = false;
        us.interactable = false;
        asia.interactable = false;
        sa.interactable = false;
        connectingToRegionUs = true;
    }
    public void Asia()
    {
        PhotonNetwork.Disconnect();
        eu.interactable = false;
        us.interactable = false;
        asia.interactable = false;
        sa.interactable = false;
        connectingToRegionAsia = true;
    }
    public void SA()
    {
        PhotonNetwork.Disconnect();
        eu.interactable = false;
        us.interactable = false;
        asia.interactable = false;
        sa.interactable = false;
        connectingToRegionSA = true;
    }
    [PunRPC]
    void NextRoundThing()
    {
        try
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (!startingNextRound)
                {
                    photonView.RPC("NextRoundRPC", PhotonTargets.All, null);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    //called when everyone has died
    void NextRound()
    {
        photonView.RPC("NextRoundThing", PhotonTargets.MasterClient, null);
    }
    //if we get disconnected
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            GameObject killFeed = Instantiate(GameManager.instance.killFeed, Vector3.zero, Quaternion.identity) as GameObject;
            killFeed.transform.SetParent(GameObject.Find("MainCanvas").transform.Find("Kill Feed Canvas").transform, false);
            killFeed.transform.FindAnyChild<Transform>("victim").GetComponent<Text>().text = "Disconnected";
            killFeed.transform.FindAnyChild<Transform>("killer").GetComponent<Text>().text = otherPlayer.NickName;


            killFeed.transform.FindAnyChild<Transform>("weapon").GetComponent<Image>().sprite = disconnectedSprite;

            //reset the player lists
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == otherPlayer.NickName)
                {
                    if (players[i].GetComponent<PlayerHealth>().isDead)
                    {
                        playersDead--;
                        if (twovtwo)
                        {
                            if (players[i].id == 1)
                            {
                                playersDeadTeam1--;
                            }
                            if (players[i].id == 2)
                            {
                                playersDeadTeam2--;
                            }
                        }
                    }

                    players.Remove(players[i]);
                }
            }
            for (int i = 0; i < actorSortedId.Count; i++)
            {
                if (otherPlayer.ID == actorSortedId[i])
                {
                    actorSortedId.Remove(actorSortedId[i]);
                }
            }
            if (players.Count == 1)
            {
                if (!ending)
                {
                    //set the win leaderboard to active (not the round winner leaderboard)
                    win.SetActive(true);
                    //get the winner's color
                    Color brighterColor1 = players[0].GetComponent<SpriteRenderer>().color;
                    //do some converting from rgb to hsv
                    float vh1, vs1, vv1;
                    Color.RGBToHSV(brighterColor1, out vh1, out vs1, out vv1);
                    brighterColor1 = Color.HSVToRGB(vh1, vs1, 1);
                    //win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player " + i.ToString()).gameObject.SetActive(true);

                    //set the win text and colors
                    win.transform.FindAnyChild<Transform>("Winners").transform.FindAnyChild<Transform>("Player Winner").transform.GetComponent<Text>().text = "everyone disconnected";

                    //end the game after 10 seconds
                    Invoke("EndGame", 10f);
                }
            }

        }
    }
    //don't mind this
    void OnDisable()
    {
        //PlayerPrefs.DeleteAll();
    }
    void OnApplicationQuit()
    {
        if (Steamworks.SteamClient.IsValid)
        {
            int myKills = Steamworks.SteamUserStats.GetStatInt("KILLS");
            int myDeaths = Steamworks.SteamUserStats.GetStatInt("DEATHS");
            Debug.Log(myDeaths);
            int myWins = Steamworks.SteamUserStats.GetStatInt("WINS");
            int myLosses = Steamworks.SteamUserStats.GetStatInt("LOSSES");
            UpdateLeaderboards("Losses", myLosses);
            UpdateLeaderboards("Wins", myWins);
            UpdateLeaderboards("Kills12", myKills);
            UpdateLeaderboards("Deaths", myDeaths);
        }
    }
    //called when the game is finished
    public void EndGame()
    {
        ending = false;
        AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource aud in audios)
        {
            if (aud.name == "MusicManager")
            {
                aud.gameObject.SetActive(true);
            }
        }
        Time.timeScale = 1f;
        exitMatchButton.interactable = false;
        //clear stuff up and leave the room
        //PhotonNetwork.LeaveRoom();
        localPlayer = null;
        players.Clear();
        spawnPoints.Clear();
        playersDead = 0;
        playersDeadTeam1 = 0;
        playersDeadTeam2 = 0;
        actorSortedId.Clear();
        playersFull.Clear();
        playerNums.Clear();
        roundOver = false;
        round = 1;
        resetting = false;
        for (int i = 0; i < playerScores.Length; i++)
        {
            playerScores[i] = 0;
        }
        teamScores[0] = 0;
        teamScores[1] = 0;
        countDown = 4;
        GameObject.Find("HUD").transform.FindAnyChild<Transform>("AMMO AND SHIELD").gameObject.SetActive(false);
        numberOfRounds = 5;
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.room.IsVisible = visible;
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.LoadLevel(1);
        }
    }
    //countdown
    IEnumerator CountDown()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            photonView.RPC("PunCountDown", PhotonTargets.AllBufferedViaServer);
        }

        foreach (Player p in players)
        {
            p.GetComponent<PlayerMovement>().photonView.RPC("ResetCanMove", PhotonTargets.All, null);
        }

        photonView.RPC("CountDownFinished", PhotonTargets.AllBufferedViaServer, null);
    }
    [PunRPC]
    void CountDownFinished()
    {
        try
        {
            if (localPlayer && countDowntext)
            {
                //player can move and PickUp
                localPlayer.GetComponent<PlayerMovement>().canMove = true;
                localPlayer.GetComponent<PlayerPickUp>().canPickUp = true;
                //disable countdown
                countDowntext.transform.parent.gameObject.SetActive(false);
                //stop counting down
                StopCoroutine("CountDown");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    [PunRPC]
    void PunCountDown()
    {
        try
        {
            if (countDowntext)
            {
                //countdown minus 1
                countDown--;
                if (countDown > 0)
                {
                    countDowntext.transform.parent.gameObject.SetActive(true);
                }
                if (countDown <= 0)
                {
                    startingNextRound = false;
                    countDowntext.text = "Round Started";
                }
                else
                {
                    countDowntext.text = "Round starts in " + countDown.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    void OnLevelWasLoaded(int levelNumber)
    {
        GameObject.Find("HUD").GetComponent<Canvas>().worldCamera = Camera.main;
        SteamInventory.instance.GetItems();
        Time.timeScale = 1f;
        ending = false;
        if (levelNumber == 0)
        {
            numberOfRounds = 5;
        }
        if (soundOn == false)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name != "MusicManager")
                {
                    aud.mute = true;
                }
            }
        }
        if (musicOn == false)
        {
            AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource aud in audios)
            {
                if (aud.name == "MusicManager")
                {
                    aud.mute = true;
                    aud.gameObject.SetActive(true);
                }
            }
        }
        if (levelNumber == 0 || levelNumber == 1)
        {
            crossHair.transform.localScale = new Vector3(1, 1, 1);
            lobbyMenuMusic.SetActive(true);
        }
        //if we're in a room/lobby
        if (!PhotonNetwork.inRoom) return;

        if (levelNumber == 1)
        {
            GameObject.Find("Canvas").transform.Find("Settings Panel (1)").transform.Find("GameCode").transform.GetComponent<Text>().text = "code: " + PhotonNetwork.room.Name;
        }

        //if we loaded a map
        if (levelNumber >= 2)
        {
            GameObject.Find("HUD").transform.FindAnyChild<Transform>("AMMO AND SHIELD").gameObject.SetActive(true);
            lobbyMenuMusic.SetActive(false);
            //sort the players ids
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                if (!playerNums.Contains(p.ID))
                {
                    playerNums.Add(p.ID);
                }
            }
            playerNums.Sort();
            //go through the players ids
            for (int i = 0; i < playerNums.Count; i++)
            {
                //if that id is the same as our id
                if (playerNums[i] == PhotonNetwork.player.ID)
                {
                    //set the player num to the num of that id in the list so we can choose a spawn point
                    playerNum = i;
                }
            }
            //set the countdown text
            countDowntext = GameObject.Find("Countdown").transform.Find("Text").GetComponent<Text>();
            countDown = 4;
            //start the countdown for the first time, since we just joined a game
            if (PhotonNetwork.isMasterClient)
            {
                StartCoroutine("CountDown");
            }
            //add all the spoint points to the list of spawnpoints
            spawnPoints.Add(GameObject.Find("SpawnPoint1").transform);
            spawnPoints.Add(GameObject.Find("SpawnPoint2").transform);
            spawnPoints.Add(GameObject.Find("SpawnPoint3").transform);
            spawnPoints.Add(GameObject.Find("SpawnPoint4").transform);

            //set the other ui obects like the score board win screen etc
            roundStartText = GameObject.Find("Round Start");
            roundWinnerGO = GameObject.Find("Round Winner");

            roundStart2V2Text = GameObject.Find("2v2 Round Start");
            twovtwoRoundWinnerGO = GameObject.Find("2v2 Round Winner");

            eliminated = GameObject.Find("Eliminated").GetComponent<Text>();
            win = GameObject.Find("Win");
            //then disable them
            win.SetActive(false);
            roundStartText.SetActive(false);
            roundWinnerGO.SetActive(false);
            roundStart2V2Text.SetActive(false);
            twovtwoRoundWinnerGO.SetActive(false);
            eliminated.gameObject.SetActive(false);
            exitMatchButton.interactable = true;
            //get a spawn point, that's this players using the id and player num from above
            var spawnPoint = spawnPoints[playerNum];

            int teamNumber = playerNum / 2;

            if (twovtwo)
            {
                roundStartText = roundStart2V2Text;
                roundWinnerGO = twovtwoRoundWinnerGO;
                spawnPoint = spawnPoints[teamNumber];
            }
            //spawn the player on all computers and set it to the localplayer
            localPlayer = PhotonNetwork.Instantiate(
                "Player",
                spawnPoint.position,
                spawnPoint.rotation, 0);

            //set the player's spawn point
            localPlayer.GetComponent<Player>().spawnPoint = spawnPoint;

            photonView.RPC("SyncHat", PhotonTargets.All, localPlayer.GetPhotonView().viewID, hat);
            photonView.RPC("SyncBackpack", PhotonTargets.All, localPlayer.GetPhotonView().viewID, backpack);
            foreach (int iddd in defids)
            {
                photonView.RPC("SyncWeaponSkin", PhotonTargets.All, localPlayer.GetPhotonView().viewID, iddd);
            }

            //add the player to the player list on all other players
            photonView.RPC("PlayerAdd", PhotonTargets.AllBuffered, localPlayer.GetComponent<PhotonView>().viewID, localPlayer.GetComponent<PhotonView>().ownerId);

            //set the player names in the scoreboard
            for (int i = 0; i < 4;)
            {
                i++;
                roundStartText.transform.FindAnyChild<Transform>("Player " + i.ToString()).transform.FindAnyChild<Transform>("Score").GetComponent<Text>().text = playerScores[i - 1].ToString() + "/" + numberOfRounds.ToString();
            }
            //if we're the host
            if (gunGame)
            {
                numberOfRounds = 9;
            }
            if (PhotonNetwork.isMasterClient && !gunGame)
            {
                //spawn the first weapons
                var weaponSpawnPoints = FindObjectsOfType(typeof(WeaponSpawnPoint)) as WeaponSpawnPoint[];
                foreach (WeaponSpawnPoint wepSP in weaponSpawnPoints)
                {
                    int weaponsToSpawn = Random.Range(1, 3);
                    for (int i = 0; i < weaponsToSpawn; i++)
                    {
                        PhotonNetwork.InstantiateSceneObject(weapons[Random.Range(0, weapons.Count)].name, new Vector3(Random.Range(wepSP.minX, wepSP.maxX), Random.Range(wepSP.minY, wepSP.maxY), 0), Quaternion.identity, 0, null);
                        SteamInventory.instance.GetMyItemsInGamePuBLIC();
                    }
                    //PhotonNetwork.InstantiateSceneObject(go.name, new Vector3(Random.Range(-4.34f, 4.34f), Random.Range(-9.56f, -6.23f), 0), Quaternion.identity, 0, null);
                }
            }
            if (soundOn == false)
            {
                AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
                foreach (AudioSource aud in audios)
                {
                    if (aud.name != "MusicManager")
                    {
                        aud.mute = true;
                    }
                }
            }
            if (musicOn == false)
            {
                AudioSource[] audios = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
                foreach (AudioSource aud in audios)
                {
                    if (aud.name == "MusicManager")
                    {
                        aud.mute = true;
                    }
                }
            }
        }
        else
        {
            playerNum = 0;
        }

    }
    //set the players name on all players
    [PunRPC]
    void SetName(string name, int id)
    {
        try
        {
            PhotonView.Find(id).gameObject.name = name;

        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    //another help with sorting
    static int SortByID(Player p1, Player p2)
    {
        return p1.id.CompareTo(p2.id);
    }
    //adding the player to the player list on all players
    [PunRPC]
    void PlayerAdd(int id, int id2)
    {
        try
        {
            Debug.LogWarning("added player");
            players.Add(PhotonView.Find(id).gameObject.GetComponent<Player>());
            players.Sort(SortByID);
            PhotonView.Find(id).gameObject.name = PhotonPlayer.Find(id2).NickName;
            //if we're the host
            if (PhotonNetwork.isMasterClient)
            {
                //call player full in 2 seconds
                Invoke("PlayerFull", 2f);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    void PlayerFull()
    {
        //call playerfullrpc on all players
        photonView.RPC("PlayerFullRPC", PhotonTargets.All, null);
    }
    [PunRPC]
    void PlayerFullRPC()
    {
        try
        {
            //add the player to the slots that the players take up in the playersfull list
            if (playersFull.Count == 0)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    playersFull.Add(players[i]);
                }
            }
            playersFull.Sort(SortByID);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }

    }
    //this isn't used anywhere so ignore it
    Transform GetRandomSpawnPoint()
    {
        var spawnPoints_ = spawnPoints;
        //playerNum++;
        //playerNum = players.Count;
        playerNum = PhotonNetwork.player.ID - 1;
        print(playerNum);

        if (!PhotonNetwork.isMasterClient)
        {
            playerNum = PhotonNetwork.playerList.Length - 1;

        }
        if (spawnPoints_.Count == 0)
        {
            return defaultSpawnPoint.transform;
        }
        else
        {
            return spawnPoints_[playerNum].transform;

        }

    }
    //this is just some help too
    public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject))
                    as GameObject[])
        {
            if (go.hideFlags == HideFlags.NotEditable ||
                go.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (go.GetComponent<T>() != null)
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
    [PunRPC]
    void SyncHat(int Id, int hatnum)
    {
        try
        {
            PhotonView.Find(Id).gameObject.GetComponent<Player>().SyncHat(hatnum);

        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    [PunRPC]
    void SyncBackpack(int Id, int backpacknum)
    {
        try
        {
            PhotonView.Find(Id).gameObject.GetComponent<Player>().SyncBackpack(backpacknum);

        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
    public void SyncWeaponSkinsFromInventory(int defID)
    {
        photonView.RPC("SyncWeaponSkin", PhotonTargets.All, localPlayer.GetPhotonView().viewID, defID);
    }
    [PunRPC]
    void SyncWeaponSkin(int Id, int DefIds)
    {
        try
        {
            PhotonView.Find(Id).gameObject.GetComponent<Player>().SyncWeaponSkin(DefIds);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred in SyncWeaponSkin: " + e.Message);
            Debug.LogError(e.StackTrace);
        }
    }

}

//ayy sam
//bich
//bich
//bich
//bich
//bich
//bich
//gimme my background back, muffin
