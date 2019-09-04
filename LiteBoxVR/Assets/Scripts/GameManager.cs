using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : TagModularity
{
    // use controller pitch axis to determine if the controller is being pointed at the bag
    // then when the controller is horizontal, check is the controller velocities are below the threshold


    //started at 1:30
    public SongLoader songLoader;

    private Animator GameStateAnim;

    public static GameManager Instance { get; private set; }

    public enum LoadableScenes
    {
        //set each of these to the index of the scenes in the build settings
        ControllerSelection = 8,
        ArmCalibration = 5,
        BoardPlacement = 9,
        Env_Studio = 2,
        Env_Setup = 7,
        RevolvingMenuTest = 3,
        ModularGameplayTest = 4,
        BoardCalibration = 6,
        VrBaseScene = 1,
        PunchingBagCalibration = 10,
        BoardTypeSelection = 11,
        GamemodeSelect = 12,
        RobotGameplay = 13,
        Env_Robot = 14,
        PunchingBagGameplay = 15,
        PunchingBagRenderTexture = 16
    }

    [System.Serializable]
    public class SceneState
    {
        public LoadableScenes SceneIndex;

        // animStateID is used to prevent this scene from being unloaded when a scene from the same anim state is loaded
        //may not need it stored here, can pass from AddSceneState() instead
        public uint AnimStateID;

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

    public enum Gamemode
    {
        LiteboxerDevice,
        RobotBoxing,
        LightUpBag
    }

    /*
    public class Gamemode
    {
        public enum Type
        {
            LiteboxerDevice,
            RobotBoxing
        }

        public string GamemodeName;

        public BoardType boardType;
    }*/

        /*
         
        data to be inherited from parent gameplay class

        + lerp progress of note

        virtual functions that call when note is created/destroyed

        child class should store the start and end points of each pad

        child
        + list of gameobjects the same length as the parent's list of notes
        + note gameobject position should be determined completely by the child

        or

        + dictionary of NoteObjects
        key = int (id)
        value = NoteObject

        class NoteObject
        {
            public int id;
            public float lerpProgress;
            GameObject gameObject;

            NoteObject(GameObject Prefab, int id)
            {
                this.id = id;
                gameObject = Instantiate(Prefab);
            
            }

            SetPosition(Vector3 position)
            {
                gameObject.transform.position = position;
            }


        }

        override functions that call when note is created/destroyed

        creation function

        void override CreateNoteObject(ref float LerpProgress)
        {



        }

         
         
         */

    public Gamemode gamemode;

    public class CalibratedObject
    {
        public bool calibrated;

        public Vector3 position;

        public Vector3 eulerAngles;

        public Vector3 localScale;

        public void SetCalibration(Transform sourceTransfrom)
        {
            calibrated = true;

            position = sourceTransfrom.position;

            eulerAngles = sourceTransfrom.eulerAngles;

            localScale = sourceTransfrom.localScale;
        }

        public void GetCalibration(ref GameObject targetGameObject)
        {
            targetGameObject.transform.position = position;

            targetGameObject.transform.eulerAngles = eulerAngles;

            targetGameObject.transform.localScale = localScale;

        }

        public void ResetCalibration ()
        {
            calibrated = false;

        }

    }

    public CalibratedObject calibratedObject;

    /*
    public class CalibratedBag : CalibratedDevice
    {
        public float radius;

        public float height;

        CalibratedBag()
        {


        }
        
        void SetCalibration(float radius, float height, Vector3 position)
        {
            calibrated = true;
            this.radius = radius;
            this.height = height;
            this.position = position;

        }
        
    }*/
    

    [Header("Game State Scenes To Load")]
    public List<SceneState> gameStateScenes;

    Dictionary<LoadableScenes, SceneState> SceneStateDict = new Dictionary<LoadableScenes, SceneState>();

    public TextMesh GameStateText;

    //public bool isBoardTracked;

    //public bool isBoardPlaced;

    public bool isBoardTypeSelected;

    public bool isGamemodeSelected;

    public bool isArmCalibrated;

    public bool controllerModeSelected;

    public bool UsingWristStraps;

    //public Vector3 BoardPosition;

    //public Vector3 BoardForward;

    public AudioClip SongAudioToPlay;

    public TextAsset SongJsonToPlay;

    public int SelectedSong;

    public int SongDifficulty;

    public GameObject OptionsMenuObj;

    // animator hashes
    private readonly int NextStateParam = Animator.StringToHash("NextState");
    private readonly int LastStateParam = Animator.StringToHash("LastState");
    private readonly int StartGameParam = Animator.StringToHash("StartGame");
    private readonly int ArmCalibratedParam = Animator.StringToHash("isArmCalibrated");
    private readonly int BoardCalibratedParam = Animator.StringToHash("isBoardCalibrated");
    private readonly int BoardPlacedParam = Animator.StringToHash("isBoardPlaced");
    private readonly int GamemodeSelectedParam = Animator.StringToHash("gamemodeSelected");
    private readonly int BoardTypeParam = Animator.StringToHash("BoardType");
    private readonly int GamemodeParam = Animator.StringToHash("Gamemode");
    private readonly int ControlModeSelectedParam = Animator.StringToHash("controlModeSelected");
    private readonly int UsingWristStrapsParam = Animator.StringToHash("UsingWristStraps");
    private readonly int RestartCalibrationParam = Animator.StringToHash("RestartCalibration");
    private readonly int ReturnToSongMenuParam = Animator.StringToHash("ReturnToSongMenu");

    //animator IDs
    //private uint AnimIDCounter;

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

        calibratedObject = new CalibratedObject();

        SetUniqueAnimStateIDs();
    }

    private void Start()
    {
        //Song Loading
        songLoader.Load();

    }
    /*
    public void SetBoardPosition(Vector3 bPos, Vector3 bFwd)
    {
        BoardPosition = bPos;

        BoardForward = bFwd;

    }
    */
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
            GameStateAnim.SetTrigger(NextStateParam);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.B))
        {
            GameStateAnim.SetTrigger(LastStateParam);
        }

        UpdateAnimParameters();
        
    }

    public void NextState()
    {
        UpdateAnimParameters();

        GameStateAnim.SetTrigger(NextStateParam);

    }

    public void LastState()
    {
        UpdateAnimParameters();

        GameStateAnim.SetTrigger(LastStateParam);

    }

    void UpdateAnimParameters()
    {
        GameStateAnim.SetBool(ArmCalibratedParam, isArmCalibrated);

        //GameStateAnim.SetBool(BoardCalibratedParam, isBoardTracked);

        GameStateAnim.SetBool(BoardCalibratedParam, calibratedObject.calibrated);

        GameStateAnim.SetBool(BoardPlacedParam, calibratedObject.calibrated);

        GameStateAnim.SetBool(GamemodeSelectedParam, isGamemodeSelected);

        GameStateAnim.SetInteger(BoardTypeParam, (int)boardType);

        GameStateAnim.SetInteger(GamemodeParam, (int)gamemode);

        GameStateAnim.SetBool(UsingWristStrapsParam, UsingWristStraps);

        //GameStateAnim.SetBool(BoardPlacedParam, isBoardPlaced);

        GameStateAnim.SetBool(ControlModeSelectedParam, controllerModeSelected);

    }

    //unload old scenes that are marked to unload on scene change
    void UnloadOldScenes(uint AnimStateID)
    {
        List<LoadableScenes> KeyList = new List<LoadableScenes>(SceneStateDict.Keys);

        for (int i = 0; i < KeyList.Count; i++)
        {
            if (SceneStateDict[KeyList[i]].AnimStateID != AnimStateID)
            {
                if (SceneStateDict[KeyList[i]].UnloadOnStateChange)
                {
                    SceneManager.UnloadSceneAsync((int)KeyList[i]);

                    SceneStateDict.Remove(KeyList[i]);

                }
            }
        }

        /*
        foreach (var value in SceneStateDict.Values)
        {
            if (value.AnimStateID == AnimStateID)
                continue;

            if(value.UnloadOnStateChange)
            {
                SceneManager.UnloadSceneAsync((int)value.SceneIndex);

                SceneStateDict.Remove(value.SceneIndex);
            }

        }
        */
    }


    IEnumerator LoadSceneState(SceneState aScene)
    {
        // this unloads scenes just loaded on the same anim state
        // add an int param unique to each state
        // when loading new scenes the unloader can ignore the scenes with the same int param
        //pass ignore int to unloadOldScenes
        UnloadOldScenes(aScene.AnimStateID);

        aScene.Loading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)aScene.SceneIndex, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (aScene.SetActiveOnLoaded)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            
            /* this works well, but we want it to unload the start scene
            //remove the old active scene
            if (SceneStateDict.ContainsKey((LoadableScenes)SceneManager.GetActiveScene().buildIndex))
            {
                SceneStateDict.Remove((LoadableScenes)SceneManager.GetActiveScene().buildIndex);

                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
            */

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

    public void StartGameplay(Song song, int difficulty)
    {
        SongDifficulty = difficulty;

        SongAudioToPlay = song.Audio(difficulty);

        GameStateAnim.SetTrigger(StartGameParam);

    }

    public void RestartCalibration()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex == (int)LoadableScenes.ControllerSelection)
                return;
        }

        OptionsMenuObj.SetActive(false);

        GameStateAnim.SetTrigger(RestartCalibrationParam);

    }

    public void ReturnToSongMenu()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex == (int)LoadableScenes.RevolvingMenuTest)
                return;
        }

        ToggleOptionsMenu(false);

        GameStateAnim.SetTrigger(ReturnToSongMenuParam);
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
    /*
    public uint GetUniqueAnimStateID()
    {
        AnimIDCounter++;
        Debug.Log(AnimIDCounter);
        AnimGameStateController[] animStates = GameStateAnim.GetBehaviours<AnimGameStateController>();
        return AnimIDCounter;
    }
    */
    public void SetUniqueAnimStateIDs()
    {
        
        AnimGameStateController[] animStates = GameStateAnim.GetBehaviours<AnimGameStateController>();

        for (uint i = 0; i < animStates.Length; i++)
        {
            //Debug.Log(i);

            animStates[i].SetId(i);
        }

        
    }
}

