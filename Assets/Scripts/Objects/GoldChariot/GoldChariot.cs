using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    [SerializeField] private int _goldCount;
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;
    public int goblinOnTheChariot;

    private FixedJoint _joint;

    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            UpdateText();
        }
    }

    void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _goldCountText.text = _goldCount.ToString();
    }

    public void TryJoinPlayer(Rigidbody player)
    {
        if (_joint != null)
        {
            Debug.Log(_joint);
            return;
        }
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = player;
    }

    public void EmptyJoin()
    {
        Destroy(_joint);
        _joint = null;
    }
}