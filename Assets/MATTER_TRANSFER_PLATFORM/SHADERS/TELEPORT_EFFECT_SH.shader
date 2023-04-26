// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:33990,y:32542,varname:node_1,prsc:2|diff-11-RGB,emission-394-OUT,alpha-357-OUT;n:type:ShaderForge.SFN_Panner,id:4,x:31659,y:32526,varname:node_4,prsc:2,spu:-0.1,spv:-0.9;n:type:ShaderForge.SFN_Fresnel,id:5,x:32938,y:33499,varname:node_5,prsc:2|EXP-5410-OUT;n:type:ShaderForge.SFN_Tex2d,id:6,x:31867,y:32526,ptovrint:False,ptlb:Noise Texture,ptin:_NoiseTexture,varname:node_3934,prsc:2,tex:3f5802bf8070604439bda876b7c70e89,ntxv:0,isnm:False|UVIN-4-UVOUT;n:type:ShaderForge.SFN_Color,id:11,x:32727,y:32297,ptovrint:False,ptlb:Main Color,ptin:_MainColor,varname:node_8859,prsc:2,glob:False,c1:0.608564,c2:0.9073499,c3:0.9852941,c4:1;n:type:ShaderForge.SFN_Multiply,id:54,x:33610,y:32639,varname:node_54,prsc:2|A-405-OUT,B-145-OUT;n:type:ShaderForge.SFN_ValueProperty,id:59,x:32390,y:32504,ptovrint:False,ptlb:Emissive Multiply,ptin:_EmissiveMultiply,varname:node_6097,prsc:2,glob:False,v1:2;n:type:ShaderForge.SFN_VertexColor,id:83,x:32905,y:33308,varname:node_83,prsc:2;n:type:ShaderForge.SFN_Multiply,id:84,x:32961,y:32919,varname:node_84,prsc:2|A-5032-OUT,B-83-A;n:type:ShaderForge.SFN_Multiply,id:145,x:33451,y:32661,varname:node_145,prsc:2|A-11-RGB,B-9701-OUT;n:type:ShaderForge.SFN_OneMinus,id:164,x:33215,y:33502,varname:node_164,prsc:2|IN-5-OUT;n:type:ShaderForge.SFN_Vector1,id:185,x:32136,y:33560,varname:node_185,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Exp,id:284,x:32484,y:33532,varname:node_284,prsc:2,et:1|IN-185-OUT;n:type:ShaderForge.SFN_Multiply,id:357,x:33637,y:32856,varname:node_357,prsc:2|A-2571-OUT,B-164-OUT;n:type:ShaderForge.SFN_Color,id:393,x:33099,y:32389,ptovrint:False,ptlb:Mult Color,ptin:_MultColor,varname:node_3964,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:394,x:33779,y:32607,varname:node_394,prsc:2|A-393-RGB,B-54-OUT;n:type:ShaderForge.SFN_Exp,id:405,x:32634,y:32486,varname:node_405,prsc:2,et:0|IN-59-OUT;n:type:ShaderForge.SFN_Tex2d,id:2957,x:31757,y:32949,ptovrint:False,ptlb:Plasma Texture,ptin:_PlasmaTexture,varname:_NoiseTexture_copy,prsc:2,tex:91237ad0765b1cd40b869d17f1ed66bc,ntxv:0,isnm:False|UVIN-5929-UVOUT;n:type:ShaderForge.SFN_Panner,id:5929,x:31535,y:32917,varname:node_5929,prsc:2,spu:0.5,spv:0;n:type:ShaderForge.SFN_Multiply,id:6930,x:32194,y:32603,varname:node_6930,prsc:2|A-6-R,B-2957-R,C-8432-OUT;n:type:ShaderForge.SFN_Multiply,id:5410,x:32742,y:33499,varname:node_5410,prsc:2|A-185-OUT,B-284-OUT;n:type:ShaderForge.SFN_Tex2d,id:8139,x:32824,y:32593,ptovrint:False,ptlb:Ramp Texture,ptin:_RampTexture,varname:node_8139,prsc:2,tex:d6942042d022a294b928627866cd2c5f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2571,x:33088,y:32820,varname:node_2571,prsc:2|A-8139-R,B-84-OUT;n:type:ShaderForge.SFN_Tex2d,id:9708,x:32300,y:32876,ptovrint:False,ptlb:Lines_Texture,ptin:_Lines_Texture,varname:node_9708,prsc:2,tex:14aeaf35373ea2f42bd358f2633149b4,ntxv:0,isnm:False|UVIN-3367-UVOUT;n:type:ShaderForge.SFN_Panner,id:3367,x:32087,y:32850,varname:node_3367,prsc:2,spu:1,spv:1;n:type:ShaderForge.SFN_Add,id:3273,x:32493,y:32806,varname:node_3273,prsc:2|A-9708-R,B-6930-OUT;n:type:ShaderForge.SFN_Time,id:5888,x:31387,y:32650,varname:node_5888,prsc:2;n:type:ShaderForge.SFN_Sin,id:9490,x:31684,y:32690,varname:node_9490,prsc:2|IN-5888-T;n:type:ShaderForge.SFN_Add,id:9701,x:33289,y:32744,varname:node_9701,prsc:2|A-2571-OUT,B-3273-OUT;n:type:ShaderForge.SFN_Tex2d,id:358,x:32304,y:33166,ptovrint:False,ptlb:Lasers_Texture,ptin:_Lasers_Texture,varname:node_358,prsc:2,tex:7869a9eca99249b4eb33c5e2b7eb894d,ntxv:0,isnm:False|UVIN-3879-UVOUT;n:type:ShaderForge.SFN_Panner,id:3879,x:31919,y:33210,varname:node_3879,prsc:2,spu:0.4,spv:0;n:type:ShaderForge.SFN_Blend,id:1599,x:32523,y:33139,varname:node_1599,prsc:2,blmd:10,clmp:True|SRC-358-R,DST-3273-OUT;n:type:ShaderForge.SFN_Add,id:5032,x:32750,y:32939,varname:node_5032,prsc:2|A-3273-OUT,B-4967-OUT;n:type:ShaderForge.SFN_Clamp,id:8432,x:31985,y:32724,varname:node_8432,prsc:2|IN-9490-OUT,MIN-9945-OUT,MAX-3948-OUT;n:type:ShaderForge.SFN_Vector1,id:9945,x:31770,y:32841,varname:node_9945,prsc:2,v1:0.22;n:type:ShaderForge.SFN_Vector1,id:3948,x:31877,y:32902,varname:node_3948,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Add,id:4967,x:32708,y:33139,varname:node_4967,prsc:2|A-1599-OUT,B-1599-OUT;proporder:11-59-393-6-2957-8139-9708-358;pass:END;sub:END;*/

