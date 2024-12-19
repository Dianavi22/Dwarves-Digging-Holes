using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGame : MonoBehaviour
{
    [SerializeField] List<GameObject> _ladderPart;

    public IEnumerator LadderIntro()
    {
        ShakyCame sc = TargetManager.Instance.GetGameObject<ShakyCame>();
        for (int i = 0; i < _ladderPart.Count; i++)
        {
            sc.ShakyCameCustom(0.2f, 0.2f);
            StartCoroutine(DestroyPart(_ladderPart[i]));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator DestroyPart(GameObject ladderPart)
    {
        ladderPart.GetComponentInChildren<ParticleSystem>().Play();
        ladderPart.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3);
        Destroy(ladderPart);
    }
}
