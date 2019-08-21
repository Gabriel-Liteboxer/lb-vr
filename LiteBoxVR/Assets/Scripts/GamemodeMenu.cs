using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeMenu : RevolvingMenuParent
{
    public GamemodeConfig[] GamemodeLibrary;

    public GameObject MenuItemPrefab;

    public TMPro.TextMeshPro GamemodeName;

    public override void GenerateTiles()
    {
        MenuItemTiles = new MenuItem[GamemodeLibrary.Length];

        for (int i = 0; i < MenuItemTiles.Length; i++)
        {
            MenuItemTiles[i] = new MenuItem();

            MenuItemTiles[i].MenuItemObject = GameObject.Instantiate(MenuItemPrefab, transform);

            MenuItemTiles[i].MenuItemObject.GetComponent<GamemodeTile>().SetTile(GamemodeLibrary[i]);

            MenuItemTiles[i].MenuItemRenderer = MenuItemTiles[i].MenuItemObject.GetComponent<GamemodeTile>().GetTumbnailRenderer();

            /*Material NewAlbumCover = new Material(AlbumCoverMat);

            NewAlbumCover.SetTexture("_MainTex", GamemodeLibrary[i].Thumbnail);

            MenuItemTiles[i].MenuItemMat = MenuItemTiles[i].MenuItemObject.GetComponentInChildren<Renderer>();

            MenuItemTiles[i].MenuItemMat.material = NewAlbumCover;*/

            //NewAlbumCover.SetTexture("_MainTex", SongLibrary[i].AlbumCover);
        }

        GameManager.Instance.isGamemodeSelected = false;
    }

    public override void SongSelectionChanged()
    {
        base.SongSelectionChanged();

        GamemodeName.text = GamemodeLibrary[currentlySelected].GamemodeName;

        //ArtistName.text = GamemodeLibrary[currentlySelected].ArtistName;

    }

    public void SelectGamemode() // called by button
    {
        GameManager.Instance.gamemode = GamemodeLibrary[currentlySelected].gamemode;

        GameManager.Instance.isGamemodeSelected = true;

        GameManager.Instance.NextState();
    }
}
