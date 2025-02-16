using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using FMODUnity;

public class Ending : MonoBehaviour
{
    private bool _isEnding = false;
    [SerializeField] private List<Transform> m_InterrestPoint = new();

    [SerializeField] TMP_Text _bubbletext;
    [SerializeField] TypeSentence _ts;
    [SerializeField] GameObject _bubbleEnd;
    [SerializeField] TheEndObj _theEnd;
    [SerializeField] private GameObject _pointDirectionGoldWinPart;

    [SerializeField] private EventReference _lavaUpSound;
    [SerializeField] private EventReference _ladderClimbSound;
    [SerializeField] private EventReference _floorSound;


    private Vector3 _targetPosition = new Vector3(9.97000027f, 0.810000002f, -0.660000026f);
    private bool _isLavaMoving = false;

    private void Start()
    {
        _theEnd = FindAnyObjectByType<TheEndObj>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_theEnd.isDwarfUp && Utils.Component.TryGetInParent<Player>(other, out var player))
        {
            _theEnd.isDwarfUp = true;
            GameManager.Instance.SetScrollingSpeed(0);
            EventManager.Instance.enabled = false;
            StartCoroutine(MovePlayerToEndPosition(player));
        }
    }

    private void Update()
    {
        if (!_isEnding && null == GamePadsController.Instance.PlayerList.Find(p => p.HasCompletedLevel == false))
        {
            _isEnding = true;
            GameManager.Instance.isEnding = true;
            if (GameManager.Instance.isChariotWin)
            {
                _theEnd._levelCompleteText.text = "Congratulation !";
                _theEnd._levelCompleteText2.text = "You Win";
                StartCoroutine(_theEnd.wp.GoldCountWin());
            }
            else
            {
               _theEnd._levelCompleteText.text = "Next time, take gold before escape";
               _theEnd._levelCompleteText2.text = "Too bad";
                StartCoroutine(EndAnim());
            }

        }

        if (_isLavaMoving)
        {
            _theEnd._lavaFloorPart.transform.position = Vector3.Lerp(_theEnd._lavaFloorPart.transform.position, _targetPosition, 2 * Time.deltaTime);
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
        player.GetAnimator().SetBool("isClimbingLadder", true);
        Debug.Log("coucou");
        RuntimeManager.PlayOneShot(_ladderClimbSound);

        foreach (Transform t in m_InterrestPoint)
        {
            float distance = Vector3.Distance(player.transform.position, t.position);
            sequence.Append(player.GetRigidbody().DOMove(t.position, distance / player.GetMovement().Stats.RunMaxSpeed)
                .SetEase(Ease.Linear))
                .AppendInterval(0.25f);
        }

        sequence.Play();
        yield return sequence.WaitForCompletion();
        player.GetAnimator().SetBool("isClimbingLadder", false);
        player.HasCompletedLevel = true;

    }

    private IEnumerator EndAnim()
    {
        _theEnd._sc.ShakyCameCustom(0.2f, 0.2f);
        _theEnd._breakFlorPart.Play();
        RuntimeManager.PlayOneShot(_floorSound);
        yield return new WaitForSeconds(1.5f);
        _isLavaMoving = true;
        RuntimeManager.PlayOneShot(_lavaUpSound);
        CinematicBand.Instance.ShowCinematicBand();
        yield return new WaitForSeconds(0.8f);
        _isLavaMoving = false;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(GameManager.Instance.LevelComplete());


    }
}
