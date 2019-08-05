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

    public enum BoardType
    {
        liteboxerShield,
        cylinderPunchingBag,
        rectanglePunchingBag
    }

    public BoardType boardType;

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

    // animator hashes
    private readonly int NextState = Animator.StringToHash("NextState");
    private readonly int LastState = Animator.StringToHash("LastState");
    private readonly int ArmCalibrated = Animator.StringToHash("isArmCalibrated");
    private readonly int BoardCalibrated = Animator.StringToHash("isBoardCalibrated");
    private readonly int BoardPlaced = Animator.StringToHash("isBoardPlaced");

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

        GameStateAnim = GetComponent<Animator>();
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
            GameStateAnim.SetTrigger(NextState);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.B))
        {
            GameStateAnim.SetTrigger(LastState);
        }

        UpdateAnimParameters();
        
    }

    void UpdateAnimParameters()
    {
        GameStateAnim.SetBool(ArmCalibrated, isArmCalibrated);

        GameStateAnim.SetBool(BoardCalibrated, isBoardTracked);

        GameStateAnim.SetBool("UsingWristStraps", UsingWristStraps);

        GameStateAnim.SetBool(BoardPlaced, isBoardPlaced);

    }

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

    public void StartGameplay(TextAsset aSongJson, AudioClip aSongAudio)
    {

        SongJsonToPlay = aSongJson;

        SongAudioToPlay = aSongAudio;

        GameStateAnim.SetTrigger(NextState);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene("VRBaseScene");

    }

    public void ReturnToSongMenu()
    {
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

