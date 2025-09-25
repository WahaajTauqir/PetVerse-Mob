using System.Collections;
using UnityEngine;

/// <summary>
/// A tiny helper that ensures coroutines can be started from anywhere even if
/// the caller's GameObject is inactive. It creates a single persistent GameObject
/// with this component and exposes a static StartRoutine method.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner instance;

    static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                // Look for an existing one in scene
                instance = FindObjectOfType<CoroutineRunner>();
                if (instance == null)
                {
                    var go = new GameObject("~CoroutineRunner");
                    // hide from hierarchy to keep things tidy in editor
                    go.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<CoroutineRunner>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Start a coroutine using the runner instance.
    /// </summary>
    public static Coroutine StartRoutine(IEnumerator routine)
    {
        if (routine == null) return null;
        return Instance.StartCoroutine(routine);
    }

    // Optional: allow stopping coroutines via runner if needed
    public static void StopRoutine(Coroutine c)
    {
        if (instance == null || c == null) return;
        instance.StopCoroutine(c);
    }
}
