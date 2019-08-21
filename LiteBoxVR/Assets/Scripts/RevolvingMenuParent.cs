using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvingMenuParent : MonoBehaviour
{
    

    public Material AlbumCoverMat;

    [HideInInspector]
    public MenuItem[] MenuItemTiles;

    public float TileOffsetDegrees;

    public float MenuSlider;

    private float PreContactMenuSlider;

    public float SliderVelocity;

    public float MenuRadius;

    public float MenuHeight;

    public float startangle;

    public class MenuItem
    {
        public GameObject MenuItemObject;

        public Renderer MenuItemRenderer;
    }

    private Transform PlayerHead;

    //private GameManager gameMgr;

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

    public int currentlySelected;

    int lastSelected = -1; // default value -1 so it updates text right away 

    int difficultyLevel;

    // put highscores to right of play button

    // put difficulty settings to left of play button

    //public TMPro.TextMeshPro SongName;

    //public TMPro.TextMeshPro ArtistName;

    private AudioSource MenuRatchetAudio;

    //public HighscoreManager highscoreMgr;


    private void Start()
    {
        MenuRatchetAudio = GetComponent<AudioSource>();

        LeftHand = new HandContact();

        RightHand = new HandContact();

        PlayerHead = Camera.main.gameObject.transform;

        PlayerPositionTracker = new GameObject().transform;


        transform.right = GameManager.Instance.BoardForward; // remove this and it works fine

        RightHand.handTransform = ArmPositioning.RightHandInstance.transform;

        LeftHand.handTransform = ArmPositioning.LeftHandInstance.transform;

        GenerateTiles();
    }

    public virtual void GenerateTiles()
    {


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


        if (MenuSliderHand != UsingHand.noHand)
        {
            HandContact HandTouch = new HandContact();

            if (MenuSliderHand == UsingHand.leftHand)
                HandTouch = LeftHand;
            else if (MenuSliderHand == UsingHand.rightHand)
                HandTouch = RightHand;

            float ContactAngle = HandTouch.contactAngle;

            if (!MovingMenuSlider)
            {
                MovingMenuSlider = true;
                PreContactMenuSlider = MenuSlider;
            }
            else
            {
                MenuSlider = PreContactMenuSlider + ContactAngle / 18;
            }

        }
        else
        {
            MovingMenuSlider = false;

        }

        if (MenuSlider < 0)
        {
            MenuSlider = 0;

        }
        else if (MenuSlider > MenuItemTiles.Length - 1)
        {
            MenuSlider = MenuItemTiles.Length - 1;

        }

        currentlySelected = (int)(MenuSlider + 0.5f);

        if (!LeftHand.inContact && !RightHand.inContact)
            MenuSlider = Mathf.Lerp(MenuSlider, currentlySelected, Time.deltaTime * 5);

        float ConvertToRadians = (Mathf.PI / 180f);

        float selectedAngle = (startangle - transform.eulerAngles.y) * ConvertToRadians;

        for (int i = 0; i < MenuItemTiles.Length; i++)
        {
            float TileOffsetRad = TileOffsetDegrees * ConvertToRadians;

            float radRotationY = transform.eulerAngles.y * ConvertToRadians;

            //Debug.Log("rad" + TileOffsetRad);

            float angleRad = TileOffsetRad * i - MenuSlider * TileOffsetRad;

            angleRad -= radRotationY;

           

            float transparency = 1 - Mathf.Abs(MenuSlider - i) / 4f;

            float ScaleValue = MenuRadius;

            if (transparency < 0)
                transparency = 0;

            if (i == currentlySelected)
            {
                ScaleValue = 1.5f * MenuRadius;
                transparency = 1;
                angleRad = selectedAngle;
            }

            MenuItemTiles[i].MenuItemObject.transform.position = new Vector3(transform.position.x + MenuRadius * Mathf.Cos(angleRad), transform.position.y, transform.position.z + MenuRadius * Mathf.Sin(angleRad));

            MenuItemTiles[i].MenuItemRenderer.material.color = new Color(1, 1, 1, transparency);

            MenuItemTiles[i].MenuItemObject.transform.LookAt(transform);

            MenuItemTiles[i].MenuItemObject.transform.localScale = Vector3.Lerp(MenuItemTiles[i].MenuItemObject.transform.localScale, Vector3.one * ScaleValue, Time.deltaTime * 5);

            if (transparency == 0)
            {
                MenuItemTiles[i].MenuItemRenderer.enabled = false;

            }
            else if (!MenuItemTiles[i].MenuItemRenderer.enabled)
            {
                MenuItemTiles[i].MenuItemRenderer.enabled = true;

            }
        }

        MenuItemTiles[currentlySelected].MenuItemObject.transform.position += MenuItemTiles[currentlySelected].MenuItemObject.transform.forward * 0.08f * MenuRadius;

    }

    void CheckHandSlider(HandContact handContact)
    {
        if (WithinDonut(handContact.handTransform.position, transform.position, MenuRadius, MenuRadius + 5, MenuHeight))
        {
            handContact.currentContactVector = handContact.handTransform.position - transform.position;

            handContact.currentContactVector.Normalize();

            if (!handContact.inContact)
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
        float yDistance = Mathf.Abs(TargetPos.y - OriginPos.y);

        TargetPos = new Vector3(TargetPos.x, 0, TargetPos.z);

        OriginPos = new Vector3(OriginPos.x, 0, OriginPos.z);

        float SqrDistance = Vector3.SqrMagnitude(TargetPos - OriginPos);

        if (SqrDistance > innerRadius * innerRadius && SqrDistance < outerRadius * outerRadius && yDistance < height / 2)
            return true;
       

        return false;
    }

    private void LateUpdate()
    {
        if (PlayerHead != null)
        {
            if (Vector3.Distance(PlayerPositionTracker.position, PlayerHead.position) > UpdatePositionDistance * UpdatePositionDistance)
                PlayerPositionTracker.position = PlayerHead.position;
        }

        transform.position = Vector3.Lerp(transform.position, PlayerPositionTracker.position, Time.deltaTime * 2);

        if (lastSelected != currentlySelected)
        {
            lastSelected = currentlySelected;

            SongSelectionChanged();
        }

    }

    public virtual void SongSelectionChanged()
    {
        //SongName.text = SongLibrary[currentlySelected].SongName;

        //ArtistName.text = SongLibrary[currentlySelected].ArtistName;

        MenuRatchetAudio.Play();

        //highscoreMgr.SetHighscorePage(SongLibrary[currentlySelected], difficultyLevel);
        
    }
    

    public void ChangeDifficulty(int aDifficulty)
    {
        difficultyLevel = aDifficulty;

        if (difficultyLevel > 2)
            difficultyLevel = 2;
        else if (difficultyLevel < 0)
            difficultyLevel = 0;

        SongSelectionChanged();
    }

}
