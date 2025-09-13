// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Dummy Shader"
{
        
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }

	SubShader {

        Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"
            
			float4 _Tint;
            sampler2D _MainTex;

            struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

            struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators vert (VertexData v) {
                Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.uv = v.uv;
                return i;
			}

			float4 frag (Interpolators i) : SV_TARGET {
				return tex2D(_MainTex, i.uv) * _Tint;
			}

			ENDCG
		}	
	}
}
