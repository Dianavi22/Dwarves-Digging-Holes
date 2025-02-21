using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BigRock : MonoBehaviour
{
    [SerializeField] private EventReference bigRockExplosionSound;

    [SerializeField] GameObject _gfx;
    [SerializeField] GameObject _canva;
    [SerializeField] Collider _collider;
    [SerializeField] ParticleSystem _destroyPart;

    public IEnumerator DestroyBigRock()
    {
        PlayBigRockExplosionSound(gameObject.transform.position);

        _gfx.SetActive(false);
        _canva.SetActive(false);
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.2f, 0.4f);
        _destroyPart.Play();
        _collider.enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void PlayBigRockExplosionSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(bigRockExplosionSound, position);
    }
}
