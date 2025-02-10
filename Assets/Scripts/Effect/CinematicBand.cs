using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CinematicBand : MonoBehaviour
{
    public static CinematicBand Instance; // A static reference

    [SerializeField] private Transform[] _cinematicBandList;

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private readonly float[] _showPositions = { 475f, -475f };
    private readonly float[] _closePositions = { 650f, -650 };
    private const float _animationDuration = 1.75f;

    public void ShowCinematicBand() => MoveCinematicBands(_showPositions);
    
    public void CloseCinematicBand() => MoveCinematicBands(_closePositions);

    private void MoveCinematicBands(float[] targetPositions)
    {
        for (int i = 0; i < _cinematicBandList.Length && i < targetPositions.Length; i++)
        {
            _cinematicBandList[i].DOLocalMoveY(targetPositions[i], _animationDuration);
        }
    }
}
