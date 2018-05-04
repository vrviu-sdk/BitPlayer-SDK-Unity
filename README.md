# VRVIU-BitPlayer-Demo-Unity

[![](https://img.shields.io/badge/Powered%20by-vrviu.com-brightgreen.svg)](https://vrviu.com)

## 版本
V1.1

## 功能说明
支持VR点播以及VR直播功能。VR点播是播放云端或者本地的文件；VR直播可以是网络主播实时推送的视频流，用户使用此播放器能够看到主播直播的画面。

## 产品特点
* **播放器格式支持**：可以支持常见视频格式播放，也可以播放威尔云转码的视频，其中威尔云[**FE编码算法**](https://www.vrviu.com/technology.html)在同样清晰度的前提下有更高的压缩率。

* **支持系统**：Android/iOS/Web/PC

* **VR视频类别**：  
    支持**360度3D ERP**视频直播点播，支持左右、上下视频格式  
    支持**360度2D ERP**视频直播点播  
    支持**180度3D ERP**视频直播点播，支持左右、上下视频格式  
    支持**180度3D FISH-EYE**视频直播点播，支持左右、上下视频格式

* **支持传输协议**：RTMP, HTTP MP4, HTTP FLV, HLS, VRVIU-FE

* **音视频编码**：H.264, H.265, AAC

* **最大分辨率**：4K

* **播放模式**：直播，点播

* **解码方式**：硬解，软解

* **支持平台**：ARMV7, ARM64, X86

* **接口丰富**：提供播放器状态监听、屏幕常亮以及音量控制等接口

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

### 5. 预览demo scene

* 在Unity项目窗口，进入Assets > VRVIUBitVR > Demos > Scenes。打开Lobby场景
* 点击Play按钮。在Game窗口中可以看到Lobby
* 同时按下Alt键和鼠标左键，可以选择需要播放的视频类型
* 等待凝视点变红后会跳转到播放页面

### 6. 接口调用

##### 6.1 初始化
```c#
player.SetupPlayer(VideoData data, Account account);
```

##### 6.2 暂停点播播放
```c#
player.Pause();
```

##### 6.3点播播放时长
```c#
player.GetDuration();
```

##### 6.4点播播放进度
```c#
player.GetPlayPosition ();
```

##### 6.5点播跳转
```c#
player.SeekTo(int msec);
```

##### 6.6设置音量
```c#
player.SetVolume (float volume);
```

##### 6.7设置点播播放速度
```c#
player.SetSpeed(float speed);
```

##### 6.8获取点播播放速度
```c#
player.GetSpeed();
```

##### 6.9获取播放状态
```c#
player.GetPlayState();
```

##### 6.10结束点播（直播）播放
```c#
player.Release();
```

### 7. 检查混淆
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

## 商务合作
电话：0755-86960615

邮箱：business@vrviu.com
