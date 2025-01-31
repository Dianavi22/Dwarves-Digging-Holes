using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreGold : MonoBehaviour
{
    private bool isActive = true;
    public GameObject gfx;

    public int IDGoldStep { get; private set; }

    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem _spawnPart;
    [SerializeField] ParticleSystem _destroyPart;
    private ShakyCame _sc;
    private GoldChariot _gc;

    public GameObject myPlateform;

    public Transform GetSpawnPoint => spawnPoint;

    private void Start()
    {
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();
        _gc = TargetManager.Instance.GetGameObject<GoldChariot>();
    }

    public void Instanciate(int currentID)
    {
        IDGoldStep = currentID;
        _spawnPart.Play();
    }

    private void Update()
    {
        if (isActive && _gc.GetHighestIndexStepList < IDGoldStep)
        {
            isActive = false;
            _gc.LostGoldStage(IDGoldStep);
            Destroy(gameObject);
            StartCoroutine(DespawnBlock());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (Utils.Component.TryGetInParent<Rock>(other, out _))
        {

            isActive = false;
            _gc.LostGoldStage(IDGoldStep);
            Destroy(gameObject);
            StartCoroutine(DespawnBlock());
           

        }
    }

    public IEnumerator DespawnBlock()
    {
        GetComponent<Collider>().enabled = false;
        myPlateform.SetActive(false);
        gfx.SetActive(false);
        print("isActive "+ isActive + " currentID "+ IDGoldStep);
        _sc.ShakyCameCustom(0.3f, 0.5f);
        _destroyPart.Play();

        yield return new WaitForSeconds(_destroyPart.main.duration + 0.1f);
        Destroy(gameObject);
    }
}
