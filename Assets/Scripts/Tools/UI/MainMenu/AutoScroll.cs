using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{

    public ScrollRect scrollRect;
    public RectTransform dropdownContent;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        dropdownContent = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        if (EventSystem.current.currentSelectedGameObject.transform.IsChildOf(dropdownContent))
        {
            RectTransform selected = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            if (selected != null)
            {
                EnsureVisible(selected);
            }
        }
    }

    private void EnsureVisible(RectTransform target)
    {
        // Get the position of the target in the ScrollRect's content
        Vector3[] itemCorners = new Vector3[4];
        target.GetWorldCorners(itemCorners);

        Vector3[] viewCorners = new Vector3[4];
        scrollRect.viewport.GetWorldCorners(viewCorners);

        // Check if the target is outside the visible area
        if (itemCorners[0].y < viewCorners[0].y) // Above the viewport
        {
            float delta = viewCorners[0].y - itemCorners[0].y;
            scrollRect.content.localPosition += new Vector3(0, delta, 0);
        }
        else if (itemCorners[2].y > viewCorners[2].y) // Below the viewport
        {
            float delta = itemCorners[2].y - viewCorners[2].y;
            scrollRect.content.localPosition -= new Vector3(0, delta, 0);
        }
    }
}
