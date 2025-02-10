using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ending : MonoBehaviour
{
    private bool _isEnding = false;

    [SerializeField] private List<Transform> m_InterrestPoint = new ();

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Player>(other, out var player))
        {
            CinematicBand.Instance.ShowCinematicBand();
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
        player.HasCompletedLevel = true;
    }
}
