using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ending : MonoBehaviour
{
    private bool _isEnding = false;

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
        float targetX = transform.position.x;
        float distance = Mathf.Abs(player.transform.position.x - targetX);
        float duration = distance / 5f;
        sequence.Append(player.GetRigidbody().DOMoveX(targetX, duration).SetEase(Ease.Linear))
            .AppendInterval(0.25f)
            .Append(player.GetRigidbody().DOMoveY(transform.position.y + 6, 2f).SetEase(Ease.OutQuad))
            .AppendInterval(0.25f);

        sequence.Play();
        yield return sequence.WaitForCompletion();
        player.HasCompletedLevel = true;
    }
}
