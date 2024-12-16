using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BigRock : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _canva;
    [SerializeField] Collider _collider;
    [SerializeField] ParticleSystem _destroyPart;

    public IEnumerator DestroyBigRock()
    {
        _gfx.SetActive(false);
        _canva.SetActive(false);
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.2f, 0.4f);
        _destroyPart.Play();
        _collider.enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
