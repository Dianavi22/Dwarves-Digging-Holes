using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] List<string> dialogList = new List<string>();
    [SerializeField] TMP_Text _dialogText;
    [SerializeField] Image _dwarfHead;
    [SerializeField] List<Sprite> _dwarfHeadSprite;
    [SerializeField] GamePadsController _gpController;
    private TypeSentence _typeSentence;
    private int i;

    void Start()
    {
        _typeSentence = TargetManager.Instance.GetGameObject<TypeSentence>();
        StartCoroutine(Dialog());
    }

    void Update()
    {
        
    }

    private IEnumerator Dialog()
    {
        yield return new WaitForSeconds(30);
        TextChoice();
        _typeSentence.WriteMachinEffect(dialogList[i], _dialogText, 0.05f);
        StartCoroutine(Dialog());
        _dwarfHead.gameObject.SetActive(true);
        _dwarfHead.sprite = _dwarfHeadSprite[Random.Range(0,_gpController.NbPlayer)];
        yield return new WaitForSeconds(10);
        _dialogText.text = "";
        _dwarfHead.gameObject.SetActive(false);
    }

    private void TextChoice()
    {
        int j = Random.Range(0, dialogList.Count);
        if (j == i)
        {
            TextChoice();
        }
        else
        {
            i = j;
        }
    }
}
