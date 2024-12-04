using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] List<string> dialogList = new List<string>();
    [SerializeField] TMP_Text _dialogText;
    private TypeSentence _typeSentence;
    private int i;

    void Start()
    {
        _typeSentence = TargetManager.Instance.GetGameObject<TypeSentence>(Target.TypeSentence);
        StartCoroutine(Dialog());
    }

    void Update()
    {
        
    }

    private IEnumerator Dialog()
    {
        yield return new WaitForSeconds(10);
        _dialogText.text = "";
        TextChoice();
        _typeSentence.WriteMachinEffect(dialogList[i], _dialogText, 0.05f);
        StartCoroutine(Dialog());
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
