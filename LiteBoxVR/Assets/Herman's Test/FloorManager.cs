using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;


    public static FloorManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayFloorAnim()
    {
        m_Animator.SetTrigger("PlayShockWave");
    }

}
