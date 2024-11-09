using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;

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
}