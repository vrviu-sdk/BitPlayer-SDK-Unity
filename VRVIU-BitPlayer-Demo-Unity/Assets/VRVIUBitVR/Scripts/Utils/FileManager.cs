//-----------------------------------------------------
//            VRVIU: BitPlayer-SDK-Unity
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using VRVIU.BitVRPlayer.BitData;
using VRVIU.BitVRPlayer.Utils;

namespace Assets.VRVIUBitVR.Utils
{
    class FileManager
    {
        private static FileManager instance = null;

        public enum SILVER_ERROR
        {
            SILVER_SUCCESS = 0,
            SILVER_ERROR_GENERAL = -1,
            SILVER_NOT_IMPLEMENTED = -2,
            SILVER_OUT_OF_MEMORY = 1000,
            SILVER_ALREADY_EXIST = 1001,
            SILVER_NOT_EXIST = 1002,
            SILVER_NOT_READY = 1003,
            SILVER_ERROR_INVALID_PARAMETER,
            SILVER_INVALID_STATE,
            SILVER_PARSE_CONFIG_FAIL,
            SILVER_UNHANDLED_EXCEPTION
        }

        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetViuDescription(String pVr1Path, StringBuilder desc, int size);

        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverExtractVr1SubFile(String pVr1Path, String pFileName, String pDstPath);

        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetVr1RootDirectory(String pVr1Path, StringBuilder desc, int rootDirBufSize);

        public static FileManager getInstance()
        {
            if (instance == null)
            {
                instance = new FileManager();
            }
            return instance;
        }
        
        /***
         * 读取指定文件的视频信息
         * **/
        public VideoInfo GetLocalFileVideoInfo(string filePath) {
            VideoInfo videoInfo = null;
            StringBuilder videoDescription = new StringBuilder(4096);
            silverGetViuDescription(filePath, videoDescription, 4 * 1024);
            if (videoDescription != null)
            {
                Debug.Log(filePath+" Get video desc :"+ videoDescription.ToString());
                videoInfo = JsonUtility.FromJson<VideoInfo>(videoDescription.ToString()); ;
            }
            else
            {
                Debug.Log("Get video desc failed!");
            }
            string tmp = null;
            int index = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                            index =  filePath.LastIndexOf("/");
                            if(index <= 0){
                                index = filePath.Length - 1;
                            }
                            tmp= filePath.Substring(index+1,filePath.Length - 4 - index -1);
#else
            index = filePath.LastIndexOf("\\");
            if (index < 1)
            {
                index = filePath.Length - 1;
            }
            tmp = filePath.Substring(index + 1, filePath.Length - 4 - index - 1);

#endif
            if (videoInfo != null)
            {

                StringBuilder videoPath = new StringBuilder(256);
                silverGetVr1RootDirectory(filePath, videoPath, 256);

                if (!string.IsNullOrEmpty(videoInfo.url) &&
                !videoInfo.url.Contains("https") && !videoInfo.url.Contains("http"))
                {
                    videoInfo.url = "http://localhost:7777/vr1/" + filePath + "/" + videoPath + "/" + videoInfo.url;
                }
                videoInfo.isLocal = 1;
            }
            return videoInfo;
        }

        /***
        * 读取指定文件夹后缀为vr1视频，过滤指定文件夹的视频
        * **/
        public List<VideoInfo> GetLocalFileList(List<string> paths)
        {
           
            List<VideoInfo> localList = new List<VideoInfo>();
            if (paths == null && paths.Count == 0)
            {
                Debug.Log("Local path is empty!");
                return null;
            }
            foreach (string path in paths)
            {
                List<FileInfo> files = GetLocalVr1Files(path);
                
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        // VLog.log(VLog.LEVEL_WARN, "Local file 3 " + str);
                        try
                        {
                            string coverPath = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                            coverPath = files[i].Directory+"/."+files[i].Name + ".jpg";
#else
                            coverPath = files[i].Directory + "\\." + files[i].Name + ".jpg";

#endif
                            if (!File.Exists(coverPath))
                            {
                                silverExtractVr1SubFile(files[i].FullName, ".jpg", coverPath);
                            }
                            StringBuilder videoDescription = new StringBuilder(4096);
                            silverGetViuDescription(files[i].FullName, videoDescription, 4 * 1024);
                            VideoInfo localVideoEnity = null;
                            if (videoDescription != null)
                            {
                                localVideoEnity = JsonUtility.FromJson<VideoInfo>(videoDescription.ToString()); ;
                            }
                            else
                            {
                                Debug.Log("Get video desc failed!");
                            }

                            if (localVideoEnity != null)
                            {
                                if (!string.IsNullOrEmpty(localVideoEnity.url) &&
                                !localVideoEnity.url.Contains("https") && !localVideoEnity.url.Contains("http"))
                                {
                                    localVideoEnity.url =   files[i].FullName ;
                                }
                                if (!string.IsNullOrEmpty(localVideoEnity.cover_url) &&
                                    !localVideoEnity.cover_url.Contains("https") && !localVideoEnity.cover_url.Contains("http"))
                                {
                                    localVideoEnity.cover_url = "file://" + coverPath;
                                }
                                localVideoEnity.isLocal = 1;
                                localList.Add(localVideoEnity);
                                
                            }
                        }
                        catch
                        {
                            Debug.Log( "Parse viu json failed!");
                        }
                        finally
                        {
                        }
                    }
                }
            }
            string json = GetVideoInfos();
            if (!string.IsNullOrEmpty(json))
            {
                try {
                    VideoList tmp = JsonUtility.FromJson<VideoList>("{ \"array\": " + json + "}");
                    if (tmp != null && tmp.array != null && tmp.array.Count > 0)
                    {
                        localList.AddRange(tmp.array);
                    }
                    else {
                        Debug.Log("No data scan");
                    }
                } catch {
                    Debug.Log("parse local list failed!!");
                }
            }
            return localList;
        }

        private T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
        private string GetVideoInfos()
        {
            return AndoridNativePlugin.Call_GetVideoInfos();
        }

        /**
         * 获取指定的路径下后缀为vr1的文件
         * **/
        private List<FileInfo> GetLocalVr1Files(string fullPath)
        {
            List<FileInfo> files = new List<FileInfo>();
            if (Directory.Exists(fullPath))
            {
                //VLog.log(VLog.LEVEL_WARN, "Local file exist");
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] fileInfos = direction.GetFiles("*.vr1", SearchOption.TopDirectoryOnly);
                if (fileInfos != null)
                {
                    files.AddRange(fileInfos);
                }
            }
            else
            {
                Debug.Log("Local file not exist");
            }
            return files;
        }
    }
}
