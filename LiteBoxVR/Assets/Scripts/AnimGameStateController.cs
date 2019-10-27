using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimGameStateController : StateMachineBehaviour
{
    /*public enum LoadableScenes
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
        public LoadableScenes scene;

        public bool SetActiveOnLoad;

        public bool UnloadOnStateChange;

    }*/

    public uint Id;

    //bool IdSet;

    public GameManager.SceneState[] ScenesToLoad;

    /*
    IEnumerator GetIDFromGameManager()
    {
        bool GameManagerNull = true;

        while(GameManagerNull)
        {
            if (GameManager.Instance != null)
            {
                GameManagerNull = false;
                Id = GameManager.Instance.GetUniqueAnimStateID();
            }

            yield return null;
        }

    }*/

    public void SetId(uint Id)
    {
        this.Id = Id;

        Debug.Log("Set Id" + Id.ToString());

    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        foreach (GameManager.SceneState ss in ScenesToLoad)
        {
            ss.AnimStateID = Id;

            GameManager.Instance.AddSceneState(ss);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        /*Debug.Log(Time.deltaTime);


        if (!IdSet)
        {
            if (GameManager.Instance != null)
            {
                Id = GameManager.Instance.GetUniqueAnimStateID();
                IdSet = true;
            }
        }*/

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);

            }

        }
    }
    /*
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        foreach (GameManager.SceneState ss in ScenesToLoad)
        {
            if(ss.UnloadOnStateChange)
                GameManager.Instance.RemoveSceneState(ss);
        }
    }
    */
}
