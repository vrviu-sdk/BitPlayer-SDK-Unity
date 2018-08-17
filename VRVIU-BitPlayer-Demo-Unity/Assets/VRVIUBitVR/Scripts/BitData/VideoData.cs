//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using System;
using System.Collections.Generic;
[Serializable]
public class VideoInfo
    {
        public string vid;

        public string url;

        public string cover_url;

        public int format;

        public string title;

        public string desc;

        public int duration;

        public int isLocal = 0;//0:服务端，1：本地
         
        public int vaid;
        
        public int hfov;  //180或者360

        public int stereo; //0：未知；1：2D；2：3D左右；3：3D上下；4：3D右左；5：3D下上

        public int projection; //0：未知；1：ERP；2：FISHEYE；3：TROPIZED；4：FLAT
    }
[Serializable]
public class VideoList {
        public List<VideoInfo> array;
    }