using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class Constant
    {
        public static readonly string BEST_SCORE_KEY = "BEST_SCORE";
    }

    public class Component
    {
        public static T Get<T>(GameObject _gameObject)
        {
            TryGetInParent(_gameObject, out T _out);
            return _out;
        }
        public static bool TryGetInParent<T>(GameObject _gameObject, out T _out)
        {
            if (_gameObject.TryGetComponent(out T _out2))
            {
                _out = _out2;
                return true;
            }
            else if (_gameObject.transform.parent != null)
                return TryGetInParent(_gameObject.transform.parent.gameObject, out _out);
            else
            {
                _out = default;
                return false;
            }
        }

        public static T GetInParent<T>(Collider _gameObject)
        {
            TryGetInParent(_gameObject, out T _out);
            return _out;
        }
        public static bool TryGetInParent<T>(Collider _gameObject, out T _out)
        {
            return TryGetInParent(_gameObject.gameObject, out _out);
        }

        public static bool TryGetInChild<T>(GameObject _gameObject, out T _out, int index)
        {
            if (_gameObject.TryGetComponent(out T _out2))
            {
                _out = _out2;
                return true;
            }
            else if (_gameObject.transform.childCount >= index - 1)
                return TryGetInChild(_gameObject.transform.GetChild(index).gameObject, out _out, index);
            else
            {
                _out = default;
                return false;
            }
        }
    }

    public class DRayCast
    {
        public static List<Collider> Cone(Vector3 origin, Vector3 direction, float angle, float maxDistance, int numRays, LayerMask layerHit)
        {
            List<Collider> allhits = new();
            for (int i = 0; i < numRays; i++)
            {
                float currentAngle = Mathf.Lerp(-angle / 2, angle / 2, i / (float)(numRays - 1));
                Vector3 rayDirection = Quaternion.Euler(0, 0, currentAngle) * direction;

                if (Physics.Raycast(origin, rayDirection, out RaycastHit hit, maxDistance, layerHit))
                {
                    //Debug.Log(hit.collider.gameObject.name);
                    Debug.DrawRay(origin, rayDirection * hit.distance, Color.green, 0.1f);
                    allhits.Add(hit.collider);
                }
                else
                {
                    Debug.DrawRay(origin, rayDirection * maxDistance, Color.red, 0.1f);
                }
            }
            return allhits;
        }
    }

    public class Layer
    {
        public static void SetNewLayerObject(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetNewLayerObject(child.gameObject, layer);
            }
        }
    }

    public class Anim
    {
        private static IEnumerator AnimationOnCurve(float time, Action<float> animation, AnimationCurve curve)
        {
            float currentTime = 0f;

            while (currentTime < time)
            {
                animation(curve.Evaluate(currentTime / time));
                currentTime += Time.deltaTime;
                yield return null;
            }

        }
        
        #region FadeIn/FadeOut
        public static IEnumerator FadeIn(float t, CanvasGroup c)
        {
            c.gameObject.SetActive(true);
            c.alpha = 0f;
            while (c.alpha < 1.0f)
            {
                c.alpha += Time.deltaTime / t;
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, CanvasGroup c)
        {
            c.alpha = 1f;
            while (c.alpha > 0.0f)
            {
                c.alpha -= Time.deltaTime / t;
                yield return null;
            }
            c.gameObject.SetActive(false);
        }
        #endregion

        #region Blink
        public static IEnumerator Blink(CanvasGroup obj, float time)
        {
            bool switchAnim = true;
            float endTime = Time.time + time;
            while (endTime > Time.time)
            {
                switchAnim = !switchAnim;
                obj.alpha = switchAnim ? 1f : 0f;
                yield return new WaitForSeconds(0.05f);
            }
            //To make sure the gameobject stay visible at the end of the animation
            obj.alpha = 1f;
        }
        #endregion

        #region MovesTo
        public static IEnumerator MoveUI(RectTransform o, Vector2 to, float time, AnimationCurve curve)
        {
            Vector2 from = o.anchoredPosition;
            yield return AnimationOnCurve(time, t => o.anchoredPosition = Vector2.Lerp(from, to, t), curve);
        }

        public static IEnumerator MoveObject(Transform o, Vector3 to, float time, AnimationCurve curve)
        {
            Vector3 from = o.position;
            yield return AnimationOnCurve(time, t => o.position = Vector3.Lerp(from, to, t), curve);
        }
        #endregion
    }
}
