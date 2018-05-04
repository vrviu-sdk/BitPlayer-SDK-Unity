//-----------------------------------------------------
//            VRVIU: BitPlayer-SDK-Unity
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;
using VRVIU.BitVrPlayer.Data;
using Assets.VRVIUBitVR.Scripts.Data;

public class Lobby : MonoBehaviour {
    #region Inspector Settings
  
    [Header("User Settings")]
    public Transform playerHead;
    [SerializeField] private Transform gazePointer; //  
    [SerializeField] private Image progress;        //  
    #endregion // Inspector Settings

    private int distance = 200;
    private float processTime = 0f;
    private float activateTime = 0f;
    public float fixedProcessTime = 1f;
    public float fixedActivateTime = 0.5f;
    private bool isProcess = false;
    private bool isShowGazePointer = false;
    private bool isTriggerOnNew = false; 
    private RaycastHit prevHit;
    public static Account account = new Account();
    public static VideoData videoData = new VideoData();
    private static int backTimes = 0;
    // Use this for initialization
    void Start () {
        XRSettings.enabled = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                backTimes++;
                if (backTimes == 2)
                {
                    Application.Quit();
                }
            }
        }

        Vector3 fwd = playerHead.TransformDirection(Vector3.forward);

        //Debug.DrawLine(playerHead.position, gazePointer.transform.position, Color.red);
        Ray ray = new Ray(playerHead.position, playerHead.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            OnGazeProcessing(hitInfo, true);
            //Debug.DrawLine(playerHead.position, hitInfo.point, Color.black);
        }
        else
        {
            OnGazeProcessing(hitInfo, false);
            //Debug.DrawRay(playerHead.position, playerHead.forward * 200, Color.blue, 1f);
        }
    }

    private void OnGazeProcessing(RaycastHit hit, bool isHit)
    {
        if (isHit)
        {
            if (isTriggerOnNew && !hit.collider.gameObject.Equals(prevHit.collider.gameObject))
            {
                isProcess = false;
                progress.fillAmount = 0f;
                processTime = 0f;
                activateTime = 0f;
            }
            isTriggerOnNew = true;
            prevHit = hit;
            ShowGazePointer();
            OnUpdateGazePointer(hit);

            if (activateTime < fixedActivateTime)
            {
                activateTime += Time.deltaTime;
            }
            else
            {
                if (processTime < fixedProcessTime)
                {
                    processTime += Time.deltaTime;
                    progress.fillAmount = processTime / fixedProcessTime;
                }
                else if (!isProcess)
                {
                    progress.fillAmount = 1f;
                    isProcess = true;
                }
            }
            if (isProcess)
            {
                goToVideoScene(hit.collider.gameObject);
            }
        }
        else
        {
            ShowGazePointer();
            isTriggerOnNew = false;
            isProcess = false;
            progress.fillAmount = 0f;
            processTime = 0f;
            activateTime = 0f;
        }
    }

    // 注视点更新
    private void OnUpdateGazePointer(RaycastHit hit)
    {
        gazePointer.position = hit.point;
        float scl = Vector3.Distance(playerHead.position, hit.point);
        gazePointer.localScale = new Vector3(scl, scl, scl);
    }

    // 注视点显示
    private void ShowGazePointer()
    {
        if (gazePointer != null)
        {
            isShowGazePointer = true;
            gazePointer.gameObject.SetActive(true);
        }
    }

    //注视点隐藏
    private void HideGazePointer()
    {
        if (gazePointer != null)
        {
            isShowGazePointer = false;
            gazePointer.gameObject.SetActive(false);
        }
    }

    public void goToVideoScene(GameObject tag) {
        account.appId = "vrviu_altsdk";
        account.bizId = "altsdk_alpha";
        account.accessKey = "a2fe8f5e4767e6c3dca8beb9b410f17a";
        account.accessKeyId = "dcb0af5f194f410796452a1644132f03";
        if (tag.CompareTag("ERP Video"))
        {
            videoData.name = "test";
            videoData.format = (int)VideoFormat.Mono;
			videoData.url = "http://10.86.0.103/video/NewYear_8K_s30t120_erp_center_3kx3k.mp4";
            videoData.algorithmType = (int)Algorithm.NONE;
            videoData.formatDegree = 180;
            videoData.meshType = (int)MeshType.Erp;
            SceneManager.LoadScene("Video");
        }
        else if (tag.CompareTag("FE Video"))
        {
            videoData.name = "test";
            videoData.format = (int)VideoFormat.Mono;
            videoData.url = "http://10.86.0.103/Letin_fe_dancing_4M_v5_vbv3/JSL_4k50mb/JSL_4k50mb.hls";
            videoData.algorithmType = (int)Algorithm.FE;
            videoData.formatDegree = 360;
            videoData.meshType = (int)MeshType.Erp;
            SceneManager.LoadScene("Video");
        }
        else if (tag.CompareTag("Exit")) {
            Application.Quit();
        }
    }
}
