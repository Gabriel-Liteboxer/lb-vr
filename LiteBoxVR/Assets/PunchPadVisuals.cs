using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPadVisuals : MonoBehaviour
{
    public GameObject NotePrefab;

    public GameplayController.NoteObject[] Notes;

    GameObject[] NoteVisuals;

    private Vector3[] NoteStartpoint;

    private Vector3[] NoteEndpoint;

    public void SetNotes(ref GameplayController.NoteObject[] newNotes)
    {
        Notes = newNotes;

        NoteVisuals = new GameObject[Notes.Length];

        NoteStartpoint = new Vector3[6];

        for (int i = 0; i < Notes.Length; i++)
        {
            NoteVisuals[i] = GameObject.Instantiate(NotePrefab);

        }
    }

    private void Update()
    {
        for (int i = 0; i < Notes.Length; i++)
        {
            NoteVisuals[i].transform.position = Vector3.Lerp(NoteStartpoint[Notes[i].pad], NoteEndpoint[Notes[i].pad], Notes[i].LerpProgress);

            
        }


    }


}
