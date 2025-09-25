using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

public class PlaySequenceHandler : MonoBehaviour
{
    [SerializeField] GameObject mouthObject;
    [SerializeField] Rigidbody ballRb;
    [SerializeField] UnityEvent onRightSideClicked;
    [SerializeField] UnityEvent onLeftSideClicked;

    [SerializeField] GameObject mouthObject1;
    [SerializeField] Rigidbody ballRb1;
    [SerializeField] PlayableDirector sequenceA;
    [SerializeField] PlayableDirector sequenceB;
        PlayableDirector lastPlayed;

// ...existing code...
     public void PlayRandomSequence()
    {
        if (sequenceA == null && sequenceB == null)
        {
            Debug.LogWarning("No PlayableDirectors assigned for random play.");
            return;
        }

        // stop both so we have a clean start
        sequenceA?.Stop();
        sequenceB?.Stop();

        // pick randomly
        PlayableDirector pick = (Random.value > 0.5f) ? sequenceA : sequenceB;

        // if chosen is null pick the other available one
        if (pick == null) pick = (sequenceA != null) ? sequenceA : sequenceB;

        // if both directors are available and we picked the same as last time, flip to the other
        if (lastPlayed != null && sequenceA != null && sequenceB != null && pick == lastPlayed)
        {
            pick = (pick == sequenceA) ? sequenceB : sequenceA;
        }

        if (pick != null)
        {
            Debug.Log("Playing random sequence: " + pick.name);

            // record last played
            lastPlayed = pick;

            // Ensure the director's GameObject is active and component enabled
            pick.gameObject.SetActive(true);
            pick.enabled = true;

            // subscribe to stopped for cleanup (safe double-subscribe handling)
            pick.stopped -= OnDirectorStopped;
            pick.stopped += OnDirectorStopped;

            // Start playing next frame to allow Unity to initialize bindings when object was inactive
            StartCoroutine(PlayDirectorNextFrame(pick));
        }
        else
        {
            Debug.LogWarning("PlayRandomSequence failed: both directors are null.");
        }
    }

    // called when a PlayableDirector finishes playing
    void OnDirectorStopped(PlayableDirector dir)
    {
        if (dir == null) return;
        dir.stopped -= OnDirectorStopped;
        // optional: disable director GameObject again if you want
        // dir.gameObject.SetActive(false);
    }

    System.Collections.IEnumerator PlayDirectorNextFrame(PlayableDirector dir)
    {
        if (dir == null) yield break;

        // Wait one frame so Unity initializes bindings if the director or targets were inactive
        yield return null;

        // ensure director starts from beginning
        try
        {
            dir.time = 0;
            dir.Evaluate();
        }
        catch { /* Evaluate may throw on some versions; ignore safely */ }

        dir.Play();
    }
// ...existing code...
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
