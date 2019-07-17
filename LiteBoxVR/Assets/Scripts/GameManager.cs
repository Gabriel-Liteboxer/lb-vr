using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : TagModularity
{
    
    public enum GameState
    {
        defaultState,
        setupEnvironmentLoad,
        controllerModeSelect,
        armCalibration,
        boardCalibration,
        gamemodeSelect,
        environmentLoad,
        songSelect,
        gamePlay,
        gamePaused,
        gameOver

    }


    [Header("Current State of Game")]
    public GameState StateOfGame;

    [System.Serializable]
    public class GameStateScene
    {
        public GameState AssociatedState;
        public string SceneName;

        public bool Loading;
        public bool Loaded;

        public string CallOnLoaded;
        public bool SetActiveOnLoaded;
        public bool UnloadOnStateChange;

    }

    [Header("Game State Scenes To Load")]
    public GameStateScene[] gameStateScene;

    Dictionary<GameState, GameStateScene> GameStateDict = new Dictionary<GameState, GameStateScene>();

    public TextMesh GameStateText;

    public bool isBoardTracked;

    public bool isArmCalibrated;

    private bool controllerModeSelected;

    public bool UsingWristStraps;

    Vector3 BoardPosition;

    Vector3 BoardForward;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameStateScene gss in gameStateScene)
        {
            GameStateDict.Add(gss.AssociatedState, gss);

        }
        NextState();
    }

    public void SetBoardPosition(Vector3 bPos, Vector3 bFwd)
    {
        isBoardTracked = true;

        BoardPosition = bPos;

        BoardForward = bFwd;

    }

    public void ApplyBoardPosition()
    {
        FindTaggedObject("BoardObj").transform.position = BoardPosition /*+ BoardForward*0.016f*/;

        FindTaggedObject("BoardObj").transform.forward = BoardForward;

        Debug.Log("Applied Board Position");
    }

    public void NextState()
    {
        do
        {
            if (StateOfGame == GameState.armCalibration && !isArmCalibrated) { Debug.Log("Must Calibrate Arm"); break; }

            if (StateOfGame == GameState.boardCalibration && !isBoardTracked) { Debug.Log("Must Calibrate Board"); break; }

            if (StateOfGame == GameState.controllerModeSelect && !controllerModeSelected) { Debug.Log("Must SelectControllerMode"); break; }

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
    }

    public void LastState()
    {
        StateOfGame--;

    }

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextState();
           

        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //LastState();

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
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(aScene.SceneName));

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
}
