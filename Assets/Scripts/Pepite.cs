using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Pepite : MonoBehaviour
{
    private bool _isDestroy;
    private bool _canGet = false;
    private ParticleSystem _gcPart ;
    [SerializeField] ParticleSystem _takeNuggetPart;
    [SerializeField] private EventReference pickUpANuggetSound;
    [SerializeField] private EventReference nuggetInTheCartSound;


    private void Start()
    {
        Invoke("CanGetNugget", 0.5f);

        //ToDo find better solution 
        _gcPart = GameObject.Find("TakeOneGold_PART").GetComponent<ParticleSystem>(); 
    }
    private void OnCollisionEnter(Collision collision)
    {

         if (collision.collider.CompareTag("GoldChariot"))
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider, true);
            }

        //if (Utils.Component.TryGetInParent<Rock>(collision.collider, out var rock))
        //{
        //    Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.collider, true);
        //}

        if (_canGet)
        {
            if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player) && !_isDestroy)
            {
                PickUpANuggetSoundSound();
                NuggetInTheCartSoundSound();
                _isDestroy = true;
                _gcPart.Play();
                TargetManager.Instance.GetGameObject<GoldChariot>().TakeNugget();
                StartCoroutine(DestroyNugget());

            }

            if (Utils.Component.TryGetInParent<Enemy>(collision.collider, out var enemy) && !_isDestroy)
            {
                _isDestroy = true;
                Destroy(gameObject);

            }

            if (Utils.Component.TryGetInParent<Lava>(collision.collider, out var lava) && !_isDestroy)
            {
                _isDestroy = true;
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator DestroyNugget()
    {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        _takeNuggetPart.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void CanGetNugget()
    {
        _canGet = true;
    }

    #region Sounds
    private void PickUpANuggetSoundSound()
    {
        RuntimeManager.PlayOneShot(pickUpANuggetSound, transform.position);
    }

    private void NuggetInTheCartSoundSound()
    {
        RuntimeManager.PlayOneShot(nuggetInTheCartSound, transform.position);
    }
    #endregion
}
