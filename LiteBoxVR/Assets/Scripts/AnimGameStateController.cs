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

    public GameManager.SceneState[] ScenesToLoad;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        foreach (GameManager.SceneState ss in ScenesToLoad)
        {
            GameManager.Instance.AddSceneState(ss);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);

            }

        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        foreach (GameManager.SceneState ss in ScenesToLoad)
        {
            if(ss.UnloadOnStateChange)
                GameManager.Instance.RemoveSceneState(ss);
        }
    }

}
