using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] List<string> dialogList = new List<string>();
    [SerializeField] TMP_Text _dialogText;
    private TypeSentence _typeSentence;

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
        _typeSentence.WriteMachinEffect(dialogList[Random.Range(0, dialogList.Count)], _dialogText, 0.5f);
        StartCoroutine(Dialog());
    }
}
