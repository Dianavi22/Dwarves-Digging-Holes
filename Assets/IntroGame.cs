using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGame : MonoBehaviour
{
    [SerializeField] List<GameObject> _ladderPart;
    [SerializeField] Lava _lava;
    [SerializeField] ShakyCame _sc;

    private void Start()
    {
    }
    public IEnumerator LadderIntro()
    {
        
        for (int i = 0; i < _ladderPart.Count; i++)
        {
            _sc.ShakyCameCustom(0.2f, 0.2f);
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
