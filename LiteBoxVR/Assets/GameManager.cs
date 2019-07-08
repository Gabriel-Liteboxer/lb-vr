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

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameStateScene gss in gameStateScene)
        {
            GameStateDict.Add(gss.AssociatedState, gss);

        }

    }


    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.RawButton.A))
        {
            StateOfGame++;

        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            StateOfGame--;

        }

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
