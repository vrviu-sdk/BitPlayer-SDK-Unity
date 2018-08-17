//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
namespace VRVIU.BitVRPlayer.Utils
{
    public class AndoridNativePlugin
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject javaObj = null;

	private static AndroidJavaObject GetJavaObject()
	{
	if (javaObj == null)
	{
	javaObj = new AndroidJavaObject("com.EasyMovieTexture.NativeAndroidPlugin");
	}

	return javaObj;
	}

    private static void Call_SetUnityActivity(){
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
	    GetJavaObject().Call("SetUnityActivity", jo);
    }

    public static string  Call_GetVideoInfos() {
        Call_SetUnityActivity();
        Debug.Log("Call_GetVideoInfos");
        return GetJavaObject().Call<String>("GetVideoInfos");
    }

#elif UNITY_EDITOR
        public static string Call_GetVideoInfos()
        {
            Debug.Log("Call_GetVideoInfos");
            return null;
        }
#elif UNITY_STANDALONE_OSX
     public static string  Call_GetVideoInfos() {
    Debug.Log("Call_GetVideoInfos");
        return null;
    }
#endif

    }
}
