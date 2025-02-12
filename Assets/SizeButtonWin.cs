using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeButtonWin : MonoBehaviour
{
    [SerializeField] private List<GameObject> _button;
    public bool scaleButton;
    void Start()
    {
        
    }

    void Update()
    {
        if (scaleButton)
        {
            UpdateButtonScale();
        }
    }

    public void UpdateButtons()
    {
        scaleButton = true;
    }

    private void UpdateButtonScale()
    {
        Vector3 targetScale = new Vector3(2.41867685f, 3.43942618f, 2.81897068f);

        for (int i = 0; i < _button.Count; i++)
        {
            if (_button[i] != null)
            {
                _button[i].transform.localScale = targetScale;
            }
        }

        scaleButton = false;
    }
}
