using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button _button;
    private TMP_Text _text;

    [SerializeField] private Color _color;
    [SerializeField] private Color _colorBase;

    void Start()
    {
        _button = GetComponent<Button>();
        _text = _button.GetComponentInChildren<TMP_Text>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _text.color = _color;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _text.color = _colorBase;
    }
}
