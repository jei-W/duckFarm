Shader "Custom/SelectedDuck"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		[HDR]_LineColor ("Outline Color", color) = (0,0,1,1)
		_OverColor ("Overay Color", color) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
		Lighting Off

		cull front
		zwrite off
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

			fixed4 _LineColor;

            v2f vert (appdata v)
            {
				float3 normal = UnityObjectToWorldNormal(v.normal);
                v2f o;
				v.vertex.xyz += v.normal.xyz * 0.1;
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return _LineColor;
            }
			ENDCG
		}

		cull back
		zwrite on
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };

            sampler2D _MainTex;
			fixed4 _OverColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color * 0.6 + _OverColor * 0.4;
            }
            ENDCG
        }
    }
}
