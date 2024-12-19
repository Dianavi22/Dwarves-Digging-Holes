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
            WillDestroyStep();

        //if (isSpawn && _canSpawn)
        //{
        //    isSpawn = false;
        //    StartCoroutine(CdSpawn());
        //    SpawnPepite();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (Utils.Component.TryGetInParent<Rock>(other, out _))
            WillDestroyStep();
    }

    //private IEnumerator CdSpawn()
    //{
    //    _canSpawn = false;

    //    yield return new WaitForSeconds(3);
    //    _canSpawn = true;
    //}

    private void WillDestroyStep()
    {
        // Test without cooldown when losing gold
        _gc.LostGoldStage(IDGoldStep);
        StartCoroutine(DespawnBlock());
    }

    public IEnumerator DespawnBlock()
    {
        GetComponent<Collider>().enabled = false;
        myPlateform.SetActive(false);
        gfx.SetActive(false);
        isActive = false;

        _sc.ShakyCameCustom(0.3f, 0.5f);
        _destroyPart.Play();

        yield return new WaitForSeconds(_destroyPart.main.duration + 0.1f);
        Destroy(gameObject);
    }
}
