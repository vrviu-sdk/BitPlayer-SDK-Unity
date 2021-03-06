﻿//-----------------------------------------------------
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2017 VRVIU Technologies Limited. 
//-----------------------------------------------------
namespace VRVIU.BitVRPlayer.BitVideo
{
    /// <summary>
    /// Original video source formats.
    /// </summary>
    public enum VideoFormat : int
    {
        OPT_UNKONW = 0,
        OPT_FLAT_MONO = 1,
        OPT_FLAT_LR = 2,
        OPT_FLAT_TB = 3,
        OPT_ERP_180_MONO,
        OPT_ERP_180_LR,
        OPT_ERP_180_TB,
        OPT_ERP_360_MONO,
        OPT_ERP_360_LR,
        OPT_ERP_360_TB,
        OPT_FISHEYE_MONO,
        OPT_FISHEYE_LR,
        OPT_FISHEYE_TB,
        OPT_TROPIZED_MONO,
        OPT_TROPIZED_TB,
        OPT_TROPIZED_LR
    }

    public enum VideoPorjection : int {
        OPT_UNKONW = 0,
        OPT_ERP = 1,
        OPT_FISHEYE = 2,
        OPT_TROPIZOED = 3,
        OPT_FLAT = 4
    }

    public enum VideoSteroType : int {
        OPT_UNKONW = 0,
        OPT_MONO = 1,
        OPT_STERO_LR = 2,
        OPT_STERO_TB = 3,
        OPT_STERO_RL = 4,
        OPT_STERO_BT = 5
    }

    public enum VideoHfov
    {
        UNKONW = 0,
        DEGREE_180 = 180,
        DEGREE_360 = 360
    }
}