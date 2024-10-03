using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    [SerializeField] private int _goldCount;
    [SerializeField] private TMP_Text _goldCountText;

    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            UpdateText();
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