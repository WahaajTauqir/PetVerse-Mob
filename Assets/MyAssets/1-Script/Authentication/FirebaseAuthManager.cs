using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.Events;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DependencyStatus dependencyStatus;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    [Header("Registeration")]
    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;

    [SerializeField] GeneralManager gm;

    void Awake()
    {
        // Prevent running in Edit mode
        if (!Application.isPlaying) return;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            Debug.Log("Firebase dependencies status: " + dependencyStatus);
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies are available.");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

        Debug.Log(PlayerPrefs.GetString("FirebaseUserId", ""));
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        FirebaseDataManager dataManager = FindObjectOfType<FirebaseDataManager>();
        if (dataManager != null)
        {

        }

        Debug.Log("-------------------------- " + auth);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("auth =>" + auth);
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {

            }
        }
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError("Login failed: " + loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login failed: " + authError;

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage = "Invalid email format.";
                    break;
                case AuthError.WrongPassword:
                    failedMessage = "Incorrect password.";
                    break;
                case AuthError.MissingEmail:
                    failedMessage = "Email is required.";
                    break;
                case AuthError.MissingPassword:
                    failedMessage = "Password is required.";
                    break;
                default:
                    failedMessage = "Login failed: " + authError;
                    break;
            }

            Debug.LogError(failedMessage);
        }
        else
        {
            user = loginTask.Result.User;
            Debug.Log("Login successful: " + user.UserId + " - " + user.DisplayName);

            PlayerPrefs.SetString("FirebaseUserId", user.UserId);
            PlayerPrefs.Save();
            Debug.Log("Login successful: " + user.UserId + " - " + user.DisplayName);
        }
    }

    public void Register()
    {
        Debug.Log("auth is " + (auth == null ? "null" : "set"));

        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("Name is required.");
            yield break;
        }
        else if (email == "")
        {
            Debug.LogError("Email is required.");
            yield break;
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Passwords do not match.");
            yield break;
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError("Registration failed: " + registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration failed: " + authError;

                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage = "Invalid email format.";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage = "Incorrect password.";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage = "Email is required.";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage = "Password is required.";
                        break;
                    default:
                        failedMessage = "Registration failed: " + authError;
                        break;
                }

                Debug.LogError(failedMessage);
            }
            else
            {
                if (registerTask.Result != null && registerTask.Result.User != null)
                {
                    user = registerTask.Result.User;
                    Debug.Log("Registration successful: " + user.UserId + " - " + user.DisplayName);

                    PlayerPrefs.SetString("FirebaseUserId", user.UserId);
                    PlayerPrefs.Save();
                    Debug.Log("Registration successful: " + user.UserId + " - " + user.DisplayName);

                    UserProfile userProfile = new UserProfile { DisplayName = name };
                    var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                    yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                    if (updateProfileTask.Exception != null)
                    {
                        user.DeleteAsync();
                        Debug.LogError("Failed to update user profile: " + updateProfileTask.Exception);

                        FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError authError = (AuthError)firebaseException.ErrorCode;

                        string failedMessage = "Failed to update user profile: " + authError;
                        switch (authError)
                        {
                            case AuthError.InvalidEmail:
                                failedMessage = "Invalid email format.";
                                break;
                            case AuthError.WrongPassword:
                                failedMessage = "Incorrect password.";
                                break;
                            case AuthError.MissingEmail:
                                failedMessage = "Email is required.";
                                break;
                            case AuthError.MissingPassword:
                                failedMessage = "Password is required.";
                                break;
                            default:
                                failedMessage = "Failed to update user profile: " + authError;
                                break;
                        }

                        Debug.LogError(failedMessage);
                    }
                    else
                    {
                        Debug.Log("User profile updated successfully: " + user.DisplayName);
                    }
                }
                else
                {
                    Debug.LogError("Registration failed: User object is null.");
                }
            }
        }

    }

    public string GetCurrentUserId()
    {
        if (user != null)
        {
            return user.UserId;
        }
        return PlayerPrefs.GetString("FirebaseUserId", "");
    }
}
