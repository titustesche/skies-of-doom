using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class OnClick : MonoBehaviour, IPointerClickHandler
{

    public Tile setTile;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(name + " Game Object Clicked");
    }
}
