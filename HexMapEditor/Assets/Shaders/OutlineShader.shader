Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ShapeLineWidth("ShapeWidth",float) = 0.1
        _Color("ShapeColor",COLOR) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "Queue" = "Geometry" }
            LOD 100

            //output stencil to define occlued area
            Pass
            {
                ColorMask 0
                ZTest Off
                Stencil
                {
                    Ref 1
                    Comp Always
                    Pass Replace
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                float _ShapeLineWidth;
                fixed4 _Color;

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                };


                v2f vert(appdata v)
                {
                    v2f o;
                    v.vertex.xyz -= v.vertex * _ShapeLineWidth;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(1,1,1,1);
                }
                ENDCG
            }

            //output outlinecolor
            Pass
            {
                Stencil
                {
                    Comp Equal
                }

                ZWrite Off
                ZTest Off
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag


                #include "UnityCG.cginc"

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                float _ShapeLineWidth;
                fixed4 _Color;

                v2f vert(appdata_base v)
                {
                    v2f o;
                    UNITY_INITIALIZE_OUTPUT(v2f, o);
                    v.vertex.xyz += v.vertex * _ShapeLineWidth;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                //[earlyDepthStencil]
                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = _Color;
                    return col;
                }
                ENDCG
            }
        }
}