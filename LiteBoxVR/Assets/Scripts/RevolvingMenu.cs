using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvingMenu : TagModularity
{
    public SongConfig[] SongLibrary;

    public GameObject AlbumPrefab;

    public Material AlbumCoverMat;

    private Album[] AlbumTiles;

    public float TileOffsetDegrees;

    public float MenuSlider;

    private float PreContactMenuSlider;

    public float SliderVelocity;

    public float MenuRadius;

    public float startangle;

    class Album
    {
        public GameObject AlbumObject;

        public Renderer AlbumMat;
    }

    private Transform PlayerHead;

    private GameManager gameMgr;

    public float UpdatePositionDistance = 1;

    Transform PlayerPositionTracker;

    class HandContact
    {
        public Transform handTransform;

        public bool inContact;

        public Vector3 currentContactVector;

        public Vector3 startingContactVector;

        public float contactAngle;

        public float menuSliderValueOnContact;
    }

    private HandContact LeftHand;

    private HandContact RightHand;

    private bool MovingMenuSlider;

    enum UsingHand
    {
        noHand,
        leftHand,
        rightHand
    }

    private UsingHand MenuSliderHand;

    int currentlySelected;

    int difficultyLevel;

    // put highscores to right of play button

    // put difficulty settings to left of play button

    private void Start()
    {
        LeftHand = new HandContact();

        RightHand = new HandContact();

        PlayerHead = Camera.main.gameObject.transform;

        PlayerPositionTracker = new GameObject().transform;

        //gameMgr = FindTaggedObject("GameController").GetComponent<GameManager>();

        RightHand.handTransform = FindTaggedObject("HandR").transform;

        LeftHand.handTransform = FindTaggedObject("HandL").transform;

        AlbumTiles = new Album[SongLibrary.Length];

        for (int i = 0; i < AlbumTiles.Length; i++)
        {
            AlbumTiles[i] = new Album();

            AlbumTiles[i].AlbumObject = GameObject.Instantiate(AlbumPrefab, transform);

            Material NewAlbumCover = new Material(AlbumCoverMat);

            NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].AlbumCover);

            AlbumTiles[i].AlbumMat = AlbumTiles[i].AlbumObject.GetComponentInChildren<Renderer>();

            AlbumTiles[i].AlbumMat.material = NewAlbumCover;

            //NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].AlbumCover);
        }
    }

    private void Update()
    {
        CheckHandSlider(LeftHand);

        CheckHandSlider(RightHand);

        if (LeftHand.inContact && MenuSliderHand != UsingHand.rightHand)
        {
            MenuSliderHand = UsingHand.leftHand;

        }
        else if (RightHand.inContact && MenuSliderHand != UsingHand.leftHand)
        {
            MenuSliderHand = UsingHand.rightHand;

        }
        else
        {
            MenuSliderHand = UsingHand.noHand;

        }

        if (LeftHand.inContact || RightHand.inContact)
        {
            float ContactAngle = LeftHand.contactAngle;

            if (!MovingMenuSlider)
            {
                MovingMenuSlider = true;
                PreContactMenuSlider = MenuSlider;
            }
            else
            {
                MenuSlider = PreContactMenuSlider + ContactAngle/18;
            }




        }
        else
        {
            MovingMenuSlider = false;

        }
        /*
        SliderVelocity = Input.GetAxis("Horizontal")/5;

        SliderVelocity = Mathf.Lerp(SliderVelocity, 0, Time.deltaTime*(10/SliderVelocity));

        MenuSlider += SliderVelocity;*/
        
        if (MenuSlider < 0)
        {
            MenuSlider = 0;

        }
        else if (MenuSlider > AlbumTiles.Length - 1)
        {
            MenuSlider = AlbumTiles.Length - 1;

        }

        currentlySelected = (int)(MenuSlider + 0.5f);

        if(!LeftHand.inContact && !RightHand.inContact)
            MenuSlider = Mathf.Lerp(MenuSlider, currentlySelected, Time.deltaTime*5);

        float selectedAngle = startangle * (Mathf.PI / 180f);

        for (int i = 0; i < AlbumTiles.Length; i++)
        {
            float TileOffsetRad = TileOffsetDegrees * (Mathf.PI/180f);

            //Debug.Log("rad" + TileOffsetRad);

            float angleRad = TileOffsetRad * i - MenuSlider * TileOffsetRad;

            float transparency = 1 - Mathf.Abs(MenuSlider - i)/4f;

            float ScaleValue = MenuRadius;

            if (transparency < 0)
                transparency = 0;

            if(i == currentlySelected)
            {
                ScaleValue = 1.5f * MenuRadius;
                transparency = 1;
                angleRad = selectedAngle;
            }

            AlbumTiles[i].AlbumObject.transform.position = new Vector3(transform.position.x + MenuRadius * Mathf.Cos(angleRad), transform.position.y, transform.position.z + MenuRadius * Mathf.Sin(angleRad));

            AlbumTiles[i].AlbumMat.material.color = new Color(1, 1, 1, transparency);

            AlbumTiles[i].AlbumObject.transform.LookAt(transform);

            AlbumTiles[i].AlbumObject.transform.localScale = Vector3.Lerp(AlbumTiles[i].AlbumObject.transform.localScale, Vector3.one * ScaleValue, Time.deltaTime * 5);

            if (transparency == 0)
            {
                AlbumTiles[i].AlbumMat.enabled = false;

            }
            else if (!AlbumTiles[i].AlbumMat.enabled)
            {
                AlbumTiles[i].AlbumMat.enabled = true;

            }
        }

        AlbumTiles[currentlySelected].AlbumObject.transform.localPosition += AlbumTiles[currentlySelected].AlbumObject.transform.forward * 0.08f * MenuRadius;

    }

    void CheckHandSlider(HandContact handContact)
    {
        if (WithinDonut(handContact.handTransform.position, transform.position, MenuRadius, MenuRadius + 5, 5))
        {
            handContact.currentContactVector = handContact.handTransform.position - transform.position;

            handContact.currentContactVector.Normalize();

            if(!handContact.inContact)
            {
                handContact.inContact = true;
                handContact.startingContactVector = handContact.currentContactVector;
                handContact.menuSliderValueOnContact = MenuSlider;
            }
            else
            {
                handContact.contactAngle = Vector3.SignedAngle(handContact.startingContactVector, handContact.currentContactVector, Vector3.up);

            }

        }
        else
        {
            handContact.inContact = false;

        }

    }

    bool WithinDonut(Vector3 TargetPos, Vector3 OriginPos, float innerRadius, float outerRadius, float height)
    {
        TargetPos = new Vector3(TargetPos.x, 0, TargetPos.z);

        OriginPos = new Vector3(OriginPos.x, 0, OriginPos.z);

        float SqrDistance = Vector3.SqrMagnitude(TargetPos-OriginPos);

        if (SqrDistance > innerRadius * innerRadius && SqrDistance < outerRadius * outerRadius && TargetPos.y > OriginPos.y - height / 2 && TargetPos.y < OriginPos.y + height / 2)
            return true;

        return false;
    }

    private void LateUpdate()
    {
        if (PlayerHead != null)
        {
            if (Vector3.Distance(transform.position, PlayerHead.position) > UpdatePositionDistance * UpdatePositionDistance)
                PlayerPositionTracker.position = PlayerHead.position;
        }

        transform.position = Vector3.Lerp(transform.position, PlayerPositionTracker.position, Time.deltaTime*2);

    }

    public void PlaySong ()
    {
        gameMgr.StartGameplay(SongLibrary[currentlySelected].DifficultyLevels[difficultyLevel].TrackJson, SongLibrary[currentlySelected].DifficultyLevels[difficultyLevel].audioClip);

    }

}
