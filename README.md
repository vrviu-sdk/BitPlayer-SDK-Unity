# BitPlayer-SDK-Unity

[![](https://img.shields.io/badge/Powered%20by-vrviu.com-brightgreen.svg)](https://vrviu.com)

## 版本历史
 20181221 V2.9 增加错误回调接口；增加渲染模式切换接口；优化了左右眼摄像机挂载方式；修复了一些bug；  
 20181108 V2.8 优化.vr1文件播放性能；  
 20180930 V2.7 支持vr1文件名特殊字符播放；优化seek精度；修复一些bug；   
 20180921 V2.6 支持最新的8K编码优化算法。修复bug，包括循环播放失败，本地播放普通4K视频偶现黑屏等；   
 20180903 V2.5 支持获取下载速度；  
 20180831 V2.4 支持大于4G的vr1文件，优化3D视频的2D单眼播放模式；  
 20180823 V2.3 支持本地vr1文件名修改；  
 20180817 V2.2 增加清晰度选择，支持本地VR视频播放，支持8K vr1文件格式，修复部分bug；  
 20180726 V2.1 优化性能，底层库修改，增加巨幕支持接口；  
 20180712 V2.0 优化性能，支持设置vid播放，支持多UI图层；  
 20180607 V1.3 支持8K VR点播视频播放；  
 20180514 V1.2 优化SDK，避免了目前多数VR播放SDK由于渲染不当造成的眩晕感；  
 20180503 V1.1 支持4K VR点播和VR直播。

## 功能说明
 支持最高8K分辨率的VR视频网络点播/直播功能。其中，VR点播支持播放云端的视频；VR直播支持相机实时推送的视频流。  
 支持最高8K分辨率的VR视频本地播放功能。8K视频为vr1格式，可以通过[威尔云](http://master.vrviu.com)转码后下载获得。  
 支持最高4K分辨率，MP4格式的普通视频播放。  
 本SDK免费使用，帐号需要申请授权。  
 
## 产品特点
**1. 播放器格式支持**：  
 支持常见的VR视频格式播放，同时支持播放[威尔云](http://master.vrviu.com)转码的视频。威尔云转码的8K视频，支持在线和下载到本地播放，文件格式是vr1格式；  
 威尔云是威尔视觉的高清VR云服务，能够提供VR视频的标准算法编转码服务，以及强大的[**FE编码**](https://www.vrviu.com/technology.html)服务。经测试，对于4K的VR视频，与标准算法相比，FE编码在同样清晰度的前提下能够节省最高70%的码率；与Facebook的Pyramid算法相比，FE编码也能进一步节省最高40%的码率。对于6K以上的VR视频，FE算法的压缩率表现比4K更好。


**2. 支持系统**：Android

**3. 最大分辨率**：8192*8192@60fps  
 
**4. VR视频格式**：支持 **180度3D/360度2D/360度3D**

**5. 支持传输协议**：RTMP, HTTP MP4, HTTP FLV, HLS, VRVIU-FE

**6. 音视频编码**：H.264, H.265, AAC  

**7. 播放格式**：直播，点播

**8. 解码方式**：硬解，软解

**9. 支持平台**：ARMV7, ARM64, X86  

**10. 接口丰富**：提供播放器控制、状态监听、本地/网络视频播放、多清晰度切换等接口  

**11. 处理器要求**：骁龙820或者同等性能以上  


## 开发环境
Unity2017.2.2

## 导入工程
### 1. 开发准备
下载最新的demo、BitPlayerSDK_\*.unitypackage 和 GoogleVRForUnity_\*.unitypackage。  
如果希望支持Oculus，需要下载 OVRForUnity.unitypackage

### 2. 导入BitPlayerSDK_\*.unitypackage

* 打开Unity并创建一个新的3D项目
* 选择Assets > Import Package > Custom Package
* 选择BitPlayerSDK_\*.unitypackage
* 在导入包对话框中选择导入

### 3. 导入GoogleVRForUnity_\*.unitypacage

* 选择Assets > Import Package > Custom Package
* 选择GoogleVRForUnity_\*.unitypackage
* 在导入包对话框中选择导入

### 4. 配置构建设置和player settings

* 选择File > Build Settings
* 选择相应平台，例如选中Android或者iOS并点击Switch Platform
* 在Build Setting中点击Player Settings

  配置Player settings如下：

    Player Settings > XR Settings > Virtual Reality Support	Enabled  
    Player Settings > XR Settings > SDKs，点击添加Daydream和Cardboard或者Oculus  
    Player Settings > XR Setting > Minimum API Level  
    DayDream: Android7.0‘Nougat’(API level 24)或者更高  
    Cardboard: Android4.4’Kit Kat’(API level 19)或者更高
### 5. 添加图层
* 在播放场景中，增加图层RightEye和LeftEye
* 选中VideoManager->RightEye 设置Layer为RightEye
* 选中VideoManager->LeftEye 设置Layer为LeftEye
* 选中VideoManager->RightSide 设置Layer为RightEye
* 选中VideoManager->LeftSide 设置Layer为LeftEye
### 6. 预览demo scene

* 在Unity项目窗口，进入Assets > VRVIUBitVR > Demos > Scenes。打开Lobby场景
* 点击Play按钮。在Game窗口中可以看到Lobby
* 同时按下Alt键和鼠标左键，可以选择需要播放的视频类型
* 等待凝视点变红后会跳转到播放页面

### 第三方SDK接入威尔BitSDK时请注意
* 若第三方SDK已存在LeftEye/RightEye Camera，请禁用VideoManager->LeftEye/RightEye Camera对象；
* 并在VideoManager对应的Inspector面板，设置Video Formater脚本中LeftEye/RightEye Camera对象；
  如若未设置以上左右眼Camera对象，请确保第三方SDK中左右眼Camera命名为“LeftEye”和“RightEye”；
  两种摄像机设置方式任选其一，如果都不设置，会造成无法渲染的问题。
* 另请注意第三方SDK的LeftEye/RightEye Camera的Clipping Planes属性中Far值，默认值为5000，太小可能无法看到渲染画面。

### 7. 接口调用

##### 7.1 初始化
```c#
player.SetupPlayer(VideoData data, Account account);
```

##### 7.2 暂停点播播放
```c#
player.Pause();
```

##### 7.3点播播放时长
```c#
player.GetDuration();
```

##### 7.4点播播放进度
```c#
player.GetPlayPosition ();
```

##### 7.5点播跳转
```c#
player.SeekTo(int msec);
```

##### 7.6设置音量
```c#
player.SetVolume (float volume);
```

##### 7.7设置点播播放速度
```c#
player.SetSpeed(float speed);
```

##### 7.8获取点播播放速度
```c#
player.GetSpeed();
```

##### 7.9获取播放状态
```c#
player.GetPlayState();
```

##### 7.10结束点播（直播）播放
```c#
player.Release();
```

##### 7.11获取清晰度列表  
```c#
player.GetResolution()；  
```

##### 7.12 设置本地视频信息  
```c#
public void SetLocalVideoInfo(string url, int projection, int stereo, int hfov, Account account)；  
```
/**
 * url:			指定的vr1或者mp4文件路径
 * projection:  投影方式，eg: FISHEYE
 * stereo:		立体格式（eg: 2D/3D)
 * hfov: 		水平视角（eg:180°/360°）
 * account: 	鉴权信息  
 **/

##### 7.13 通过vid设置网络视频信息  
```c#
public void SetVid(string vid, int format, Account account)； 
```
/*
 *  eg: 播放网络视频、切换清晰度时可调用此接口完成
 * vid: 	视频ID
 * format: 	清晰度ID
 * account: 鉴权信息  
*/

##### 7.14 获取当前播放位置  
```c#
public int GetPlayPosition()； 
```

##### 7.15 获取网络下载速度  
```c#
public long GetNetWorkSpeed()； 
```

##### 7.16 错误回调  
```c#
public void OnError(int errorCode, int errorCodeExtra)； 
```
/*
 *  eg: 播放初始化时设置异常回调接口：mPlayer.OnVideoError += OnError;
 * errorCode: 错误码，用于异常分析和处理
 * errorCodeExtra: 拓展错误码
*/

##### 7.17 切换渲染模式  
```c#
public void UpdateRenderMode(VideoPorjection projection, VideoSteroType steroType, VideoHfov hfov)； 
```
/**
 * projection:  投影方式（eg: FISHEYE）
 * stereo:	立体格式（eg: 2D/3D)
 * hfov: 	水平视角（eg:180°/360°）  
 **/


### 8. 检查混淆
```proguard
-keep class com.viu.player.** { *; } 
```

## 播放本地视频
1. 拷贝vr1或mp4文件至Android 存储卡指定目录  
	   eg: /sdcard/Movies/viuvideos/Boxing_2d_default.vr1  
	   
2. 进入工程目录下VRVIUBitVRSample/BitDemo/Script/BitLobby/BitLobby.cs，
	   在public void goToVideoScene(GameObject tag)接口中，设置vi.url播放地址，
	   改为步骤1中存储vr1或mp4文件的路径即可。

## 账号鉴权参数表
 |参数|说明|是否必填|类型|
 |:---|:---|:---|:---|
 |AppId|分配给用户的ID，可发送邮件至 busniess@vrviu.com申请|必填|String|
 |AccessKeyId|分配给用户的ID，可发送邮件至 busniess@vrviu.com申请|必填|String|
 |BizId|分配给用户的ID，可发送邮件至 busniess@vrviu.com申请|必填|String|
 |AccessKey|分配给用户的ID，可发送邮件至 busniess@vrviu.com申请|必填|String
 
 邮件发送内容如下：  
 ```
 标题：公司名+申请威尔云播放SDK  
 ```
 正文 （请填写以下信息）：    
 ```
 公司名：  
 公司网址：  
 联系人姓名：  
 联系电话：  
 联系邮件：  
 
 申请授权使用的SDK：  
 使用用途简述：  
 对应的威尔云平台账户（如果没有，请至http://master.vrviu.com注册）：  
 ```
 
## 联系我们
 如果有技术问题咨询，请加入官方QQ群：136562408；   
 商务合作请电话：0755-86960615；邮箱：business@vrviu.com；或者至[官网](http://www.vrviu.com)"联系我们" 。  
 
