using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class PlaySequenceHandler : MonoBehaviour
{
    [SerializeField] GameObject mouthObject;
    [SerializeField] Rigidbody ballRb;
    [SerializeField] UnityEvent onRightSideClicked;
    [SerializeField] UnityEvent onLeftSideClicked;

    [SerializeField] GameObject mouthObject1;
    [SerializeField] Rigidbody ballRb1;

    public void ActivateObject()
    {
        if (mouthObject == null)
        {
            Debug.LogWarning("ActivateObject called but mouthObject is null.");
            return;
        }

        mouthObject.SetActive(true);
    }

    public void BallRelease()
    {
        if (ballRb == null)
        {
            Debug.LogWarning("BallRelease called but ballRb is null.");
            return;
        }

        if (mouthObject != null)
            mouthObject.transform.parent = null;

        ballRb.isKinematic = false;
        ballRb.useGravity = true;
    }

    public void DeleteBall()
    {
        ballRb1.gameObject.SetActive(false);
    }

        public void ActivateObject1()
    {
        if (mouthObject1 == null)
        {
            Debug.LogWarning("ActivateObject called but mouthObject is null.");
            return;
        }

        mouthObject1.SetActive(true);
    }

    public void BallRelease1()
    {
        if (ballRb1 == null)
        {
            Debug.LogWarning("BallRelease called but ballRb is null.");
            return;
        }

        if (mouthObject1 != null)
            mouthObject1.transform.parent = null;

        ballRb1.isKinematic = false;
        ballRb1.useGravity = true;
    }

    public void DeleteBall1()
    {
        ballRb1.gameObject.SetActive(false);
    }


    void Update()
    {
        // Support both touch (mobile) and mouse (editor/desktop)
        // Mouse / single-click
        if (Input.GetMouseButtonDown(0))
        {
            HandlePointer(Input.mousePosition);
        }

        // Touch input (first touch only)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                HandlePointer(t.position);
            }
        }
    }

    void HandlePointer(Vector2 screenPosition)
    {
        // Right half -> ActivateObject, Left half -> BallRelease
        if (screenPosition.x > (Screen.width * 0.5f))
        {
            OnRightSideClick();
        }
        else
        {
            OnLeftSideClick();
        }
    }

    // Public handlers that can also be called from UI Buttons or other scripts
    public void OnRightSideClick()
    {
        Debug.Log("Right side clicked");
        // default behavior
        ActivateObject();
        // designer hooks
        onRightSideClicked?.Invoke();
    }

    public void OnLeftSideClick()
    {
        Debug.Log("Left side clicked");
        // default behavior
        BallRelease();
        // designer hooks
        onLeftSideClicked?.Invoke();
    }

}
