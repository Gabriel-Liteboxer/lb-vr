using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : TagModularity
{
    private Animator GameStateAnim;

    [SerializeField]
    public AnimatorStateInfo[] animatorStateInfos;

    public enum GameState
    {
        defaultState,
        controlSelect,
        armCalibration,
        boardCalibration,
        boardPlacement,
        gamemodeSelect,
        songSelect,
        gamePlay,
        oldDemo,
        gameOver
    }

    public enum BoardType
    {
        liteboxerShield,
        cylinderPunchingBag,
        rectanglePunchingBag
    }

    public BoardType boardType;


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

        //public UnityEvent FunctionToCall;


        /*
        [Header("States before and after")]
        public GameState NextGameState;
        public GameState LastGameState;
        */

    }

    [Header("Game State Scenes To Load")]
    public GameStateScene[] gameStateScenes;

    Dictionary<GameState, GameStateScene> GameStateDict = new Dictionary<GameState, GameStateScene>();

    public TextMesh GameStateText;

    public bool isBoardTracked;

    public bool isBoardPlaced;

    public bool isArmCalibrated;

    private bool controllerModeSelected;

    public bool UsingWristStraps;

    public Vector3 BoardPosition;

    public Vector3 BoardForward;

    /*
    [System.Serializable]
    public class MyEvent : UnityEvent<bool> { }
    public MyEvent myEvent;*/

    public AudioClip SongAudioToPlay;

    public TextAsset SongJsonToPlay;

    public GameObject OptionsMenuObj;

    // Start is called before the first frame update
    void Start()
    {
        GameStateAnim = GetComponent<Animator>();
        /*
        foreach (GameStateScene gss in gameStateScenes)
        {
            GameStateDict.Add(gss.AssociatedState, gss);

        }
        NextState();*/
        
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

        Debug.Log("Applied Board Position");
    }

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

                if (StateOfGame == GameState.controlSelect && !controllerModeSelected) { Debug.Log("Must SelectControllerMode"); break; }

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
        foreach (GameStateScene gss in gameStateScenes)
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
        
        if (OVRInput.GetDown(OVRInput.RawButton.Start) || Input.GetKeyDown(KeyCode.M))
        {
            if (!OptionsMenuObj.activeInHierarchy)
                ToggleOptionsMenu(true);
            else
                ToggleOptionsMenu(false);
        }


        if (OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetKeyDown(KeyCode.A))
        {
            GameStateAnim.SetTrigger("AdvanceState");

            /*
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
            */
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.B))
        {
           
            //GoToState(GameStateDict[StateOfGame].LastGameState);
            
        }
        
        GameStateText.text = StateOfGame.ToString();

        UpdateAnimParameters();


    }

    public void OnAnimatorStateChange()
    {
        /*
        Debug.Log("called from animator " + gameStateScenes.Length);

        foreach (GameStateScene gss in gameStateScenes)
        {

            if (GameStateAnim.GetCurrentAnimatorStateInfo(0).IsName(System.Enum.GetName(typeof(GameState), gss.AssociatedState)))
            {
                Debug.Log("name: " + gss.AssociatedState.ToString());

                StateOfGame = gss.AssociatedState;

                StartCoroutine(LoadGameStateScene(gss));
            }

        }
        */
        StartCoroutine(ChangeGameState());
    }

    public IEnumerator ChangeGameState ()
    {
        yield return new WaitForSeconds(1);

        Debug.Log("called from animator " + gameStateScenes.Length);

        foreach (GameStateScene gss in gameStateScenes)
        {

            if (GameStateAnim.GetCurrentAnimatorStateInfo(0).IsName(System.Enum.GetName(typeof(GameState), gss.AssociatedState)))
            {
                Debug.Log("name: " + gss.AssociatedState.ToString());

                StateOfGame = gss.AssociatedState;

                StartCoroutine(LoadGameStateScene(gss));

                UnloadOldStates();
            }

        }
    }

    void UpdateAnimParameters()
    {
        GameStateAnim.SetBool("isArmCalibrated", isArmCalibrated);

        GameStateAnim.SetBool("isBoardCalibrated", isBoardTracked);

        GameStateAnim.SetBool("UsingWristStraps", UsingWristStraps);

        GameStateAnim.SetBool("isBoardPlaced", isBoardPlaced);

    }

    public bool LoadSceneFromGameState(GameState aState)
    {
        if (GameStateDict.ContainsKey(aState))
        {
            if (!GameStateDict[aState].Loading && !GameStateDict[aState].Loaded)
            {
                StartCoroutine(LoadGameStateScene(GameStateDict[aState]));

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

            foreach (GameStateScene gss in gameStateScenes)
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

    public void RestartGame()
    {
        SceneManager.LoadScene("VRBaseScene");

    }

    public void ReturnToSongMenu()
    {
        //GoToState(GameState.environmentLoad);
        ToggleOptionsMenu(false);
    }

    public void ToggleOptionsMenu(bool isOpen)
    {
        if (!isOpen)
            OptionsMenuObj.transform.localScale = Vector3.zero;

        OptionsMenuObj.SetActive(isOpen);

    }
}
