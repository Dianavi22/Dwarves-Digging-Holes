using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    [SerializeField] private int _goldCount;
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;

    private FixedJoint _joint;
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
    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            UpdateText();
        }
    }
    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _goldCountText.text = _goldCount.ToString();
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

    public void TryJoinPlayer(Player player)
    {
        if (_joint != null)
        {
            Debug.Log(_joint);
            return;
        }
        player.GetRigidbody().mass = 20f;
        _joint = player.gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = GetComponent<Rigidbody>();
    }

    public void EmptyJoin()
    {
        _joint.GetComponent<Rigidbody>().mass = 1f;
        Destroy(_joint);
        _joint = null;
    }
}