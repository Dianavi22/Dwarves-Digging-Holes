using UnityEngine;

public class GameResolution : MonoBehaviour
{
    public int targetWidth = 1920;
    public int targetHeight = 1080;

    void Start()
    {
        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution res in resolutions)
        {
            if (res.width == targetWidth && res.height == targetHeight)
            {
                Screen.SetResolution(targetWidth, targetHeight, true);
                break;
            }
        }
    }
}