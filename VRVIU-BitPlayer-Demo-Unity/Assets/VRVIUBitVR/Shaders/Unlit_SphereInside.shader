
// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "VRVIU/Unlit_SphereInside" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	// Non-lightmapped
	Pass {
		
        Cull Off
        Lighting off
        ZTest off
        Zwrite off
        Blend off

		SetTexture [_MainTex] { combine texture } 
	}
	
	
	

}
}



