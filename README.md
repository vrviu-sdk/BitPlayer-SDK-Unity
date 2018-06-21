# BitPlayer-SDK-Unity

[![](https://img.shields.io/badge/Powered%20by-vrviu.com-brightgreen.svg)](https://vrviu.com)

## 版本历史
 20180607 V1.3 支持8K VR点播视频播放；   
 20180514 V1.2 优化SDK，避免了目前多数VR播放SDK由于渲染不当造成的眩晕感；   
 20180503 V1.1 支持4K VR点播和VR直播；   

## 功能说明
 支持最高8K分辨率的VR视频点播/直播功能。其中，VR视频点播支持播放本地或者云端的文件；VR直播支持相机实时推送的视频流。  
 
## 产品特点
**1. 播放器格式支持**：  
 支持常见的VR视频格式播放，同时支持播放[威尔云](http://master.vrviu.com)转码的视频。  
 威尔云是威尔视觉的高清VR云服务，能够提供VR视频的标准算法编转码服务，以及强大的[**FE编码**](https://www.vrviu.com/technology.html)服务。经测试，对于4K的VR视频，与标准算法相比，FE编码在同样清晰度的前提下能够节省最高70%的码率；与Facebook的Pyramid算法相比，FE编码也能进一步节省最高40%的码率。对于6K以上的VR视频，FE算法的压缩率表现比4K更好。

**2. 支持系统**：Android

**3. VR视频格式**：支持 **180度3D/360度2D/360度3D** 视频直播点播；  
 
**4. 支持传输协议**：RTMP, HTTP MP4, HTTP FLV, HLS, VRVIU-FE

**5. 音视频编码**：H.264, H.265, AAC  

**6. 最大分辨率**：8K

**7. 播放格式**：直播，点播

**8. 解码方式**：硬解，软解

**9. 支持平台**：ARMV7, ARM64, X86  

**10. 接口丰富**：提供播放器状态监听、屏幕常亮以及音量控制等接口  

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

### 8. 检查混淆
```proguard
-keep class com.viu.player.** { *; } 
```
## 账号鉴权参数表
 |参数|说明|是否必填|类型|
 |:---|:---|:---|:---|
 |AppId|分配给用户的ID，可通过 www.vrviu.com 填写表单或者联系客服申请|必填|String|
 |AccessKeyId|分配给用户的ID，可通过 www.vrviu.com 填写表单或者联系客服申请|必填|String|
 |BizId|分配给用户的ID，可通过 www.vrviu.com 填写表单或者联系客服申请|必填|String|
 |AccessKey|分配给用户的ID，可通过 www.vrviu.com 填写表单或者联系客服申请|必填|String

## 联系我们
 如果有技术问题咨询，请加入官方QQ群：136562408；   
 商务合作请电话：0755-86960615；邮箱：business@vrviu.com；或者至[官网](http://www.vrviu.com)"联系我们" 。  
 
