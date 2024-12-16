using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class AnimSequence
{
    public static class Chariot
    {
        public static Sequence NearDeathSequenceVignette(Vignette vignette)
        {
            return DOTween.Sequence()
                .Append(
                    DOTween.To(
                        () => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0.5f,
                        0.2f
                    ).SetEase(Ease.InQuart)
                )
                .Append(
                    DOTween.To(
                        () => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0.35f,
                        0.2f
                    ).SetEase(Ease.OutQuad)
                )
                .Append(
                    DOTween.To(
                        () => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0.4f,
                        0.2f
                    ).SetEase(Ease.InQuad)
                )
                .Append(
                    DOTween.To(
                        () => vignette.intensity.value,
                        x => vignette.intensity.value = x,
                        0.35f,
                        0.2f
                    ).SetEase(Ease.OutQuad)
                );
        }
        public static Sequence NearDeathSequenceColorGrading(ColorGrading colorGrading)
        {
            return DOTween.Sequence().Append(DOTween.To(
                () => colorGrading.temperature.value,
                x => colorGrading.temperature.value = x,
                50f,
                1f
            ).SetEase(Ease.InQuart));
        }
    }
}