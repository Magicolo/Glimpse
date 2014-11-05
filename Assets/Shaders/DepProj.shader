Shader "Custom/DepProj"
{ 
 Properties
 {
  _Color ("Main Colour", Color) = (1,1,1,1)
  _MainTex ("Projection Texture", 2D) = "gray" { TexGen ObjectLinear }
  _DepthTex ("Depth Texture", 2D) = "white"
  _MaxDepth ("Max Depth", float) = 1
  _MinDepth ("Min Depth", float) = 0
  _DepthTolerance ("Depth Tolerance", float) = 0.2
  _DepthMultiplier ("Depth Multiplier", float) = 12
  //_FalloffTex ("FallOff", 2D) = "white" { TexGen ObjectLinear   }
  //_far("FarClip", float) = 1
 }
 
 SubShader
 {
  Tags { "RenderType"="Background"  "Queue"="Background-100"}
  Pass
  {
   Lighting Off
   // Cull Off
   ZWrite Off
   Offset -1, -1
   
   Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
   //Fog { Mode Off }
   AlphaTest Greater 0
   ColorMask RGB
   Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma fragmentoption ARB_fog_exp2
   #pragma fragmentoption ARB_precision_hint_fastest
   #include "UnityCG.cginc"
 
   struct Input
   {
    float4 vertex : SV_POSITION;
    float3 normal : NORMAL;
    //float4 color : COLOR;
   };
   
   struct v2f
   {
    float4 pos : SV_POSITION;
    float4 uv_Main  : TEXCOORD0;
    float koef : COLOR;
    float projectionDepth : TEXCOORD1;
   };
 
   sampler2D _MainTex;
   sampler2D _DepthTex;
   float _MaxDepth;
   float _MinDepth;
   float _DepthTolerance;
   float _DepthMultiplier;
   //sampler2D _FalloffTex;
   float4 _Color;
   float4x4 _Projector;
   //float4x4 _ProjectorClip;
   //float _far;
 
   v2f vert(Input v)
   {
   v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv_Main = mul (_Projector, v.vertex);

    
    float3 normView = normalize(float3(_Projector[2][0],_Projector[2][1], _Projector[2][2]));
    float d = dot(v.normal, normView);
    
    o.koef = d < 0 ? 1 : 0;
    
    float4 depthCalc = (0,0,0,0);
    o.projectionDepth = distance(depthCalc, mul(_Projector, v.vertex));
    
    return o;
   }
 
   half4 frag (v2f i) : COLOR
   {
   	//UNITY_OUTPUT_DEPTH(i.depth);
   	
   	float depthMapValue =  tex2Dproj (_DepthTex, UNITY_PROJ_COORD(i.uv_Main)).r;
   	float depthProjectorValue =  (i.projectionDepth) / _DepthMultiplier;
    half4 tex = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.uv_Main)) * _Color * i.koef;
    //tex.r = depthValue;
    //tex.g = depthValue;
    //tex.b = depthValue;
    //tex.a = 1;

    if (depthProjectorValue < (depthMapValue - _DepthTolerance) || depthProjectorValue > (depthMapValue + _DepthTolerance)){
    	tex = (0,0,0,0);
    } 
    return tex;
   }
   ENDCG
  }
 }
}