using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using Utils;

public class Ending : MonoBehaviour
{
    private bool _isEnding = false;

    [SerializeField] private List<Transform> m_InterrestPoint = new ();
    [SerializeField] ParticleSystem _lavaFloorPart;
    [SerializeField] ParticleSystem _breakFlorPart;
    [SerializeField] TMP_Text _bubbletext;
    [SerializeField] TypeSentence _ts;
    [SerializeField] GameObject _bubbleEnd;
    public bool test = false;

    private Vector3 _targetPosition = new Vector3(9.97000027f, 0.810000002f, -0.660000026f);
    [SerializeField] ShakyCame _sc;
    private bool _isLavaMoving = false;

    private void Start()
    {
        _bubbleEnd = GameObject.Find("BubbleEndObject");
       // _bubbletext = FindObjectOfType<Test>().GetComponent<TMP_Text>();
       // _ts = FindObjectOfType<TypeSentence> ();
        _sc = FindObjectOfType<ShakyCame>();
        _breakFlorPart = GameObject.Find("BreakFloor_PART").GetComponent<ParticleSystem>();
        _lavaFloorPart = GameObject.Find("DecoLava_PART").GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Player>(other, out var player))
        {
            GameManager.Instance.SetScrollingSpeed(0);
            EventManager.Instance.enabled = false;
            StartCoroutine(MovePlayerToEndPosition(player));
        }
    }

    private void Update()
    {
        if(!_isEnding && null == GamePadsController.Instance.PlayerList.Find(p => p.HasCompletedLevel == false))
        {
            _isEnding = true;
            StartCoroutine(GameManager.Instance.LevelComplete());
        }

        if (_isLavaMoving)
        {
            _lavaFloorPart.transform.position = Vector3.Lerp(_lavaFloorPart.transform.position, _targetPosition, 2 * Time.deltaTime);
        }

        //if (test)
        //{
        //    test = false;
        //    GameManager.Instance.SetScrollingSpeed(0);
        //    EventManager.Instance.enabled = false;
        //    StartCoroutine(EndAnim());
        //}
    }

    private IEnumerator MovePlayerToEndPosition(Player player)
    {
        Sequence sequence = DOTween.Sequence();

        player.GetRigidbody().isKinematic = true;

        foreach (Transform t in m_InterrestPoint)
        {
            float distance = Vector3.Distance(player.transform.position, t.position);
            sequence.Append(player.GetRigidbody().DOMove(t.position, distance / player.GetMovement().Stats.RunMaxSpeed)
                .SetEase(Ease.Linear))
                .AppendInterval(0.25f);
        }

        sequence.Play();
        yield return sequence.WaitForCompletion();
        StartCoroutine(EndAnim(player));
    }

    private IEnumerator EndAnim(Player player)
    {
        yield return new WaitForSeconds(0.5f);
        _sc.ShakyCameCustom(0.2f,0.2f);
        _breakFlorPart.Play();
        _sc.ShakyCameCustom(0.2f, 0.5f);

        yield return new WaitForSeconds(1.5f);
        _isLavaMoving = true;
        CinematicBand.Instance.ShowCinematicBand();
        yield return new WaitForSeconds(0.8f);
        _isLavaMoving = false;
        yield return new WaitForSeconds(0.5f);

        //TO DO : FIX
        //_bubbleEnd.SetActive(true);
        //yield return new WaitForSeconds(0.2f);
        //_ts.WriteMachinEffect("... ", _bubbletext, 1);
        //_ts.WriteMachinEffect("We don't forget this the next time", _bubbletext, 0.05f);
        //yield return new WaitForSeconds(7f);
        player.HasCompletedLevel = true;

    }
}
