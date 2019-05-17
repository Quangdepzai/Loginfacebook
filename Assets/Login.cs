using Facebook.Unity;
using System;
using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    public object User_ID { get; private set; }
    public void LoginFacebookGame()
    {
        Debug.Log(("<color=green>Starting login by facebook.</color>"));
        try
        {
            if (!FB.IsInitialized)
            {
                Debug.Log("Start Init");
                FB.Init(OnInitComplete, OnHideUnity);
            }
            else
            {
                Debug.Log("Facebook Initialized, logging in");
                FB.ActivateApp();
                FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, this.HandleResult);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game shown: " + isGameShown);
    }
    private void OnInitComplete()
    {
        try
        {
            Debug.Log(string.Format(
              "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
              FB.IsLoggedIn,
              FB.IsInitialized));

            if (AccessToken.CurrentAccessToken != null)
            {
                Debug.Log(AccessToken.CurrentAccessToken.ToString());
                //AccessToken.CurrentAccessToken.ToJson();
                foreach (string perm in AccessToken.CurrentAccessToken.Permissions)
                {
                    Debug.Log(perm);
                }
                Debug.Log(AccessToken.CurrentAccessToken.TokenString.ToString());
                User_ID = AccessToken.CurrentAccessToken.UserId.ToString();
                AuthenticatewithFirebase(AccessToken.CurrentAccessToken.TokenString.ToString());
            }
            else
            {
                FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, this.HandleResult);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("error OnInitComplete()" + ex.ToString());
        }
    }
    private void CallFBLoginForPublish()
    {
        Debug.Log("<color=blue>LogInWithPublishPermissions:</color>");
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, this.HandleResult);
    }
    protected void HandleResult(ILoginResult result)
    {
        try
        {
            if (result == null)
            {
                Debug.Log("Result is null");
            }
            else
            {
                AuthenticatewithFirebase(result.ResultDictionary["access_token"].ToString());
                User_ID = result.ResultDictionary["user_id"].ToString();
                Debug.Log(User_ID.ToString());
                foreach (string perm in result.AccessToken.Permissions)
                {
                    Debug.Log(perm);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("<color=red>HandleResult error:</color>");
            Debug.Log("HandleResult :" + ex.ToString());
            this.CallFBLoginForPublish();
        }
    }
    public void AuthenticatewithFirebase(string accessToken)
    {
        try
        {
            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
           {
               if (task.IsCanceled)
               {
                   return;
               }
               if (task.IsFaulted)
               {
                   Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                   return;
               }
               Firebase.Auth.FirebaseUser newUser = task.Result;
               
               Debug.Log(string.Format("User signed in successfully: {0} ({1})",
               newUser.DisplayName, newUser.UserId));
               PlayerPrefs.SetString("uidSave", newUser.UserId);
               SceneManager.LoadScene(1);
           });
        }
        catch (Exception ex)
        {
            Debug.LogError("LOG LOGIN FACEBOOK" + ex.Message);
        }
    }
}
