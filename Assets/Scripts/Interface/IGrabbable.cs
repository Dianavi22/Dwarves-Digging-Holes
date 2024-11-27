interface IGrabbable
{
    public void HandleCarriedState(Player currentPlayer, bool isGrabbed);

    public void HandleDestroy();
}