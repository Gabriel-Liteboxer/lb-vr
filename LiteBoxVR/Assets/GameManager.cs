using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : TagModularity
{
    public string EnvironmentScene;

    public string SongSelectScene;

    public string ControllerModeSelectScene;

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



    public GameState StateOfGame;

    public class GameStateScene
    {


    }


    public bool LoadingSongSelect;

    public bool LoadingControllerSelect;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadEnvironment());
    }


    private void Update()
    {
        if(StateOfGame == GameState.songSelect)
        {
            if(!LoadingSongSelect)
            StartCoroutine(LoadSongSelect());

        }

        
    }

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
}
