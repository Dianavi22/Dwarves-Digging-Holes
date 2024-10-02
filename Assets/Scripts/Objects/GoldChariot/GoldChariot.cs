using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    private int _goldCount = 10;
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;


    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            UpdateText();
        }
    }




    private void OnCollisionExit(Collision collision)
    {
        if (Utils.TryGetParentComponent<Enemy>(collision.collider, out var enemy))
        {
            _lostGoldPart.Stop();
        }
    }


    private void UpdateText()
    {
        _goldCountText.text = _goldCount.ToString();
    }


    void Start()
    {
        UpdateText();
    }
}