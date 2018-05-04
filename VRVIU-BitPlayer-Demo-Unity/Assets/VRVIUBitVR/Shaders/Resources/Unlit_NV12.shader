//
// Note: This shader is to parse NV12 format textures; these are planar Y,UV textures
// where the UV components are packed in to a Unity 'RG16' texture.
Shader "Unlit/Unlit_NV12"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Y("Y", 2D) = "black" {}
        _UV("UV", 2D) = "gray" {}
    }

    // GLES 3.0 shader, with ESSL3 extension
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Lighting off
        ZTest off
        Zwrite off
        Blend off

        Pass
        {
            GLSLPROGRAM
            #extension GL_OES_EGL_image_external_essl3 : require
#ifdef VERTEX // here begins the vertex shader
            varying vec2 glFragment_uv;
            uniform mediump vec4 _MainTex_ST;

            void main() // all vertex shaders define a main() function
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                glFragment_uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            }

#endif // here ends the definition of the vertex shader

#ifdef FRAGMENT // here begins the fragment shader
            varying vec2 glFragment_uv;
            uniform samplerExternalOES _MainTex;

            void main() // all fragment shaders define a main() function
            {
                gl_FragColor = texture2D( _MainTex, glFragment_uv);
            }

#endif // here ends the definition of the fragment shader
    
            ENDGLSL
        }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Lighting off
        ZTest off
        Zwrite off
        Blend off

        // GLES 2.0 shader, without ESSL3 extension
        Pass
        {
            GLSLPROGRAM
            #extension GL_OES_EGL_image_external : require
#ifdef VERTEX // here begins the vertex shader
            varying vec2 glFragment_uv;
            uniform mediump vec4 _MainTex_ST;

            void main() // all vertex shaders define a main() function
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                glFragment_uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            }

#endif // here ends the definition of the vertex shader

#ifdef FRAGMENT // here begins the fragment shader
            varying vec2 glFragment_uv;
            uniform samplerExternalOES _MainTex;

            void main() // all fragment shaders define a main() function
            {
                gl_FragColor = texture2D( _MainTex, glFragment_uv);
            }

#endif // here ends the definition of the fragment shader
    
            ENDGLSL
        }
    }
    // Generic shader
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Lighting off
        ZTest off
        Zwrite off
        Blend off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"



            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Y;
            sampler2D _UV;
            float4 _MainTex_ST;
        
            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = UnityObjectToClipPos(float4(v.vertex.xyz, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            
            fixed4 frag (v2f i) : SV_Target
            {

                float y = (tex2D(_Y, i.uv).r - 0.0625)  *  1.1643;
                float2 uv = tex2D(_UV, i.uv).rg - float2(0.5,0.5);
    
                float r = clamp(y + 1.5958 * uv.y, 0.0, 1.0);
                float g = clamp(y - 0.39173 * uv.x - 0.81290 * uv.y, 0.0, 1.0);
                float b = clamp(y + 2.017 * uv.x, 0.0, 1.0);

                fixed4 col = fixed4(r, g, b, 1.0);

                return col;
            }
            ENDCG
        }
    }
}