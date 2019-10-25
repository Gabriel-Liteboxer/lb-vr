using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePartentTest : MonoBehaviour
{
    [SerializeField] private GameObject m_Note;

    [SerializeField] private float m_TimerMax;

    private float m_CurrentTIme;

    private Transform m_NewNotePos;

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentTIme > 0)
        {
            m_CurrentTIme -= Time.deltaTime;
        }
        else
        {
            // timer logic - every "timer max" seconds, this logic executes

            m_CurrentTIme = m_TimerMax;
            Vector3 randomizedPos = new Vector3(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + Random.Range(-.5f, .5f), transform.position.z - .5f);
            Instantiate(m_Note, randomizedPos, Quaternion.identity);

        }
    }
}
