using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : TagModularity
{

    public enum GameState
    {
        defaultState,
        setupEnvironmentLoad,
        controllerModeSelect,
        armCalibration,
        boardCalibration,
        boardPlacement,
        gamemodeSelect,
        environmentLoad,
        songSelect,
        gamePlay,
        oldDemo,
        gamePaused,
        gameOver

    }


    [Header("Current State of Game")]
    public GameState StateOfGame;

    [System.Serializable]
    public class GameStateScene
    {
        public string SceneName;
        public GameState AssociatedState;
        

        public bool Loading;
        public bool Loaded;

        public string CallOnLoaded;
        public bool SetActiveOnLoaded;
        public bool UnloadOnStateChange;

        public UnityEvent callfunction;

        [Header("States before and after")]
        public GameState NextGameState;
        public GameState LastGameState;


    }

    [Header("Game State Scenes To Load")]
    public GameStateScene[] gameStateScene;

    Dictionary<GameState, GameStateScene> GameStateDict = new Dictionary<GameState, GameStateScene>();

    public TextMesh GameStateText;

    public bool isBoardTracked;

    public bool isBoardPlaced;

    public bool isArmCalibrated;

    private bool controllerModeSelected;

    public bool UsingWristStraps;

    public Vector3 BoardPosition;

    public Vector3 BoardForward;

    [System.Serializable]
    public class MyEvent : UnityEvent<bool> { }
    public MyEvent myEvent;

    public AudioClip SongAudioToPlay;

    public TextAsset SongJsonToPlay;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameStateScene gss in gameStateScene)
        {
            GameStateDict.Add(gss.AssociatedState, gss);

        }
        NextState();
        /*
        gameStateScene[1].callfunction.AddListener(() => Debug.Log("Called this"));//give function, not return value

        gameStateScene[1].callfunction?.Invoke();//check if null

        int.MaxValue = 2^32;
        System.Int32;

        long num;
        System.Int64;

        if (thing != null & thing.property1 == whatever)
        {
            thing.property2()
        }

        pointerA->property

            int i = (int)GameState.boardPlacement;*/
    }

    public void SetBoardPosition(Vector3 bPos, Vector3 bFwd)
    {
        isBoardTracked = true;

        BoardPosition = bPos;

        BoardForward = bFwd;

    }

    public void ApplyBoardPosition()
    {
        GameObject gameBoard = FindTaggedObject("BoardObj");

        //GameplayController gameplayCont = FindTaggedObject("GameplayCont").GetComponent<GameplayController>();
        
        //gameBoard.transform.position = BoardPosition /*+ BoardForward*0.016f*/;

        //gameBoard.transform.forward = BoardForward;

        //gameplayCont.StartGame(SongJsonToPlay, SongAudioToPlay, BoardPosition, BoardForward);

        Debug.Log("Applied Board Position");
    }

    /*public void NextState()
    {
        do
        {
            if (StateOfGame == GameState.armCalibration && !isArmCalibrated) { Debug.Log("Must Calibrate Arm"); break; }

            if (StateOfGame == GameState.boardCalibration && !isBoardTracked) { Debug.Log("Must Calibrate Board"); break; }

            if (StateOfGame == GameState.controllerModeSelect && !controllerModeSelected) { Debug.Log("Must Select Controller Mode"); break; }

            StateOfGame++;

        } while (false);

        LoadSceneFromGameState(StateOfGame);
        
        foreach (GameStateScene gss in gameStateScene)
        {
            if (StateOfGame == gss.AssociatedState)
                continue;

            if (gss.UnloadOnStateChange && gss.SceneName != "" && gss.Loaded)
            {
                Debug.Log("Unloading scene " + gss.SceneName);

                SceneManager.UnloadSceneAsync(gss.SceneName);
                gss.Loaded = false;
            }

        }

        UnloadOldStates();
    }*/

    public void NextState()
    {
        GoToState(StateOfGame + 1);

    }

    public void GoToState(GameState newState)
    {
        if (newState > StateOfGame)
        {
            do
            {
                if (StateOfGame == GameState.armCalibration && !isArmCalibrated) { Debug.Log("Must Calibrate Arm"); break; }

                if (StateOfGame == GameState.boardCalibration && !isBoardTracked) { Debug.Log("Must Calibrate Board"); break; }

                if (StateOfGame == GameState.boardPlacement && !isBoardPlaced) { Debug.Log("Must Place Board"); break; }

                if (StateOfGame == GameState.controllerModeSelect && !controllerModeSelected) { Debug.Log("Must SelectControllerMode"); break; }

                StateOfGame = newState;

            } while (false);

        }
        else if (newState < StateOfGame)
        {

            StateOfGame = newState;
        }

        LoadSceneFromGameState(StateOfGame);

        UnloadOldStates();
    }

    public void LastState()
    {
        GoToState(StateOfGame - 1);

    }

    void UnloadOldStates()
    {
        foreach (GameStateScene gss in gameStateScene)
        {
            if (StateOfGame == gss.AssociatedState)
                continue;

            if (gss.UnloadOnStateChange && gss.SceneName != "" && gss.Loaded)
            {
                Debug.Log("Unloading scene " + gss.SceneName);

                SceneManager.UnloadSceneAsync(gss.SceneName);
                gss.Loaded = false;
            }

        }
    }

    private void Update()
    {
        /*
        if (OVRInput.GetDown(OVRInput.RawButton.Start) || Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(LoadGameStateScene(GameStateDict[GameState.oldDemo]));

        }
        */
        if (OVRInput.GetDown(OVRInput.RawButton.Start) || Input.GetKeyDown(KeyCode.M))
        {
            GoToState(GameState.environmentLoad);

        }

        if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.A))
        {
            if (StateOfGame < GameState.environmentLoad)
            {
                if (StateOfGame == GameState.controllerModeSelect)
                {
                    if (UsingWristStraps)
                    {
                        GoToState(GameState.armCalibration);

                    }
                    else
                    {
                        GoToState(GameState.boardPlacement);

                    }

                }
                else
                {
                    GoToState(GameStateDict[StateOfGame].NextGameState);

                }
            }


            



            /*
            if (StateOfGame == GameState.controllerModeSelect && !UsingWristStraps)
            {
                GoToState(GameState.boardCalibration);
            }*/




            //GoToState(StateOfGame + 1);

            //NextState();

            //Debug.Log("hello" + StateOfGame++);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.B))
        {
            //LastState();
            //GoToState(StateOfGame-1);
            //Debug.Log("hello backwards" + StateOfGame--);

            GoToState(GameStateDict[StateOfGame].LastGameState);



        }



        //LoadSceneFromGameState(StateOfGame);


        GameStateText.text = StateOfGame.ToString();
    }

    public bool LoadSceneFromGameState(GameState aState)
    {
        if (GameStateDict.ContainsKey(aState))
        {
            if (!GameStateDict[aState].Loading && !GameStateDict[aState].Loaded)
            {
                StartCoroutine(LoadGameStateScene(GameStateDict[aState]));

            }
            else if (GameStateDict[aState].Loaded)
            {
                /*if(GameStateDict[aState].CallOnLoaded != "")
                    SendMessage(GameStateDict[aState].CallOnLoaded);*/

            }

            return true;

        }

        return false;
    }


    IEnumerator LoadGameStateScene(GameStateScene aScene)
    {
        aScene.Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(aScene.SceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (aScene.SetActiveOnLoaded)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(aScene.SceneName));

            foreach (GameStateScene gss in gameStateScene)
            {
                if (StateOfGame == gss.AssociatedState)
                    continue;

                if (gss.SetActiveOnLoaded && gss.SceneName != "" && gss.Loaded)
                {
                    Debug.Log("Unloading scene " + gss.SceneName);

                    SceneManager.UnloadSceneAsync(gss.SceneName);
                    gss.Loaded = false;
                }

            }
        }

        aScene.Loading = false;
        aScene.Loaded = true;

        if (aScene.CallOnLoaded != "")
            SendMessage(aScene.CallOnLoaded);

        Debug.Log("loaded " + aScene.SceneName);
    }

    public void SetControllerMode(bool wristStraps)
    {
        UsingWristStraps = wristStraps;
        controllerModeSelected = true;
    }

    void CheckWristStraps()
    {
        if (!UsingWristStraps)
        {
            isArmCalibrated = true;
            NextState();
        }

    }

    public void StartGameplay(TextAsset aSongJson, AudioClip aSongAudio)
    {

        SongJsonToPlay = aSongJson;

        SongAudioToPlay = aSongAudio;

        GoToState(GameState.gamePlay);
        
    }
}
