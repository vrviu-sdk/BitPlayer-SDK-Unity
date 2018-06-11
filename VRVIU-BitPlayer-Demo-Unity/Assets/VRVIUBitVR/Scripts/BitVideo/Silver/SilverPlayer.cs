using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace VRVIU.BitVRPlayer.BitVideo.Silver
{
    public class SilverPlayer
    {
        enum SILVER_PLUGIN
        {
            INITIALIZE = 0,
            TERMINATE = 1,
            UPDATE = 2
        }

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
            SILVER_UNHANDLED_EXCEPTION,

            SILVER_NOT_PREPARE = 1100,
            SILVER_SEEK_OUT_OF_RANGE,

            // Fetcher
            SILVER_OPERATION_HAD_CANCEL = 2000,
            SILVER_NOT_FIND_STREAM_INFO,
            SILVER_PREPARE_TIMEOUT,
            SILVER_BUFFER_TIMEOUT,
            SILVER_M3U8_NOT_VALID,
            SILVER_M3U8_EMPTY_DATA,
            SILVER_M3U8_EMPTY_PLAYLIST,
            SILVER_RESPONSE_ALLOC_MULTIPLE,
            SILVER_CURL_GLOBAL_INIT_FAIL,
            SILVER_CURL_INIT_FAIL, //easy_init
            SILVER_PARSE_JSON_FAIL,
            SILVER_NETWORK_ERROR,
            SILVER_BAD_URL,
            SILVER_URL_NOT_EXIST,
            SILVER_GET_VIDEO_INFO_FAIL,
            SILVER_DOWNLOAD_MESH_FAIL,
            SILVER_DOWNLOAD_M3U8_FAIL,
            SILVER_DOWNLOAD_SEGMENT_FAIL,
            SILVER_DOWNLOAD_UNKNOWN_TASK,
            SILVER_UNREPORTED_ERROR,
            SILVER_UNRECOGNIZED_STREAM_TYPE,
            SILVER_UNRECOGNIZED_SEGMENT_TYPE,

            // BufferManager
            SILVER_UNSUPPORTED_VIDEO_TYPE = 3000,
            SILVER_BUFFER_IS_FULL = 3001,
            SILVER_BUFFER_IS_EMPTY,
            SILVER_BUFFER_DEQUEUE_TIMEOUT,
            SILVER_BUFFER_INVALID_VIEW,
            SILVER_BUFFER_INVALID_SEGMENT_TYPE,

            // Decoder
            SILVER_DECODER_CANNOT_INITIALIZE = 4000,
            SILVER_DECODER_INVALID_STREAM,
            SILVER_DECODER_UNKNOWN_STREAM_TYPE,
            SILVER_DECODER_UNKNOWN_DECODER,
            SILVER_DECODER_NULL_TEXTURE_ERROR,
            SILVER_DECODER_UNKNOWN_ERROR,
            SILVER_DECODER_BUFFER_EMPTY,
            SILVER_DECODER_NOT_INITIALIZED,

            // Renderer
            SILVER_OPENGL_ERROR = 5000,
            SILVER_OPENGL_NOT_INITIALIZED,
            SILVER_OPENGL_SWITCHED_CONTEXT,
            SILVER_LOAD_MESH_ERROR,

            SILVER_RENDERER_CREATE_TEXTURE_CACHE_FAIL,

            SILVER_SDL_OPEN_AUDIO_DEVICE_ERROR,
            SILVER_SDL_AUDIO_DECODE_CALLBACK_NULL,

            SILVER_AUDIO_RENDER_NOT_INIT = 6000,

            // decoder
            SILVER_CODEC_NOT_FOUND = 7000,
            SILVER_CODEC_OPEN_FAILED,
            SILVER_CODEC_ERROR,

            // M3u8Parser

            // VideoInfo
            SILVER_PLAYLIST_NOT_FOUND = 9000,
            SILVER_UNKNOWN_CONTENT_TYPE,
            SILVER_UNSUPPORTED_CONTENT_TYPE,
            SILVER_INVALID_LAYER,

            // ISOBMFFParser
            SILVER_ISOBMFF_PARSE_FAIL = 10000,
            SILVER_ISOBMFF_NO_DATA
        }

        public enum SilverVideoType
        {
            eSilverEquirectangular = 0,
            eSilverMeshForm = 1,
            eSilverRegionInterest = 2,
            eSilverFoveated = 3,
            eNumVideoTypes
        }
        public enum SilverSourceFormat
        {
            eSilverSource_Mono = 0,
            eSilverSource_Stereo_L_R = 1,
            eSilverSource_Stereo_T_B = 2,
            eSilverSource_Stereo_R_L = 3,
            eSilverSource_Stereo_B_T = 4,
            eNumNumSourceFormats
        }
        public unsafe struct SilverConfiguration
        {
            public IntPtr javaVM;
            public string path;
        }

        public struct SilverOrientation
        {
            public float yaw;
            public float pitch;
            public float roll;
        };
        public enum SILVER_STATE
        {
            SILVER_STATE_UNKNOWN = 0,
            SILVER_STATE_IDLE,
            SILVER_STATE_PREPARING,
            SILVER_STATE_READY,
            SILVER_STATE_BUFFERING,
            SILVER_STATE_PLAYING,
            SILVER_STATE_ERROR,
            SILVER_STATE_END,
            SILVER_STATE_COMPLETE
        }

        public enum SilverEyeType
        {
            eSilverEyeTypeCenter = 0,
            eSilverEyeTypeLeft = 1 << 0,
            eSilverEyeTypeRight = 1 << 1,
            //eSilverEyeTypeAll = eSilverEyeTypeLeft | eSilverEyeTypeRight,
        };
        public struct SilverRender
        {
            public Matrix4x4 viewMatrix;
            public Matrix4x4 projectMatrix;
            public int x;
            public int y;
            public int width;
            public int height;

            public SilverOrientation headOrientation;
            public SilverEyeType eye;
        };

        public struct SilverTexture
        {
            public IntPtr pData;
            public IntPtr pDataU;
            public IntPtr pDataV;
            public Int32 width;
            public Int32 height;
            public Int32 format;
            public Int32 videoType;
            public Int64 timestamp;
            public SilverOrientation orientation;
        }
        private unsafe struct SilverMesh
        {
            public Int32 numPositions;
            public Int32 numCoords;
            public Int32 numNormals;
            public Int32 numIndices;
            public IntPtr positions;
            public IntPtr coords;
            public IntPtr normals;
            public IntPtr indices;
        }

        public struct SilverVideoInfo
        {
            public SILVER_STATE state;
            public bool bFrameReady;                        // Set true if new frame of data is available, but hasn't been used
            public Int32 iHeight;                            // Current height, in pixels, of video frame
            public Int32 iWidth;                             // Current width, in pixels, of video frame
            public Int32 iTimebaseNumerator;                 // Numerator of timebase, for 29.97fps, this is 1001 (number of ticks per frame, or sample)
            public Int32 iTimebaseDenominator;               // Denominator of timebase, for 29.97fps, this is typically 30,000 (number of ticks per second)
            public Int64 lDurationTicks;                     // Duration of video sequence, in timebase ticks
            public Int64 lPlayPositionTicks;                 // Position of video sequence, in timebase ticks
            public Int64 lPlayPositionUs;                    // Position of playback in microseconds
            public SilverSourceFormat sourceFormat;
            public SilverPixelFormat pixelFormat;
        }
        public enum SilverPixelFormat
        {
            PIX_FMT_NONE = -1,
            PIX_FMT_RGBA,      ///< packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
            PIX_FMT_NV12,      ///< planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
            PIX_FMT_YUV420P,   ///< planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)
        };

    

#if UNITY_IPHONE || UNITY_TVOS

        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGenerateMesh(MeshType type, MeshQuality quality, float density, out SilverMesh mesh);

        //[DllImport("__Internal")]
        //private static extern SILVER_ERROR silverGetExternalTexture(IntPtr s, out Int64 externalTexture);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverInitialize(out IntPtr player, ref SilverConfiguration Config);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverTerminate(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverUpdate(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverSetVideoUrl(IntPtr s, String url);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverPrepare(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverPlay(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetState(IntPtr s, out SILVER_STATE state);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverPause(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverStop(IntPtr s);
        //[DllImport("__Internal")]
        //private static extern SILVER_ERROR silverReset(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverSeekTo(IntPtr s, Int64 pos);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetCurrentSeekPosition(IntPtr s, out Int64 pos);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetDuration(IntPtr s, out Int64 pos);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverSetVolume(IntPtr s, float f);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverSetSpeed(IntPtr s, float f);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetTexture(IntPtr s, out SilverTexture Texture);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetVideoInfo(IntPtr s, out SilverVideoInfo VideoInfo);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetMesh(IntPtr s, out SilverMesh mesh);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverSetHeadOrientation(IntPtr s, ref SilverOrientation orientation);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetOrientation(IntPtr s, out SilverOrientation orientation, Int64 ulTimestamp);
        [DllImport("__Internal")]
        private static extern IntPtr silverGetUnityRenderCallback(IntPtr s);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetExternalTexture(IntPtr s, out Int64 externalTexture);
        [DllImport("__Internal")]
        private static extern IntPtr silverGetErrorString(SILVER_ERROR error);
        [DllImport("__Internal")]
        private static extern SILVER_ERROR silverGetError(IntPtr s);


        #else

        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverInitialize(out IntPtr player, ref SilverConfiguration Config);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverTerminate(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverUpdate(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSetVideoUrl(IntPtr s, String url);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverPrepare(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverPlay(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetState(IntPtr s, out SILVER_STATE state);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverPause(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverStop(IntPtr s);
       // [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
       // private static extern SILVER_ERROR silverReset(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSeekTo(IntPtr s, Int64 pos);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetCurrentSeekPosition(IntPtr s, out Int64 pos);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetDuration(IntPtr s, out Int64 pos);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSetVolume(IntPtr s, float f);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSetSpeed(IntPtr s, float f);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetTexture(IntPtr s, out SilverTexture Texture);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetVideoInfo(IntPtr s, out SilverVideoInfo VideoInfo);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetMesh(IntPtr s, out SilverMesh mesh);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSetHeadOrientation(IntPtr s, ref SilverOrientation orientation);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetOrientation(IntPtr s, out SilverOrientation orientation, Int64 ulTimestamp);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr silverGetUnityRenderCallback(IntPtr s);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetExternalTexture(IntPtr s, out Int64 externalTexture);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr silverGetErrorString(SILVER_ERROR error);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverSetReplay(IntPtr s, int replayCount);
        [DllImport("silver-sdk", CallingConvention = CallingConvention.Cdecl)]
        private static extern SILVER_ERROR silverGetError(IntPtr s);
        #endif

        //******************
        //* PLEASE NOTE: Inclusion of vrviu_base is ONLY temporary! This is necessary for ExoPlayer support & squishmesh
        //* This should not be included in final version!
        //******************
        public enum MeshType
        {
            eMeshTypeErp180 = 0,
            eMeshTypeFisheye180,
            eMeshTypeErp360,
            eMeshTypeTropized,
            eMeshTypeInvalid
        };

        public enum MeshQuality
        {
            eQ1, eQ2, eQ3, eQ4, eQ5, eQ6, eQInvalid
        };
#if UNITY_IPHONE || UNITY_TVOS



#else
        [DllImport("silver-sdk")]
        private static extern SILVER_ERROR silverGenerateMesh(MeshType type, MeshQuality quality, float density, out SilverMesh mesh);
        [DllImport("vrviu_base")]
        private static extern void vrviu_filter_set_version(int filterType);
#endif


#if UNITY_ANDROID && !UNITY_EDITOR
    [DllImport("javabridge", CallingConvention = CallingConvention.Cdecl)] private static extern IntPtr getJavaVM();
#endif

        private IntPtr m_Instance = IntPtr.Zero;
        private IntPtr m_PluginCallback = IntPtr.Zero;

        private Texture2D[] m_Textures = null;
        private SilverTexture m_TextureInfo = new SilverTexture();

        // @fn SILVER_ERROR silverInitialize(SilverPlayer** ppPlayer, SilverConfiguration* pConfiguration)
        // @brief Creates a new Silver Player instance
        // @param ppPlayer Pointer to player pointer object, receives resulting player instance pointer.
        // @param pConfiguration Configuration structure
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR Initialize()
        {
            if (m_Instance != IntPtr.Zero)
            {
                return SILVER_ERROR.SILVER_ALREADY_EXIST;
            }

            IntPtr javaVM = new IntPtr(0);
#if UNITY_ANDROID && !UNITY_EDITOR
        javaVM = getJavaVM();
#endif
            SilverConfiguration Configuration = new SilverConfiguration();
            Configuration.javaVM = javaVM;
            Configuration.path = Application.persistentDataPath;
            SILVER_ERROR Result = silverInitialize(out m_Instance, ref Configuration);
            m_TextureInfo.format = (int)SilverPixelFormat.PIX_FMT_NONE;
            m_PluginCallback = silverGetUnityRenderCallback(m_Instance);

            // Call plugin to allocate opengl texture.
            GL.IssuePluginEvent(m_PluginCallback, (int)SILVER_PLUGIN.INITIALIZE);

            return Result;
        }

        private void DeleteTextures()
        {
            if (m_Textures != null)
            {
                for (int i = 0; i < m_Textures.Length; i++)
                {
                    if (m_Textures[i] != Texture2D.blackTexture)
                    {
                        Texture2D.Destroy(m_Textures[i]);
                    }
                    m_Textures[i] = null;
                }
                m_Textures = null;
            }
        }


        //
        // @fn SILVER_ERROR silverTerminate(SilverPlayer*)
        // @brief Deletes an instance of the Silver Player
        // @param pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR Terminate()
        {
            SILVER_ERROR Result;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            Result = silverTerminate(m_Instance);
            DeleteTextures();
            GL.IssuePluginEvent(m_PluginCallback, (int)SILVER_PLUGIN.TERMINATE);
            m_Instance = IntPtr.Zero;
            return Result;
        }

        //
        // @fn SILVER_ERROR silverUpdate(SilverPlayer* p)
        // @brief Performs an update of the Silver Player state
        // @param pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR Update()
        {
            SILVER_ERROR Result;

            Result = silverUpdate(m_Instance);
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;

            if (m_PluginCallback != IntPtr.Zero)
            {
                GL.InvalidateState();
                // This will make sure opengl textures get updated. This should be called after the most
                // recent silverUpdate() call.
                GL.IssuePluginEvent(m_PluginCallback, (int)SILVER_PLUGIN.UPDATE);
            }
            return Result;
        }

        //
        // @fn SILVER_ERROR silverSetVideoUrl(SilverPlayer* p, const char* pUrl)
        // @brief Sets the video's url to use.
        // @param \pPlayer Pointer to player object
        // @param \pUrl - URI/URL of the video
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR SetVideoUrl(string URL)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverSetVideoUrl(m_Instance, URL);
        }

        //
        // @fn SILVER_ERROR silverPrepare(SilverPlayer* p)
        // @brief Sets the video player to start preparing for video sequence.
        // @param \pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR Prepare()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;

            return silverPrepare(m_Instance);
        }

        //
        // @fn SILVER_ERROR silverPlay(SilverPlayer* p)
        // @brief Starts or resumes a video sequence that is just prepared, or has been paused
        // @param \pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // On completion, expect that current state is SILVER_STATE_PREPARING
        public SILVER_ERROR Play()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverPlay(m_Instance);
        }

        //
        // @fn SILVER_ERROR silverPause(SilverPlayer* p)
        // @brief Stops a video sequence that is playing, but it can be resumed at later stage
        // @param \pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // On completion, expect that current state is SILVER_STATE_PLAYING
        public SILVER_ERROR Pause()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverPause(m_Instance);
        }


        //
        // @fn SILVER_ERROR silverStop(SilverPlayer* p)
        // @brief Stops a video sequence that is playing, it cannot be resumed
        // @param \pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // On completion, expect that current state is SILVER_STATE_IDLE
        public SILVER_ERROR Stop()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverStop(m_Instance);
        }

        // Internal helper function to convert silver mesh to unity mesh
        private static Mesh SilverMeshToUnityMesh(SilverMesh silverMesh)
        {
            Mesh unityMesh = new Mesh();

            float[] silverPositions = new float[silverMesh.numPositions * 3];
            float[] silverCoords = new float[silverMesh.numCoords * 2];
            float[] silverNormals = new float[silverMesh.numNormals * 3];
            int[] silverIndices = new int[silverMesh.numIndices];

            if (silverMesh.numPositions > 0) Marshal.Copy(silverMesh.positions, silverPositions, 0, silverMesh.numPositions * 3);
            if (silverMesh.numCoords > 0) Marshal.Copy(silverMesh.coords, silverCoords, 0, silverMesh.numCoords * 2);
            if (silverMesh.numNormals > 0) Marshal.Copy(silverMesh.normals, silverNormals, 0, silverMesh.numNormals * 3);
            if (silverMesh.numIndices > 0) Marshal.Copy(silverMesh.indices, silverIndices, 0, silverMesh.numIndices);

            Vector3[] unityPositions = new Vector3[silverMesh.numPositions];
            Vector2[] unityCoords = new Vector2[silverMesh.numCoords];
            Vector3[] unityNormals = new Vector3[silverMesh.numNormals];
            int[] unityIndices = new int[silverMesh.numIndices];

            for (int i = 0; i < silverMesh.numPositions; i++)
            {
                float x = silverPositions[i * 3 + 0];
                float y = silverPositions[i * 3 + 1];
                float z = -silverPositions[i * 3 + 2];
                Vector3 position = new Vector3(x, y, z);
                unityPositions[i] = position;
            }
            for (int i = 0; i < silverMesh.numCoords; i++)
            {
                float u = silverCoords[i * 2 + 0];
                float v = silverCoords[i * 2 + 1];
                Vector2 coord = new Vector2(u, v);
                unityCoords[i] = coord;
            }
            for (int i = 0; i < silverMesh.numNormals; i++)
            {
                float x = silverNormals[i * 3 + 0];
                float y = -silverNormals[i * 3 + 1];
                float z = silverNormals[i * 3 + 2];
                Vector3 normal = new Vector3(x, y, z);
                unityNormals[i] = normal;
            }
            for (int i = 0; i < silverMesh.numIndices; i++)
            {
                int index = silverIndices[i];
                unityIndices[i] = index;
            }
            unityMesh.normals = unityNormals;
            unityMesh.vertices = unityPositions;
            unityMesh.uv = unityCoords;
            unityMesh.triangles = unityIndices;
            unityMesh.RecalculateBounds();
            if (unityNormals.Length == 0)
            {
                unityMesh.RecalculateNormals();
            }

            return unityMesh;
        }

        //
        // @fn SILVER_ERROR SILVER_ERROR silverGetMesh(SilverPlayer* p, Mesh** pMesh )
        // @brief Return mesh orientation at a specific time
        // @param \pPlayer Pointer to player object
        // @param \pString Pointer to mesh structure
        public SILVER_ERROR GetMesh(out Mesh finalMesh)
        {
            SILVER_ERROR Result;
            finalMesh = null;
            if (m_Instance == IntPtr.Zero)
            {
                return SILVER_ERROR.SILVER_NOT_EXIST;
            }

            SilverMesh silverMesh = new SilverMesh();
            Result = silverGetMesh(m_Instance, out silverMesh);
            if (Result != SILVER_ERROR.SILVER_SUCCESS)
            {
                return Result;
            }

            finalMesh = SilverMeshToUnityMesh(silverMesh);
            return SILVER_ERROR.SILVER_SUCCESS;
        }

        //
        // @fn SILVER_ERROR silverSetHeadOrientation(SilverPlayer* p, SilverOrientation* pOrientation)
        // @brief Set head orientation
        // @param \pPlayer Pointer to player object
        // @param \pOrientation - Yaw, pitch and roll of current viewer ideal position (pitch=x, yaw=y, roll=z)
        public SILVER_ERROR SetHeadOrientation(Vector3 orientation)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            SilverOrientation newOrientation = new SilverOrientation();

            newOrientation.yaw = orientation.y;
            newOrientation.pitch = orientation.x;
            newOrientation.roll = orientation.z;
           // Debug.LogError("step 3 yaw " + newOrientation.yaw + " roll " + newOrientation.roll + " pitch " + newOrientation.pitch);
            return silverSetHeadOrientation(m_Instance, ref newOrientation);
        }

        //
        // @fn SILVER_ERROR SILVER_ERROR silverGetOrientation(SilverPlayer* p, SilverOrientation* pOrientation, int64_t ulTimestampUs)
        // @brief Return mesh orientation at a specific time
        // @param \pPlayer Pointer to player object
        // @param \pOrientation - Receives new orientation information
        // @param \pulTimestampUs - What frame to retrieve orientation for
        // Note: Time given, here, must coincide with the timestamp of the video frame about to be displayed.
        // Any delay of this, will cause brief stuttering during view transition
        public SILVER_ERROR GetOrientation(out Vector3 orientation, Int64 lTimestampUs)
        {
            SILVER_ERROR Result;
            if (m_Instance == IntPtr.Zero)
            {
                orientation = Vector3.zero;
                return SILVER_ERROR.SILVER_NOT_EXIST;
            }
            SilverOrientation newOrientation = new SilverOrientation();
            Result = silverGetOrientation(m_Instance, out newOrientation, lTimestampUs);
            orientation.x = newOrientation.pitch;
            orientation.y = newOrientation.yaw;
            orientation.z = newOrientation.roll;
//            Debug.LogError(" from silver  ori "+ orientation + " timeStampUs "+lTimestampUs);
            return Result;
        }
        //
        // @fn SILVER_ERROR silverReset(SilverPlayer* p)
        // @brief Reset player to pre-prepared state. Any error code is cleared.
        // @param \pPlayer Pointer to player object
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // On completion, expect that current state is SILVER_STATE_IDLE
       /*public SILVER_ERROR Reset()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverReset(m_Instance);
        }
        */
        //
        // @fn SILVER_ERROR silverReset(SilverPlayer* p)
        // @brief Seek to a position within the video stream.
        // @param \pPlayer Pointer to player object
        // @param \ulSeekUs time specified in microseconds
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // On completion, expect that current state is SILVER_STATE_PREPARING
        // If already at the correct position, the state will be SILVER_STATE_PLAYING
        public SILVER_ERROR SeekTo(Int64 lSeekUs)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverSeekTo(m_Instance, lSeekUs);
        }

        //
        // @fn SILVER_ERROR silverSetSpeed(SilverPlayer* p, float fSpeed )
        // @brief Seek to a position within the video stream.
        // @param \pPlayer Pointer to player object
        // @param \fSpeed Floating point value indicating playback speed
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // Possible range is 0..16
        public SILVER_ERROR SetSpeed(float fSpeed)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverSetSpeed(m_Instance, fSpeed);
        }

        //
        // @fn SILVER_ERROR silverSetVolume(SilverPlayer* p, float fVolume )
        // @brief Set playback audio volume to desired level
        // @param \pPlayer Pointer to player object
        // @param \fVolume Floating point value indicating playback volume
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // Possible range is 0..4
        public SILVER_ERROR SetVolume(float fVolume)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverSetVolume(m_Instance, fVolume);
        }

        //
        // @fn SILVER_ERROR silverGetPercentageBuffered(SilverPlayer* p, float* pPercentage)
        // @brief Return amount of buffering that has occurred
        // @param \pPlayer Pointer to player object
        // @param \pPercentage Floating point value indicating buffering percentage, can be 0..100
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR GetPercentageBuffered(out float Percentage)
        {
            Percentage = 0.0f;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return SILVER_ERROR.SILVER_SUCCESS;
        }

        //
        // @fn SILVER_ERROR silverGetState(SilverPlayer* p, SILVER_STATE* pState)
        // @brief Return current internal state of player
        // @param \pPlayer Pointer to player object
        // @param \pState enumeration that contains current state
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // Note:
        // currentState is only valid between calls to silverUpdate(), after calling silverUpdate()
        // you must retrieve new state.
        public SILVER_ERROR GetState(out SILVER_STATE State)
        {
            State = SILVER_STATE.SILVER_STATE_UNKNOWN;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverGetState(m_Instance, out State);
        }

        //
        // @fn SILVER_ERROR silverGetDuration(SilverPlayer* p, int64_t* pDurationUs)
        // @brief Return total duration of video sequence
        // @param \pPlayer Pointer to player object
        // @param \pDurationUs Duration, in microseconds
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // Note:
        // If it cannot be determined, such as Live stream, it is up to calling application to decide
        // what do display. Function with return 0 duration for Live stream.
        public SILVER_ERROR GetDuration(out Int64 DurationUs)
        {
            DurationUs = 0;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverGetDuration(m_Instance, out DurationUs);
        }

        //
        // @fn SILVER_ERROR silverGetCurrentSeekPosition(SilverPlayer* p, int64_t* pSeekPositionUs)
        // @brief Return current seek position of sequence
        // @param \pPlayer Pointer to player object
        // @param \pSeekPositionUs - Current position in microseconds
        // @return SILVER_SUCCESS on function success, other error code on failure.
        // Note:
        // If it cannot be determined, such as Live stream, it is up to calling application to decide
        // what do display. Function with return 0 duration for Live stream.
        public SILVER_ERROR GetCurrentSeekPosition(out Int64 SeekPositionUs)
        {
            SeekPositionUs = 0;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            SILVER_ERROR result = silverGetCurrentSeekPosition(m_Instance, out SeekPositionUs);
            return result;
        }

        //
        // @fn SILVER_ERROR silverGetVideoInfo(SilverPlayer* p, SilverVideoInfo * pVideoInfo)
        // @brief Return extended information on the video sequence currently loaded
        // @param \pPlayer Pointer to player object
        // @param \pVideoInfo - Where state information is stored
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR GetVideoInfo(out SilverVideoInfo VideoInfo)
        {
            VideoInfo = new SilverVideoInfo();
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            SILVER_ERROR Result = silverGetVideoInfo(m_Instance, out VideoInfo);
            return Result;
        }

        public SILVER_ERROR GetExternalTexture(out IntPtr externalTexture)
        {
            Int64 texId = 0;
            SILVER_ERROR Result = silverGetExternalTexture(m_Instance, out texId);
            externalTexture = new IntPtr(texId);
            return Result;
        }

        public SILVER_ERROR GetTexture(out IntPtr[] textureData, out Int64 lTimestamp, out Vector3 orientation, out bool rotateMesh)
        {
            double startTime = Time.realtimeSinceStartup;

            SilverTexture textureInfo = new SilverTexture();
            SILVER_ERROR Result = silverGetTexture(m_Instance, out textureInfo);
            if (Result != SILVER_ERROR.SILVER_SUCCESS || textureInfo.format == (int)SilverPixelFormat.PIX_FMT_NONE)
            {
                lTimestamp = 0;
                orientation = Vector3.zero;
                textureData = null;
                rotateMesh = false;
                return Result;
            }

            // Only trapezoid, or any P4 style mesh requires rotation
            rotateMesh = (textureInfo.videoType == (int)SilverVideoType.eSilverMeshForm);

            double afterPluginEvent = Time.realtimeSinceStartup;
            double afterSetup = Time.realtimeSinceStartup;

            m_TextureInfo = textureInfo;

            switch ((SilverPixelFormat)textureInfo.format)
            {
                case SilverPixelFormat.PIX_FMT_YUV420P:
                    {
                        textureData = new IntPtr[3];

                        textureData[0] = textureInfo.pData;
                        textureData[1] = textureInfo.pDataU;
                        textureData[2] = textureInfo.pDataV;
                        break;
                    }
                case SilverPixelFormat.PIX_FMT_NV12:
                    {
                        textureData = new IntPtr[2];

                        textureData[0] = textureInfo.pData;
                        textureData[1] = textureInfo.pDataU;
                        break;
                    }
                case SilverPixelFormat.PIX_FMT_RGBA:
                    {
                        textureData = new IntPtr[1];

                        textureData[0] = textureInfo.pData;
                        break;
                    }

                default:
                    textureData = null;
                    break;
            }
            double afterLoad = Time.realtimeSinceStartup;

            orientation.x = textureInfo.orientation.pitch;
            orientation.y = textureInfo.orientation.yaw;
            orientation.z = textureInfo.orientation.roll;

            lTimestamp = textureInfo.timestamp;
            double afterApply = Time.realtimeSinceStartup;

            int deltaTime = (int)((afterApply - startTime) * 1e6);
            int deltaPlugin = (int)((afterPluginEvent - startTime) * 1e6);
            int deltaSetup = (int)((afterSetup - afterPluginEvent) * 1e6);
            int deltaLoad = (int)((afterLoad - afterSetup) * 1e6);
            int deltaApply = (int)((afterApply - afterLoad) * 1e6);

            if (deltaTime > 5000)
            {
                // BW: Commented this out because, on android, plugin event was taking a long time; I think this is due to script synchronization, waiting
                // for the event to have been performed. This is desired behavior, I think.
                //Debug.LogWarning("GetTexture overrun: Plugin:"+deltaPlugin+", Setup:" + deltaSetup + ", Load:" + deltaLoad + ", apply:" + deltaApply);
            }
            return Result;
        }

        //
        // @fn SILVER_ERROR silverGetTimestamp(SilverPlayer* p, int64_t* pTimeStampUs)
        // @brief Return time stamp of the most recently fetched new frame 
        // @param \pPlayer Pointer to player object
        // @param \pTimeStampUs - Timestamp of last frame retrieved
        // @return SILVER_SUCCESS on function success, other error code on failure.
        public SILVER_ERROR GetTimestamp(out Int64 TimeStampUs)
        {
            TimeStampUs = 0;
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return SILVER_ERROR.SILVER_SUCCESS;
        }

        //
        // @fn SILVER_ERROR silverGetError(SilverPlayer* p)
        // @brief Return last error code
        // @param \pPlayer Pointer to player object
        // @return Last error that occurred within library
        public SILVER_ERROR GetError()
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverGetError(m_Instance);
        }

        //
        // @fn SILVER_ERROR silverGetErrorString(SilverPlayer* p)
        // @brief Return last error code
        // @param \pPlayer Pointer to player object
        // @param \pString Pointer to target buffer for error text
        // @param \maxSize Length of target buffer for error text
        // @return Extended information about the last error that occurred within library
        // If no error, retrurns zero length string.
        public string GetErrorString(SILVER_ERROR error)
        {
            IntPtr result;

            result = silverGetErrorString(error);
            return Marshal.PtrToStringAnsi(result);
        }
        //
        // @fn SILVER_ERROR silverSetReplay( int replayCount )
        // @brief Set auto-repeat count at end of video sequence
        // @param \pPlayer Pointer to player object
        // @param \replayCount Number of times to replay, 0=never, -1=always, >0=number of times
        // @return Return code
        /*
        public SILVER_ERROR SetReplay(int replayCount)
        {
            if (m_Instance == IntPtr.Zero) return SILVER_ERROR.SILVER_NOT_EXIST;
            return silverSetReplay(m_Instance,replayCount);
        }
        */
        //
        //-------------------------------------------------------------------------
        // This is a temporary only helper function; to allow external squish mesh
        // generation.
        // Input is a json formatted string, typically with following form:
        //  {
        //      squish : 
        //      {
        //          type : <uint>,
        //          quality : <uint>,
        //          density : <float>,
        //          mirror_uv : <bool>,
        //          mirror_normal: <bool>
        //      }
        //  }    
        [System.Serializable]
        private class squish_config
        {
            [System.Serializable]
            public struct Squish
            {
                public string type;
                public string quality;
                public float density;
            };
            public Squish squish;
        }

        public static Mesh ParseSquishMesh(string meshConfigString)
        {
            squish_config meshConfig = JsonUtility.FromJson<squish_config>(meshConfigString);

            SilverMesh mesh = new SilverMesh();
            MeshType meshType = MeshType.eMeshTypeInvalid;
            MeshQuality meshQuality = MeshQuality.eQInvalid;

            switch (meshConfig.squish.type.ToLower())
            {
                case "erp180":
                    meshType = MeshType.eMeshTypeErp180;
                    break;
                case "fisheye180":
                case "fisheye":
                    meshType = MeshType.eMeshTypeFisheye180;
                    break;
                case "erp360":
                case "erp":
                    meshType = MeshType.eMeshTypeErp360;
                    break;
                default:
                    meshType = (MeshType)int.Parse(meshConfig.squish.type);
                    break;
            }

            switch (meshConfig.squish.quality.ToLower())
            {
                case "eq1": meshQuality = MeshQuality.eQ1; break;
                case "eq2": meshQuality = MeshQuality.eQ2; break;
                case "eq3": meshQuality = MeshQuality.eQ3; break;
                case "eq4": meshQuality = MeshQuality.eQ4; break;
                case "eq5": meshQuality = MeshQuality.eQ5; break;
                case "eq6": meshQuality = MeshQuality.eQ6; break;
                default:
                    meshQuality = (MeshQuality)int.Parse(meshConfig.squish.quality);
                    break;
            }

            silverGenerateMesh(meshType, meshQuality, meshConfig.squish.density, out mesh);

            Mesh finalMesh = SilverMeshToUnityMesh(mesh);

            return finalMesh;
        }

        //-------------------------------------------------------------------------
        public static Mesh GenerateMesh(MeshType meshType, MeshQuality meshQuality, float density)
        {
            SilverMesh mesh = new SilverMesh();

            silverGenerateMesh(meshType, meshQuality, density, out mesh);

            Mesh finalMesh = SilverMeshToUnityMesh(mesh);

            return finalMesh;
        }
        //
        //-------------------------------------------------------------------------
        // This is a temporary only helper function; to allow setting of filter
        // type
        // Returns true if OK, false otherwise (invalid filterType)
        public static bool SetFilter(string filterType)
        {
            int filter = 0;
            if (filterType != null)
            {
                switch (filterType.ToLower())
                {
                    case "version-0.0": filter = 0; break;
                    case "version-1.0": filter = 10; break;
                    case "version-1.1": filter = 11; break;
                    case "version-1.2": filter = 12; break;
                    default:
                        return false;
                }
            }
#if UNITY_IPHONE || UNITY_TVOS
        
#else
            vrviu_filter_set_version(filter);
#endif

            return true;
        }
    }

}