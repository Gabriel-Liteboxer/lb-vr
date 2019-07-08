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

    public float SliderVelocity;

    public float MenuRadius;

    public float startangle;

    public GameObject LeftHand;

    public GameObject RightHand;

    private bool leftHandSliderContact;

    float LeftHandStartContactAngle;

    class Album
    {
        public GameObject AlbumObject;

        public Renderer AlbumMat;
    }

    private void Start()
    {
        RightHand = FindTaggedObject("HandR");

        LeftHand = FindTaggedObject("HandL");

        AlbumTiles = new Album[SongLibrary.Length];

        for (int i = 0; i < AlbumTiles.Length; i++)
        {
            AlbumTiles[i] = new Album();

            AlbumTiles[i].AlbumObject = GameObject.Instantiate(AlbumPrefab);

            Material NewAlbumCover = new Material(AlbumCoverMat);

            NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].AlbumCover);

            AlbumTiles[i].AlbumMat = AlbumTiles[i].AlbumObject.GetComponentInChildren<Renderer>();

            AlbumTiles[i].AlbumMat.material = NewAlbumCover;

            //NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].AlbumCover);
        }
    }

    private void Update()
    {
        //RightHand = FindTaggedObject("HandR");

        //LeftHand = FindTaggedObject("HandL");



        CheckHandSlider();

        SliderVelocity = Input.GetAxis("Horizontal")/5;

        SliderVelocity = Mathf.Lerp(SliderVelocity, 0, Time.deltaTime*(10/SliderVelocity));

        MenuSlider += SliderVelocity;
        
        if (MenuSlider < 0)
        {
            MenuSlider = 0;

        }
        else if (MenuSlider > AlbumTiles.Length - 1)
        {
            MenuSlider = AlbumTiles.Length - 1;

        }

        int currentlySelected = (int)(MenuSlider + 0.5f);

        if(!leftHandSliderContact)
            MenuSlider = Mathf.Lerp(MenuSlider, currentlySelected, Time.deltaTime*5);

        float selectedAngle = startangle * (Mathf.PI / 180f);

        for (int i = 0; i < AlbumTiles.Length; i++)
        {
            float TileOffsetRad = TileOffsetDegrees * (Mathf.PI/180f);

            //Debug.Log("rad" + TileOffsetRad);

            float angleRad = TileOffsetRad * i - MenuSlider * TileOffsetRad;

            float transparency = 1 - Mathf.Abs(MenuSlider - i)/4f;

            float ScaleValue = 1;

            if (transparency < 0)
                transparency = 0;

            if(i == currentlySelected)
            {
                ScaleValue = 1.5f;
                transparency = 1;
                angleRad = selectedAngle;
            }

            AlbumTiles[i].AlbumObject.transform.position = new Vector3(transform.position.x + MenuRadius * Mathf.Cos(angleRad), transform.position.y, transform.position.z + MenuRadius * Mathf.Sin(angleRad));

            AlbumTiles[i].AlbumMat.material.color = new Color(1, 1, 1, transparency);

            AlbumTiles[i].AlbumObject.transform.LookAt(transform);

            AlbumTiles[i].AlbumObject.transform.localScale = Vector3.Lerp(AlbumTiles[i].AlbumObject.transform.localScale, Vector3.one * ScaleValue, Time.deltaTime * 5);
        }

        AlbumTiles[currentlySelected].AlbumObject.transform.localPosition += AlbumTiles[currentlySelected].AlbumObject.transform.forward * 0.08f;

    }

    void CheckHandSlider()
    {
        if (WithinDonut(LeftHand.transform.position, transform.position, MenuRadius, MenuRadius + 5, 5))
        {
            //Debug.Log("within donut");

            Vector3 dir = LeftHand.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (leftHandSliderContact)
                MenuSlider += (angle - LeftHandStartContactAngle)/TileOffsetDegrees;
            else
                leftHandSliderContact = true;

            LeftHandStartContactAngle = angle;            

        }
        else
        {
            leftHandSliderContact = false;

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

}
