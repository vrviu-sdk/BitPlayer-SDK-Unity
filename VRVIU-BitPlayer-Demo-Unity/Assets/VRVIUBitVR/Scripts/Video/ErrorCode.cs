using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------
//            VRVIU: VRVIU-VR-PLAYER
//            Author: kevin.zha@vrviu.com  
// Copyright © 2016-2018 VRVIU Technologies Limited. 
//-----------------------------------------------------

namespace VRVIU.BitVrPlayer.Video
{
	public class ErrorCode
	{
		public const int CODE_SUCCESS = 1000;
		public const int CODE_TOKEN_EXPIRED = 1001;
		public const int ERROR_SOURCE = 1002;
		public const int ERROR_RENDERER = 1003;
		public const int ERROR_UNEXPECTED = 1004;
		// SOURCE
		public const int ERROR_MEDIA_SOURCE = 1005;
		// RENDERER
		public const int ERROR_QUERYING_DECODERS = 2001;
		public const int ERROR_NO_SECURE_DECODER = 2002;
		public const int ERROR_NO_DECODER = 2003;
		public const int ERROR_INSTANTIATING_DECODER = 2004;
	}
}

