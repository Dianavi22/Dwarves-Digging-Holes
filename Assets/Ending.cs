using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ending : MonoBehaviour
{

    private bool _isEnding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isEnding && other.CompareTag("Player"))
        {
            _isEnding = true;
            GameManager.Instance.SetScrollingSpeed(0);
            GameManager.Instance._eventManager.enabled = false;
            Invoke(nameof(EndingAnimation), 1f);
        }
    }

    private void EndingAnimation() {
        StartCoroutine(MovePlayersToEndPosition());
    }

    private IEnumerator MovePlayersToEndPosition() {
        Sequence sequence = DOTween.Sequence();
        foreach (Player player in GamePadsController.Instance.PlayerList)
        {
            player.GetRigidbody().isKinematic = true;
            float targetX = transform.position.x;
            float distance = Mathf.Abs(player.transform.position.x - targetX);
            float duration = distance / 5f;
            sequence.Append(player.GetRigidbody().DOMoveX(targetX, duration).SetEase(Ease.Linear))
            .AppendInterval(0.25f)
            .Append(player.GetRigidbody().DOMoveY(transform.position.y + 6, 2f).SetEase(Ease.OutQuad))
            .AppendInterval(0.25f);
        }
        sequence.Play();
        yield return sequence.WaitForCompletion();
    }
}
