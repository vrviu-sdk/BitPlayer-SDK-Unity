//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
namespace VRVIU.BitVRPlayer.BitVideo
{
    public class NetAPIs
    {
        private static readonly string VERSION = "1.0";
        private static readonly string CONTENT_VERSION = "2.0";
        public static string getPlatForm() {
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    return "30201";
                    break;
                case RuntimePlatform.Android:
                    return "30101";
                    break;
                default:
                    return "30101";
                    break;
            }
        }

        public static readonly string SERVER_GET_VIDEO_INFO = "http://vv-api.vrviu.com/v1/getvinfo" + "?platform=" + getPlatForm() + "&version=" + VERSION+ "&content_version=" + CONTENT_VERSION;
        public static readonly string SERVER_GET_VIDEO_LIST = "http://10.86.1.147/videoList.json";

    }
}