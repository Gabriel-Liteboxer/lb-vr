using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : TagModularity
{
    
    public enum GameState
    {
        controllerModeSelect,
        calibration,
        gamemodeSelect,
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

    }

    [Header("Game State Scenes To Load")]
    public GameStateScene[] gameStateScene;

    Dictionary<GameState, GameStateScene> GameStateDict = new Dictionary<GameState, GameStateScene>();

    /*
    [Header("Scene Names")]
    public string EnvironmentScene;
    public string SongSelectScene;
    public string ControllerModeSelectScene;
    public string GamePlayScene;


    [Header("Scene Loading Status")]
    public bool LoadingControllerSelect;
    public bool LoadingControllerCalibration;
    public bool LoadingEnvironment;
    public bool LoadingSongSelect;
    public bool LoadingGamePlay;*/

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameStateScene gss in gameStateScene)
        {
            GameStateDict.Add(gss.AssociatedState, gss);

        }

        //StartCoroutine(LoadEnvironment());
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            StateOfGame++;

        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            StateOfGame--;

        }

        /*
        if(GameStateDict.ContainsKey(StateOfGame))
        {
            if(!GameStateDict[StateOfGame].Loading)
            {
                StartCoroutine(LoadGameStateScene(GameStateDict[StateOfGame]));

            }
            else if (GameStateDict[StateOfGame].Loaded)
            {
                SendMessage(GameStateDict[StateOfGame].CallOnLoaded);

            }

        }
        */
        /*
        if (StateOfGame == GameState.songSelect)
        {
            if(!LoadingSongSelect)
            StartCoroutine(LoadSongSelect());

        }*/

        LoadSceneFromGameState(StateOfGame);

        
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
                if(GameStateDict[aState].CallOnLoaded != "")
                SendMessage(GameStateDict[aState].CallOnLoaded);

            }

            return true;

        }

        return false;
    }

    /*
    IEnumerator LoadControllerModeSelect()
    {
        LoadingControllerSelect = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SongSelectScene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // put assign tag thing here 

        FindTaggedObject("SongMenu");
        Debug.Log("loaded song select");
    }


    IEnumerator LoadSongSelect()
    {
        LoadingSongSelect = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SongSelectScene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // put assign tag thing here 

        FindTaggedObject("SongMenu");
        Debug.Log("loaded song select");
    }


    IEnumerator LoadEnvironment ()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(EnvironmentScene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(EnvironmentScene));

        Debug.Log("loaded environment");
    }
    */
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

        Debug.Log("loaded " + aScene.SceneName);
    }
}
