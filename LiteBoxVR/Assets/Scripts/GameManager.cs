using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : TagModularity
{
    //started at 1:30

    private Animator GameStateAnim;

    public static GameManager Instance { get; private set; }

    //[SerializeField]
    //public AnimatorStateInfo[] animatorStateInfos;

    public enum LoadableScenes
    {
        //set each of these to the index of the scenes in the build settings
        ControllerSelection = 7,
        ArmCalibration = 4,
        BoardPlacement = 8,
        Env_Studio = 1,
        Env_Setup = 6,
        RevolvingMenuTest = 2,
        ModularGameplayTest = 3,
        BoardCalibration = 5
    }

    [System.Serializable]
    public class SceneState
    {
        public LoadableScenes SceneIndex;

        [HideInInspector]
        public bool Loading;

        [HideInInspector]
        public bool Loaded;

        public bool SetActiveOnLoaded;
        public bool UnloadOnStateChange;

        public UnityEvent CallOnLoaded;

    }
    /*
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
    }*/

    public enum BoardType
    {
        liteboxerShield,
        cylinderPunchingBag,
        rectanglePunchingBag
    }

    public BoardType boardType;


    //[Header("Current State of Game")]
    //public GameState StateOfGame;
    /*
    [System.Serializable]
    public class GameStateScene
    {
        //AnimGameStateController.LoadableScenes Scene;

        public bool Loading;
        public bool Loaded;
        
        public bool SetActiveOnLoaded;
        public bool UnloadOnStateChange;

        public UnityEvent CallOnLoaded;

    }*/

    [Header("Game State Scenes To Load")]
    public List<SceneState> gameStateScenes;

    Dictionary<LoadableScenes, SceneState> SceneStateDict = new Dictionary<LoadableScenes, SceneState>();

    public TextMesh GameStateText;

    public bool isBoardTracked;

    public bool isBoardPlaced;

    public bool isArmCalibrated;

    private bool controllerModeSelected;

    public bool UsingWristStraps;

    public Vector3 BoardPosition;

    public Vector3 BoardForward;

    public AudioClip SongAudioToPlay;

    public TextAsset SongJsonToPlay;

    public GameObject OptionsMenuObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }

    }

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

        //animGame = new AnimGameStateManager();

        //animGame.
        
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
    /*
    public void NextState()
    {
        GoToState(StateOfGame + 1);
        
    }*/
    /*
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
    */
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
            
            GameStateAnim.SetTrigger("PreviousState");
            //GoToState(GameStateDict[StateOfGame].LastGameState);

        }
        
        //GameStateText.text = StateOfGame.ToString();

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
        //StartCoroutine(ChangeGameState());
    }

    
    /*
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
    }*/

    void UpdateAnimParameters()
    {
        GameStateAnim.SetBool("isArmCalibrated", isArmCalibrated);

        GameStateAnim.SetBool("isBoardCalibrated", isBoardTracked);

        GameStateAnim.SetBool("UsingWristStraps", UsingWristStraps);

        GameStateAnim.SetBool("isBoardPlaced", isBoardPlaced);

    }
    /*
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
    }*/

/*
    IEnumerator LoadGameStateScene(GameStateScene aScene)
    {
        if (SceneManager.GetSceneByBuildIndex(aScene.BuildIndex) != null)
            yield break;

        aScene.Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(aScene.BuildIndex, LoadSceneMode.Additive);

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
    }*/

    IEnumerator LoadSceneState(SceneState aScene)
    {
        aScene.Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)aScene.SceneIndex, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (aScene.SetActiveOnLoaded)
        {
            //remove the old active scene
            if (SceneStateDict.ContainsKey((LoadableScenes)SceneManager.GetActiveScene().buildIndex))
            {
                SceneStateDict.Remove((LoadableScenes)SceneManager.GetActiveScene().buildIndex);

                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
            
            // set the new active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)aScene.SceneIndex));

            
        }

        aScene.Loading = false;
        aScene.Loaded = true;

        //call a function when the scene has loaded
        if (aScene.CallOnLoaded != null)
            aScene.CallOnLoaded.Invoke();

        Debug.Log("loaded " + aScene.SceneIndex.ToString());
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
            //NextState();
        }

    }

    public void StartGameplay(TextAsset aSongJson, AudioClip aSongAudio)
    {

        SongJsonToPlay = aSongJson;

        SongAudioToPlay = aSongAudio;

        //GoToState(GameState.gamePlay);

        GameStateAnim.SetTrigger("AdvanceState");

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

    public void AddSceneState(SceneState aSceneState)
    {
        Debug.Log("called AddSceneState");

        if (SceneStateDict.ContainsKey(aSceneState.SceneIndex))
            return;

        SceneStateDict.Add(aSceneState.SceneIndex, aSceneState);

        StartCoroutine(LoadSceneState(aSceneState));

    }

    public void RemoveSceneState(SceneState aSceneState)
    {
        Debug.Log("called RemoveSceneState");

        if (SceneStateDict.ContainsKey(aSceneState.SceneIndex))
        {
            SceneManager.UnloadSceneAsync((int)aSceneState.SceneIndex);

            SceneStateDict.Remove(aSceneState.SceneIndex);

        }

    }


}

