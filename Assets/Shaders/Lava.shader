Shader "Custom/Lava"
{
    Properties
    {
        _MainTex ("Texture principale", 2D) = "white" {}
        _Color ("Couleur de base", Color) = (1,1,1,1)
        _WaveSpeed ("Vitesse des vagues", Float) = 1.0
        _WaveScale ("Échelle des vagues", Float) = 1.0
        _LightColor ("Couleur de la lumière", Color) = (1,1,1,1)
        _Shading ("Intensité de l'ombrage", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveSpeed;
            float _WaveScale;
            float4 _LightColor;
            float _Shading;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float time = _Time.y * _WaveSpeed;
                float wave = sin(v.vertex.y * _WaveScale + time) * 0.1;
                o.pos.y += wave;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(float3(0.5, 1, 0.5)); // Direction de la lumière
                float diff = max(dot(i.normal, lightDir), 0.0);
                float4 texColor = tex2D(_MainTex, i.uv);
                
                // Application d'une couleur de base avec un ombrage cartoon
                float3 shadedColor = texColor.rgb * _LightColor.rgb * (diff * _Shading + 0.2);
                
                return fixed4(shadedColor, texColor.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}