using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    private int _goldCount = 10;
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;
    public int goblinOnTheChariot;

    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            UpdateText();
        }
    }

   


    private void OnCollisionEnter(Collision collision)
    {
        
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