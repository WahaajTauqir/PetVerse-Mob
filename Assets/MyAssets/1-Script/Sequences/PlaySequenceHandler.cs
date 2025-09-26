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

    [SerializeField] GameObject catchPoint;

    public void PlayRandomSequence()
    {
        if (sequenceA == null && sequenceB == null)
        {
            Debug.LogWarning("No PlayableDirectors assigned for random play.");
            return;
        }

        sequenceA?.Stop();
        sequenceB?.Stop();

        PlayableDirector pick = (Random.value > 0.5f) ? sequenceA : sequenceB;

        if (pick == null) pick = (sequenceA != null) ? sequenceA : sequenceB;

        if (lastPlayed != null && sequenceA != null && sequenceB != null && pick == lastPlayed)
        {
            pick = (pick == sequenceA) ? sequenceB : sequenceA;
        }

        if (pick != null)
        {
            lastPlayed = pick;

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            pick.gameObject.SetActive(true);
            pick.enabled = true;

            pick.stopped -= OnDirectorStopped;
            pick.stopped += OnDirectorStopped;

            StartCoroutine(PlayDirectorNextFrame(pick));
        }
        else
        {
        }
    }


    void OnDirectorStopped(PlayableDirector dir)
    {
        if (dir == null) return;
        dir.stopped -= OnDirectorStopped;

    }

    System.Collections.IEnumerator PlayDirectorNextFrame(PlayableDirector dir)
    {
        if (dir == null) yield break;

        yield return null;

        try
        {
            dir.time = 0;
            dir.Evaluate();
        }
        catch { }

        dir.Play();
    }

    public void ActivateObject()
    {
        if (mouthObject == null)
        {
            Debug.LogWarning("ActivateObject called but mouthObject is null.");
            return;
        }

        // Move to catch point position before activating
        if (catchPoint != null)
        {
            ballRb.isKinematic = true;
            ballRb.useGravity = false;
            mouthObject.transform.position = catchPoint.transform.position;
            mouthObject.transform.rotation = catchPoint.transform.rotation;
                        mouthObject.transform.parent = catchPoint.transform; // Set parent to catchPoint
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
        mouthObject.SetActive(false);
    }

    public void ActivateObject1()
    {
        if (mouthObject1 == null)
        {
            return;
        }

        // Move to catch point position before activating
        if (catchPoint != null)
        {
            ballRb.isKinematic = true;
            ballRb.useGravity = false;

            mouthObject1.transform.position = catchPoint.transform.position;
            mouthObject1.transform.rotation = catchPoint.transform.rotation;
            mouthObject1.transform.parent = catchPoint.transform; // Set parent to catchPoint
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
        mouthObject1.SetActive(false);
    }
}
