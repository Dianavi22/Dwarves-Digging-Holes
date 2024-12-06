using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGame : MonoBehaviour
{
    [SerializeField] List<GameObject> _ladderPart;
    [SerializeField] Lava _lava;
    private float _scrollSpeed;
    private float _baseScrollSpeed;

    private void Awake()
    {
       
    }
    private void Start()
    {
     
        StartCoroutine(LadderIntro());
    }
    private IEnumerator LadderIntro()
    {
        //_scrollSpeed = GameManager.Instance.Difficulty.ScrollingSpeed;
        //_baseScrollSpeed = _scrollSpeed;
        //_scrollSpeed = 0;
        yield return new WaitForSeconds(3);
        for (int i = 0; i < _ladderPart.Count; i++)
        {
            if(i == _ladderPart.Count)
            {
              //  _scrollSpeed = _baseScrollSpeed;

            }
                StartCoroutine(DestroyPart(i));
            yield return new WaitForSeconds(0.3f);
        }

    }

    private IEnumerator DestroyPart(int i)
    {
        _ladderPart[i].GetComponentInChildren<ParticleSystem>().Play();
        _ladderPart[i].GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3);
        Destroy(_ladderPart[i]);

    }
}
