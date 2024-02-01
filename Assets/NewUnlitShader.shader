Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Point0 ("Point0", Vector) = (0,0,0,0)
        _Point1 ("Point1", Vector) = (0,0,0,0)
        _Point2 ("Point2", Vector) = (0,0,0,0)
        _Point3 ("Point3", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            vector _Point0;
            vector _Point1;
            vector _Point2;
            vector _Point3;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2 beszier (float t)
            {
                return  (1-t)*(1-t)*(1-t) * _Point0 + 3 * (1-t)*(1-t) * t * _Point1 + 3 * (1-t) * t*t * _Point2 + t*t*t * _Point3;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 currentCoord = _MainTex_TexelSize * i.uv;
                // int bezierValue = beszier(currentUV.x);
                // if (bezierValue == currentUV.y)
                // {
                //     col = vector (0,0,0,1);
                // }

                col = vector(beszier(i.uv.x),0,1)
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
