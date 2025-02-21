using UnityEngine;

interface IGrabbable
{
    public bool HandleCarriedState(Player currentPlayer, bool isGrabbed);

    public void HandleDestroy();

    public GameObject GetGameObject();
}