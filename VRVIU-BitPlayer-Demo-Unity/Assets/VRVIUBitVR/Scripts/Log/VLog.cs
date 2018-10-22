//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using UnityEngine;

namespace Assets.VRVIUBitVR.Scripts.Log
{
    public class VLog
    {
        public const int LEVEL_INFO  = 0;
        public const int LEVEL_DEBUG = 1;
        public const int LEVEL_WARN  = 2;
        public const int LEVEL_ERROR = 3;
        public const int LEVEL_PANIC = 4;
        public static void assert(bool condition,string message,Object context) {
            Debug.Assert(condition,message,context);
        }

        public static void assert(bool condition, string message) {
            Debug.Assert(condition, message);
        }

        public static void assert(bool condition, Object context) {
            Debug.Assert(condition,context);
        }

        public static void assert(bool condition) {
            Debug.Assert(condition);
        }

        public static void log(int level, object message, Object context) {
            switch (level)
            {
                case LEVEL_INFO:
                    Debug.Log(message,context);
                    break;
                case LEVEL_DEBUG:
                    //Debug.Log(message, context);
                    break;
                case LEVEL_ERROR:
                    Debug.LogWarning(message, context);
                    break;
                case LEVEL_WARN:
                    Debug.LogWarning(message, context);
                    break;
                case LEVEL_PANIC:
                    Debug.LogWarning(message, context);
                    break;
                default:
                    break;
            }
        }

        public static void log(int level,object message) {
            switch (level)
            {
                case LEVEL_INFO:
                    Debug.Log(message);
                    break;
                case LEVEL_DEBUG:
                    Debug.Log(message);
                    break;
                case LEVEL_ERROR:
                    Debug.LogWarning(message);
                    break;
                case LEVEL_WARN:
                    Debug.LogWarning(message);
                    break;
                case LEVEL_PANIC:
                    Debug.LogWarning(message);
                    break;
                default:
                    break;
            }
        }

        public static void logFormat(int level,Object context, string format, params object[] args) {
            switch (level)
            {
                case LEVEL_INFO:
                    Debug.LogFormat(context, format,args);
                    break;
                case LEVEL_DEBUG:
                    Debug.LogFormat(context, format, args);
                    break;
                case LEVEL_ERROR:
                    Debug.LogErrorFormat(context, format, args);
                    break;
                case LEVEL_WARN:
                    Debug.LogWarningFormat(context, format, args);
                    break;
                case LEVEL_PANIC:
                    Debug.LogErrorFormat(context, format, args);
                    break;
                default:
                    break;
            }
        }

        public static void logFormat(int level, string format, params object[] args)
        {
            switch (level)
            {
                case LEVEL_INFO:
                    Debug.LogFormat(format, args);
                    break;
                case LEVEL_DEBUG:
                    Debug.LogFormat(format, args);
                    break;
                case LEVEL_ERROR:
                    Debug.LogErrorFormat(format, args);
                    break;
                case LEVEL_WARN:
                    Debug.LogWarningFormat(format, args);
                    break;
                case LEVEL_PANIC:
                    Debug.LogErrorFormat(format, args);
                    break;
                default:
                    break;
            }
        }
    }
}