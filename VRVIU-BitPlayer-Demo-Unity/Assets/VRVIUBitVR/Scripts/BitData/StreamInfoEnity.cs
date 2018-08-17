//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using System;
using System.Collections.Generic;
namespace VRVIU.BitVRPlayer.BitData
{
    [Serializable]
    public class StreamInfoEnity
    {
        public string version;
        public Ret ret;
#if UNITY
	public Format[] formats;
    public Mesh[] meshs;
#else
        public List<Format> formats;
        public List<MeshParams> meshs;
#endif
        public BasicInfo basic_info;

        public List<LiveUrls> live_urls;

        public string total_content;
    }

    [Serializable]
    public class Ret
    {
        public int code;
        public string msg;
    }

    [Serializable]
    public class BasicInfo
    {
        public string title;
        public string duration;
    }

    [Serializable]
    public class LiveUrls
    {
        public int layer;
        public int content_type;
        public List<Views> views;
    }

    [Serializable]
    public class Views
    {
        public long bit_rate;
        public int yaw;
        public int roll;
        public int pitch;
        public List<Urls> urls;
        public string fid;
    }

    [Serializable]
    public class Urls
    {
        public int priority;
        public string url;
    }

    [Serializable]
    public class Format
    {
        public int selected;
        public int id;
        public int vaid;
        public int algo_version;
        public int projection;
        public int stereo;
        public int hfov;
        public int tvid;
        public int evid;
        public string display_name;
    }

    [Serializable]
    public class MeshParams
    {
        public string url;
        public float accuracy;
    }
}