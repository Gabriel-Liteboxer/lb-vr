using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelection : TagModularity
{
    public SongConfig[] SongLibrary;

    public GameObject AlbumPrefab;

    public Material AlbumCoverMat;

    private GameObject[] AlbumTiles;

    public float TileOffsetX;

    public float TileOffsetY;

    public int TileNumX;

    public int TileNumY;

    public Transform GridRootPos;

    public int CurrentPage;

    public Transform RightCont;

    public Transform LeftCont;

    //Dictionary<GameObject, System.Action> ButtonActionManager;

    public int SelectedSong = 0;

    public GameObject SelectedAlbum;

    private Renderer SelectedAlbumCover;

    public TextMesh SongName;

    public TextMesh ArtistName;

    public NoteManager NoteMgr;

    public int SongDifficultyLevel = 0;

    private GameObject LastHoveredButton = null;

    void Start()
    {
        //ButtonActionManager = new Dictionary<GameObject, System.Action>();

        RightCont = FindTaggedObject("HandR").transform;

        LeftCont = FindTaggedObject("HandL").transform;

        AlbumTiles = new GameObject[SongLibrary.Length];



        SelectedAlbumCover = SelectedAlbum.GetComponent<Renderer>();

        SelectedAlbumCover.material = new Material(AlbumCoverMat);

        SelectSong(SelectedSong);



        for (int i = 0; i < SongLibrary.Length; i++)
        {
            AlbumTiles[i] = GameObject.Instantiate(AlbumPrefab, GridRootPos);

            Material NewAlbumCover = new Material(AlbumCoverMat);

            NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].song.albumArt);

            AlbumTiles[i].GetComponent<Renderer>().material = NewAlbumCover;


            //configuring the button event for controller interaction

            ButtonEvent bE = AlbumTiles[i].AddComponent<ButtonEvent>();

            bE.ParameterToUse = "int";

            bE.IntParameter = i;

            bE.TargetObject = gameObject;

            bE.MethodName = "SelectSong";

            AlbumTiles[i].SetActive(false);
        }


        int h = 0;

        while (h < SongLibrary.Length)
        {
            for (int i = 0; i < TileNumX; i++)
            {
                for (int k = 0; k < TileNumY; k++)
                {
                    AlbumTiles[h].transform.localPosition = new Vector3(TileOffsetX * -k, TileOffsetY * -i);
                    h++;
                }
            }
        }

    }

    
    void Update()
    {
        
        

        RaycastHit hit;

        if (Physics.Raycast(RightCont.position, RightCont.forward, out hit))//need to change this so it only checks the VRUIButton layer
        {
            hit.transform.gameObject.SendMessage("SetHoverState", true);


            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                
                hit.transform.gameObject.SendMessage("ButtonAction");

            }
        }

        if (CurrentPage < 0)
            CurrentPage = 0;
        else if (CurrentPage > SongLibrary.Length/(TileNumX*TileNumY))
        {
            CurrentPage = SongLibrary.Length / (TileNumX * TileNumY);

        }

        for (int i = 0; i < SongLibrary.Length; i++)
        {
            
            if(i >= CurrentPage * TileNumX * TileNumY && i < (CurrentPage+1)*TileNumX*TileNumY)
                AlbumTiles[i].SetActive(true);
            else
                AlbumTiles[i].SetActive(false);
        }

    }

    public void Play ()
    {
        NoteMgr.SongClip = SongLibrary[SelectedSong].song.Audio(SongDifficultyLevel);

        //NoteMgr.SongKeyframes = SongLibrary[SelectedSong].DifficultyLevels[SongDifficultyLevel].TrackJson;

        NoteMgr.StartGame();

    }

    public void SelectSong (int SongIndex)
    {
        SelectedSong = SongIndex;

        SelectedAlbumCover.material.SetTexture("_MainTex", SongLibrary[SelectedSong].song.albumArt);

        SongName.text = SongLibrary[SelectedSong].song.name;

        ArtistName.text = SongLibrary[SelectedSong].song.artist;
    }

    public void PageChange(bool isLeft)
    {
        if(isLeft)
        {
            CurrentPage--;

        }
        else
        {
            CurrentPage++;

        }

    }
}