Shader "Shader Forge/TEST" {
    Properties {
        _MainColor ("Main Color", Color) = (0.608564,0.9073499,0.9852941,1)
        _EmissiveMultiply ("Emissive Multiply", Float ) = 2
        _MultColor ("Mult Color", Color) = (0.5,0.5,0.5,1)
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _PlasmaTexture ("Plasma Texture", 2D) = "white" {}
        _RampTexture ("Ramp Texture", 2D) = "white" {}
        _Lines_Texture ("Lines_Texture", 2D) = "white" {}
        _Lasers_Texture ("Lasers_Texture", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _NoiseTexture; uniform float4 _NoiseTexture_ST;
            uniform float4 _MainColor;
            uniform float _EmissiveMultiply;
            uniform float4 _MultColor;
            uniform sampler2D _PlasmaTexture; uniform float4 _PlasmaTexture_ST;
            uniform sampler2D _RampTexture; uniform float4 _RampTexture_ST;
            uniform sampler2D _Lines_Texture; uniform float4 _Lines_Texture_ST;
            uniform sampler2D _Lasers_Texture; uniform float4 _Lasers_Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * _MainColor.rgb;
////// Emissive:
                float4 _RampTexture_var = tex2D(_RampTexture,TRANSFORM_TEX(i.uv0, _RampTexture));
                float4 node_7256 = _Time + _TimeEditor;
                float2 node_3367 = (i.uv0+node_7256.g*float2(1,1));
                float4 _Lines_Texture_var = tex2D(_Lines_Texture,TRANSFORM_TEX(node_3367, _Lines_Texture));
                float2 node_4 = (i.uv0+node_7256.g*float2(-0.1,-0.9));
                float4 _NoiseTexture_var = tex2D(_NoiseTexture,TRANSFORM_TEX(node_4, _NoiseTexture));
                float2 node_5929 = (i.uv0+node_7256.g*float2(0.5,0));
                float4 _PlasmaTexture_var = tex2D(_PlasmaTexture,TRANSFORM_TEX(node_5929, _PlasmaTexture));
                float4 node_5888 = _Time + _TimeEditor;
                float node_3273 = (_Lines_Texture_var.r+(_NoiseTexture_var.r*_PlasmaTexture_var.r*clamp(sin(node_5888.g),0.22,0.4)));
                float2 node_3879 = (i.uv0+node_7256.g*float2(0.4,0));
                float4 _Lasers_Texture_var = tex2D(_Lasers_Texture,TRANSFORM_TEX(node_3879, _Lasers_Texture));
                float node_1599 = saturate(( node_3273 > 0.5 ? (1.0-(1.0-2.0*(node_3273-0.5))*(1.0-_Lasers_Texture_var.r)) : (2.0*node_3273*_Lasers_Texture_var.r) ));
                float node_2571 = (_RampTexture_var.r*((node_3273+(node_1599+node_1599))*i.vertexColor.a));
                float3 emissive = (_MultColor.rgb*(exp(_EmissiveMultiply)*(_MainColor.rgb*(node_2571+node_3273))));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                float node_185 = 0.2;
                return fixed4(finalColor,(node_2571*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),(node_185*exp2(node_185))))));
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _NoiseTexture; uniform float4 _NoiseTexture_ST;
            uniform float4 _MainColor;
            uniform float _EmissiveMultiply;
            uniform float4 _MultColor;
            uniform sampler2D _PlasmaTexture; uniform float4 _PlasmaTexture_ST;
            uniform sampler2D _RampTexture; uniform float4 _RampTexture_ST;
            uniform sampler2D _Lines_Texture; uniform float4 _Lines_Texture_ST;
            uniform sampler2D _Lasers_Texture; uniform float4 _Lasers_Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuse = directDiffuse * _MainColor.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                float4 _RampTexture_var = tex2D(_RampTexture,TRANSFORM_TEX(i.uv0, _RampTexture));
                float4 node_1932 = _Time + _TimeEditor;
                float2 node_3367 = (i.uv0+node_1932.g*float2(1,1));
                float4 _Lines_Texture_var = tex2D(_Lines_Texture,TRANSFORM_TEX(node_3367, _Lines_Texture));
                float2 node_4 = (i.uv0+node_1932.g*float2(-0.1,-0.9));
                float4 _NoiseTexture_var = tex2D(_NoiseTexture,TRANSFORM_TEX(node_4, _NoiseTexture));
                float2 node_5929 = (i.uv0+node_1932.g*float2(0.5,0));
                float4 _PlasmaTexture_var = tex2D(_PlasmaTexture,TRANSFORM_TEX(node_5929, _PlasmaTexture));
                float4 node_5888 = _Time + _TimeEditor;
                float node_3273 = (_Lines_Texture_var.r+(_NoiseTexture_var.r*_PlasmaTexture_var.r*clamp(sin(node_5888.g),0.22,0.4)));
                float2 node_3879 = (i.uv0+node_1932.g*float2(0.4,0));
                float4 _Lasers_Texture_var = tex2D(_Lasers_Texture,TRANSFORM_TEX(node_3879, _Lasers_Texture));
                float node_1599 = saturate(( node_3273 > 0.5 ? (1.0-(1.0-2.0*(node_3273-0.5))*(1.0-_Lasers_Texture_var.r)) : (2.0*node_3273*_Lasers_Texture_var.r) ));
                float node_2571 = (_RampTexture_var.r*((node_3273+(node_1599+node_1599))*i.vertexColor.a));
                float node_185 = 0.2;
                return fixed4(finalColor * (node_2571*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),(node_185*exp2(node_185))))),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
