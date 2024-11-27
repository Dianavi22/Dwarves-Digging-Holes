using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour, IGrabbable
{
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;

    [SerializeField] private Score _score;

    [SerializeField] private int _goldScore;

    private int _nbGolbinOnChariot;
    public int NbGoblin
    {
        get => _nbGolbinOnChariot;
        set
        {
            _nbGolbinOnChariot = value;
            UpdateParticle();
        }
    }

    private int _currentGoldCount;
    public int GoldCount
    {
        get => _currentGoldCount;
        set
        {
            _score.AddScoreOnce(_currentGoldCount < value ? _goldScore : -(_goldScore / 2));
            _currentGoldCount = value;
            UpdateText();
        }
    }
    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    private void UpdateText()
    {
        _goldCountText.text = GoldCount.ToString();
    }

    private void UpdateParticle()
    {
        if (_nbGolbinOnChariot > 0 && !_lostGoldPart.isPlaying)
        {
            _lostGoldPart.Play();
        } else if (_nbGolbinOnChariot <= 0)
        {
            _lostGoldPart.Stop();
        }
        Debug.Log(NbGoblin + " - isPlaying: " + _lostGoldPart.isPlaying.ToString());
    }

    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        throw new System.NotImplementedException();
    }

    public void HandleDestroy()
    {
        throw new System.NotImplementedException();
    }
}