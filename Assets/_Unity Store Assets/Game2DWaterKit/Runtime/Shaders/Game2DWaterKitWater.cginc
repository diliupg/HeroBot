#ifndef Game2D_WaterKit_Water_INCLUDED
#define Game2D_WaterKit_Water_INCLUDED

#define Is_Water2D_Refraction_Enabled defined(Water2D_Refraction)
#define Is_Water2D_Reflection_Enabled defined(Water2D_Reflection)
#define Is_Water2D_FakePerspective_Enabled defined(Water2D_FakePerspective) && defined(Water2D_Surface)

#define Is_Water2D_ReflectionFadeLinear_Enabled defined(Water2D_ReflectionFadeLinear)
#define Is_Water2D_ReflectionFadeExponentialTwo_Enabled defined(Water2D_ReflectionFadeExponentialTwo)
#define Is_Water2D_ReflectionFadeExponentialThree_Enabled defined(Water2D_ReflectionFadeExponentialThree)
#define Is_Water2D_ReflectionFadeExponentialFour_Enabled defined(Water2D_ReflectionFadeExponentialFour)
#define Is_Water2D_WaterTexture_Enabled defined(Water2D_WaterTexture)
#define Is_Water2D_WaterTextureSheet_Enabled defined(Water2D_WaterTextureSheet)
#define Is_Water2D_WaterTextureSheetWithLerp_Enabled defined(Water2D_WaterTextureSheetWithLerp)
#define Is_Water2D_WaterTextureScroll_Enabled defined(Water2D_WaterTextureScroll)
#define Is_Water2D_WaterTextureStretch_Enabled defined(Water2D_WaterTextureStretch)
#define Is_Water2D_WaterTextureStretchAutoX_Enabled defined(Water2D_WaterTextureStretchAutoX)
#define Is_Water2D_WaterTextureStretchAutoY_Enabled defined(Water2D_WaterTextureStretchAutoY)
#define Is_Water2D_WaterNoise_Enabled defined(Water2D_WaterNoise)
#define Is_Water2D_Surface_Enabled defined(Water2D_Surface)
#define Is_Water2D_SurfaceHasAbsoluteThickness_Enabled defined(Water2D_SurfaceHasAbsoluteThickness)
#define Is_Water2D_SurfaceColorGradient_Enabled defined(Water2D_SurfaceColorGradient)
#define Is_Water2D_SurfaceTexture_Enabled defined(Water2D_SurfaceTexture)
#define Is_Water2D_SurfaceTextureSheet_Enabled defined(Water2D_SurfaceTextureSheet)
#define Is_Water2D_SurfaceTextureSheetWithLerp_Enabled defined(Water2D_SurfaceTextureSheetWithLerp)
#define Is_Water2D_SurfaceTextureScroll_Enabled defined(Water2D_SurfaceTextureScroll)
#define Is_Water2D_SurfaceTextureStretch_Enabled defined(Water2D_SurfaceTextureStretch)
#define Is_Water2D_SurfaceTextureStretchAutoX_Enabled defined(Water2D_SurfaceTextureStretchAutoX)
#define Is_Water2D_SurfaceTextureStretchAutoY_Enabled defined(Water2D_SurfaceTextureStretchAutoY)
#define Is_Water2D_SurfaceNoise_Enabled defined(Water2D_SurfaceNoise)
#define Is_Water2D_ColorGradient_Enabled defined(Water2D_ColorGradient)
#define Is_Water2D_ApplyEmissionColor_Enabled defined(Water2D_ApplyEmissionColor)
#define Is_Water2D_ApplyTintColorBeforeTexture_Enabled defined(Water2D_ApplyTintColorBeforeTexture)

#define Water2D_HasWaterTextureSheet Is_Water2D_WaterTextureSheet_Enabled || Is_Water2D_WaterTextureSheetWithLerp_Enabled
#define Water2D_HasWaterTexture  Is_Water2D_WaterTexture_Enabled || Water2D_HasWaterTextureSheet
#define Water2D_HasSurfaceTextureSheet Is_Water2D_SurfaceTextureSheet_Enabled || Is_Water2D_SurfaceTextureSheetWithLerp_Enabled
#define Water2D_HasSurfaceTexture Is_Water2D_Surface_Enabled && (Is_Water2D_SurfaceTexture_Enabled || Water2D_HasSurfaceTextureSheet)

			CBUFFER_START(UnityPerObject)
				#if Is_Water2D_Refraction_Enabled || Is_Water2D_Reflection_Enabled || Is_Water2D_FakePerspective_Enabled
					#if Is_Water2D_Refraction_Enabled
						uniform sampler2D _RefractionTexture;
					#endif

					#if Is_Water2D_FakePerspective_Enabled
						uniform sampler2D _RefractionTexturePartiallySubmergedObjects;
					#endif

					#if Is_Water2D_Reflection_Enabled
						uniform sampler2D _ReflectionTexture;
						#if Is_Water2D_FakePerspective_Enabled
							uniform sampler2D _ReflectionTexturePartiallySubmergedObjects;
							uniform half _ReflectionFakePerspectiveLowerLimit;
							uniform half _ReflectionFakePerspectiveUpperLimit;
							uniform half _ReflectionFakePerspectivePartiallySubmergedObjectsUpperLimit;
						#endif
						uniform half _ReflectionLowerLimit;
					#endif

					uniform float4x4 _WaterMVP;
				#endif
				uniform float4 _AspectRatio;
				uniform float4 _Size;
			CBUFFER_END // UnityPerObject

			CBUFFER_START(UnityPerMaterial)
				#if Is_Water2D_Refraction_Enabled 
					half _RefractionNoiseSpeed;
					half _RefractionNoiseStrength;
					half4 _RefractionNoiseTiling;
					half _RefractionAmountOfBending;
				#endif

				#if Is_Water2D_Reflection_Enabled 
					half _ReflectionNoiseSpeed;
					half _ReflectionNoiseStrength;
					half4 _ReflectionNoiseTiling;
					half _ReflectionVisibility;
				#endif

				#if Is_Water2D_Surface_Enabled
					half _SurfaceLevel;
					#if Is_Water2D_SurfaceColorGradient_Enabled
					half4 _SurfaceColorGradientStart;
					half4 _SurfaceColorGradientEnd;
					half _SurfaceColorGradientOffset;
					#else
					half4 _SurfaceColor;
					#endif
					#if Water2D_HasSurfaceTexture
						sampler2D _SurfaceTexture;
						float4 _SurfaceTexture_ST;
						half _SurfaceTextureOpacity;

						#if Is_Water2D_SurfaceTextureScroll_Enabled
							half _SurfaceTextureScrollingSpeedX;
							half _SurfaceTextureScrollingSpeedY;
						#endif

						#if Is_Water2D_SurfaceNoise_Enabled
							half _SurfaceNoiseSpeed;
							half _SurfaceNoiseStrength;
							half4 _SurfaceNoiseTiling;
						#endif

						#if Water2D_HasSurfaceTextureSheet
							half _SurfaceTextureSheetFramesPerSecond;
							half _SurfaceTextureSheetFramesCount;
							half _SurfaceTextureSheetInverseColumns;
							half _SurfaceTextureSheetInverseRows;
						#endif
					#endif

					#if Is_Water2D_FakePerspective_Enabled
						half _SubmergeLevel;
					#endif
				#endif

				#if Is_Water2D_ColorGradient_Enabled
					half4 _WaterColorGradientStart;
					half4 _WaterColorGradientEnd;
					half _WaterColorGradientOffset;
				#else
					half4 _WaterColor;
				#endif

				#if Water2D_HasWaterTexture
					sampler2D _WaterTexture;
					float4 _WaterTexture_ST;
					half _WaterTextureOpacity;
					
					#if Is_Water2D_WaterTextureScroll_Enabled
						half _WaterTextureScrollingSpeedX;
						half _WaterTextureScrollingSpeedY;
					#endif

					#if Is_Water2D_WaterNoise_Enabled
						half _WaterNoiseSpeed;
						half _WaterNoiseStrength;
						half4 _WaterNoiseTiling;
					#endif

					#if Water2D_HasWaterTextureSheet
						half _WaterTextureSheetFramesPerSecond;
						half _WaterTextureSheetFramesCount;
						half _WaterTextureSheetInverseColumns;
						half _WaterTextureSheetInverseRows;
					#endif
				#endif

				#if Is_Water2D_ApplyEmissionColor_Enabled
					half3 _WaterEmissionColor;
					half _WaterEmissionColorIntensity;
				#endif
					
				#if Is_Water2D_Refraction_Enabled || Is_Water2D_Reflection_Enabled || (Water2D_HasWaterTexture && Is_Water2D_WaterNoise_Enabled) || (Is_Water2D_Surface_Enabled && Water2D_HasSurfaceTexture && Is_Water2D_SurfaceNoise_Enabled)
					sampler2D _NoiseTexture;
					float4 _NoiseTexture_ST;
				#endif

			CBUFFER_END // UnityPerMaterial
				
			struct Attributes
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
					float2 lightmapCoord : TEXCOORD1;
				#endif
			};

			struct Varyings
			{
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;

				#if Is_Water2D_Refraction_Enabled || Is_Water2D_Reflection_Enabled || Is_Water2D_FakePerspective_Enabled
					float2 refractionReflectionUV : TEXCOORD1;
				#endif

				#if Is_Water2D_Refraction_Enabled
					float2 refractionUV : TEXCOORD2;
				#endif

				#if Is_Water2D_Reflection_Enabled 
					float3 reflectionUV : TEXCOORD3;
				#endif

				#if Water2D_HasWaterTexture
					#if Is_Water2D_WaterNoise_Enabled
						float4 waterTextureUV : TEXCOORD4;
					#else
						float2 waterTextureUV : TEXCOORD4;
					#endif
				#endif

				#if Water2D_HasSurfaceTexture
					#if Is_Water2D_SurfaceNoise_Enabled
						float4 surfaceTextureUV : TEXCOORD5;
					#else
						float2 surfaceTextureUV : TEXCOORD5;
					#endif
				#endif

				#if defined(Game2DWaterKit_SRP_Lit)
					float2 lightingUV : TEXCOORD6;
				#else
					#if defined(LIGHTMAP_ON)
						float2 lightmapCoord : TEXCOORD6;
					#else
						#if defined(UNITY_SHOULD_SAMPLE_SH)
			 				half3 sh : TEXCOORD6;
						#endif
					#endif

					#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
  						float3 worldPos : TEXCOORD7;
 					#endif
				#endif

				#if !defined(Game2DWaterKit_SRP_Lit) && !defined(Game2DWaterKit_SRP_Unlit)
				UNITY_FOG_COORDS(8)
				#endif
				
				#if defined(Game2DWaterKit_VertexLit_Vertex) || defined(Game2DWaterKit_VertexLit_VertexLM) || defined(Game2DWaterKit_VertexLit_OnlyDirectional)
					half3 lightColor : COLOR0;
				#endif
			};
			
			#include "Game2DWaterKit.cginc"

			inline Varyings Water2D_Vert(Attributes v){
				Varyings o;
 				UNITY_INITIALIZE_OUTPUT(Varyings,o);
				
				o.pos = ComputeClipPosition(v.pos);

				o.uv.xy = v.uv;
				o.uv.z = v.pos;

				#if Is_Water2D_Refraction_Enabled || Is_Water2D_Reflection_Enabled || Is_Water2D_FakePerspective_Enabled
					float4 pos = mul(_WaterMVP,v.pos);
					o.refractionReflectionUV = (pos.xy / pos.w) * 0.5 + 0.5;
				#endif

				float2 vertexPositionWorldSpace = (mul(unity_ObjectToWorld, v.pos)).xy;

				#if Is_Water2D_Refraction_Enabled 
					o.refractionUV.xy = TRANSFORM_TEX((vertexPositionWorldSpace * _RefractionNoiseTiling.xy), _NoiseTexture) + fmod2(_G2DWK_Frame_Time.w * _RefractionNoiseSpeed);
				#endif

				#if Is_Water2D_Reflection_Enabled 
					o.reflectionUV.xy = TRANSFORM_TEX((vertexPositionWorldSpace * _ReflectionNoiseTiling.xy), _NoiseTexture) + fmod2(_G2DWK_Frame_Time.w * _ReflectionNoiseSpeed);
					o.reflectionUV.z = v.pos.y;
				#endif
				
				#if Is_Water2D_Surface_Enabled
					#if Is_Water2D_SurfaceHasAbsoluteThickness_Enabled
						float surfaceLevel = (_Size.y - _SurfaceLevel) * _Size.w;
					#else
						float surfaceLevel = _SurfaceLevel;
					#endif
				#endif

				#if Is_Water2D_Surface_Enabled && Water2D_HasSurfaceTexture
					#if Is_Water2D_SurfaceTextureStretch_Enabled || Is_Water2D_SurfaceTextureStretchAutoX_Enabled || Is_Water2D_SurfaceTextureStretchAutoY_Enabled
						#if Is_Water2D_SurfaceTextureStretchAutoX_Enabled
							_SurfaceTexture_ST.xy /= (1.0 - surfaceLevel);
							o.surfaceTextureUV.x = v.uv.x * (_SurfaceTexture_ST.x * _AspectRatio.x) + _SurfaceTexture_ST.z;
							o.surfaceTextureUV.y = (v.uv.y - surfaceLevel) * _SurfaceTexture_ST.y + _SurfaceTexture_ST.w;
						#elif Is_Water2D_SurfaceTextureStretchAutoY_Enabled
							o.surfaceTextureUV.x = v.uv.x * _SurfaceTexture_ST.x + _SurfaceTexture_ST.z;
							o.surfaceTextureUV.y = (v.uv.y - surfaceLevel) * (_SurfaceTexture_ST.y * _AspectRatio.w) + _SurfaceTexture_ST.w;
						#else
							o.surfaceTextureUV.x = v.uv.x * _SurfaceTexture_ST.x + _SurfaceTexture_ST.z;
							o.surfaceTextureUV.y = (v.uv.y - surfaceLevel) * (_SurfaceTexture_ST.y / (1.0 - surfaceLevel)) + _SurfaceTexture_ST.w;
						#endif
					#else
						o.surfaceTextureUV.xy = TRANSFORM_TEX(vertexPositionWorldSpace, _SurfaceTexture);
					#endif

					#if Is_Water2D_SurfaceTextureScroll_Enabled
						o.surfaceTextureUV.xy += fmod2(float2(_SurfaceTextureScrollingSpeedX, _SurfaceTextureScrollingSpeedY) * _G2DWK_Frame_Time.x);
					#endif

					#if Is_Water2D_SurfaceNoise_Enabled
						o.surfaceTextureUV.zw = TRANSFORM_TEX((o.surfaceTextureUV.xy * _SurfaceNoiseTiling.xy),_NoiseTexture) + fmod2(_G2DWK_Frame_Time.w * _SurfaceNoiseSpeed);
					#endif
				#endif

				#if Water2D_HasWaterTexture
					#if Is_Water2D_WaterTextureStretch_Enabled || Is_Water2D_WaterTextureStretchAutoX_Enabled || Is_Water2D_WaterTextureStretchAutoY_Enabled
						#if Is_Water2D_WaterTextureStretchAutoX_Enabled
							#if Is_Water2D_Surface_Enabled
								_WaterTexture_ST.xy /= surfaceLevel;
							#endif
							o.waterTextureUV.x = v.uv.x * (_WaterTexture_ST.x * _AspectRatio.x) + _WaterTexture_ST.z;
							o.waterTextureUV.y = v.uv.y * _WaterTexture_ST.y + _WaterTexture_ST.w;
						#elif Is_Water2D_WaterTextureStretchAutoY_Enabled
							o.waterTextureUV.x = v.uv.x * _WaterTexture_ST.x + _WaterTexture_ST.z;
							o.waterTextureUV.y = v.uv.y * (_WaterTexture_ST.y * _AspectRatio.w) + _WaterTexture_ST.w;
						#else
							#if Is_Water2D_Surface_Enabled
								_WaterTexture_ST.y /= surfaceLevel;
							#endif
							o.waterTextureUV.xy = TRANSFORM_TEX(v.uv, _WaterTexture);
						#endif
					#else
						o.waterTextureUV.xy = TRANSFORM_TEX(vertexPositionWorldSpace, _WaterTexture);
					#endif
						
					#if Is_Water2D_WaterTextureScroll_Enabled
						o.waterTextureUV.xy += fmod2(float2(_WaterTextureScrollingSpeedX, _WaterTextureScrollingSpeedY) * _G2DWK_Frame_Time.x);
					#endif

					#if Is_Water2D_WaterNoise_Enabled
						o.waterTextureUV.zw = TRANSFORM_TEX((o.waterTextureUV.xy * _WaterNoiseTiling.xy),_NoiseTexture) + fmod2(_G2DWK_Frame_Time.w * _WaterNoiseSpeed);
					#endif

				#endif
				return o;
			}

			#if Is_Water2D_Reflection_Enabled && (Is_Water2D_ReflectionFadeLinear_Enabled || Is_Water2D_ReflectionFadeExponentialTwo_Enabled || Is_Water2D_ReflectionFadeExponentialThree_Enabled || Is_Water2D_ReflectionFadeExponentialFour_Enabled)
			inline half4 FadeReflectionColor(half4 color, half fadeFactor)
			{
				#if Is_Water2D_ReflectionFadeLinear_Enabled
					color.a *= fadeFactor;
				#elif Is_Water2D_ReflectionFadeExponentialTwo_Enabled
					color.a *= fadeFactor * fadeFactor;
				#elif Is_Water2D_ReflectionFadeExponentialThree_Enabled
					color.a *= fadeFactor * fadeFactor * fadeFactor;
				#elif Is_Water2D_ReflectionFadeExponentialFour_Enabled
					color.a *= fadeFactor * fadeFactor * fadeFactor * fadeFactor;
				#endif

				return color;
			}
			#endif

			#if Is_Water2D_FakePerspective_Enabled && !defined(Game2DWaterKit_Unlit) && !defined(Game2DWaterKit_SRP_Unlit)
			#if defined(Game2DWaterKit_PixelLit_Add)
			inline half4 Water2D_Frag(Varyings i, out half frontColorOpacity)
			#else
			inline half4 Water2D_Frag(Varyings i, out half4 frontColor)
			#endif
			#else
			inline half4 Water2D_Frag(Varyings i)
			#endif
			{
				half4 finalColor = 0;
				
				#if Is_Water2D_Surface_Enabled
					#if Is_Water2D_SurfaceHasAbsoluteThickness_Enabled
						float surfaceLevel = (_Size.y - _SurfaceLevel) * _Size.w;
					#else
						float surfaceLevel = _SurfaceLevel;
					#endif

					bool isSurface = i.uv.y > surfaceLevel;
					#if Is_Water2D_FakePerspective_Enabled

						#if Is_Water2D_SurfaceHasAbsoluteThickness_Enabled
							float submergeLevel = surfaceLevel + (1.0 - surfaceLevel) * _SubmergeLevel;
						#else
							float submergeLevel = _SubmergeLevel;
						#endif

						bool isBelowSubmergeLevel = i.uv.y < submergeLevel;
					#endif
				#endif

				// Sampling Refraction Render-Texture
				#if Is_Water2D_Refraction_Enabled
					float refractionDistortion = (tex2D(_NoiseTexture,i.refractionUV.xy).r - 0.5) * _RefractionNoiseStrength + _RefractionAmountOfBending;
					half4 refractionColor = tex2D(_RefractionTexture,float2(i.refractionReflectionUV.xy + refractionDistortion));
					#if Is_Water2D_FakePerspective_Enabled
						half4 refractionColorPartiallySubmergedObjects;
						if (isBelowSubmergeLevel) {
							refractionColorPartiallySubmergedObjects = tex2D(_RefractionTexturePartiallySubmergedObjects, float2(i.refractionReflectionUV.xy + refractionDistortion));
							refractionColor.rgb += refractionColorPartiallySubmergedObjects.rgb - refractionColor.rgb * refractionColorPartiallySubmergedObjects.a;
						}
						else {
							refractionColorPartiallySubmergedObjects = tex2D(_RefractionTexturePartiallySubmergedObjects, i.refractionReflectionUV.xy);
						}
					#endif
				#elif Is_Water2D_FakePerspective_Enabled
					half4 refractionColorPartiallySubmergedObjects = isBelowSubmergeLevel ? 0 : tex2D(_RefractionTexturePartiallySubmergedObjects, i.refractionReflectionUV.xy);
				#endif

				// Sampling Reflection Render-Texture
				#if Is_Water2D_Reflection_Enabled
					half4 reflectionColor = 0;
					float reflectionDistortion = (tex2D(_NoiseTexture, i.reflectionUV.xy).g - 0.5) * _ReflectionNoiseStrength;
					
					#if Is_Water2D_FakePerspective_Enabled
						if(isSurface){
							float reflectionCoordY = (i.uv.y - _ReflectionFakePerspectiveLowerLimit) / (_ReflectionFakePerspectiveUpperLimit - _ReflectionFakePerspectiveLowerLimit);
							float2 reflectionTextureCoord = float2(i.refractionReflectionUV.x + reflectionDistortion, 1.0 - reflectionCoordY + reflectionDistortion);
							reflectionColor = tex2D(_ReflectionTexture, reflectionTextureCoord);
							#if Is_Water2D_ReflectionFadeLinear_Enabled || Is_Water2D_ReflectionFadeExponentialTwo_Enabled || Is_Water2D_ReflectionFadeExponentialThree_Enabled || Is_Water2D_ReflectionFadeExponentialFour_Enabled
							reflectionColor = FadeReflectionColor(reflectionColor, reflectionCoordY);
							#endif
							if (isBelowSubmergeLevel) {
								float reflectionPartiallySubmergedObjectsCoordY = (i.uv.y - _ReflectionFakePerspectiveLowerLimit) / (_ReflectionFakePerspectivePartiallySubmergedObjectsUpperLimit - _ReflectionFakePerspectiveLowerLimit);
								float2 reflectionTextureCoordPartiallySubmergedObjects = float2(i.refractionReflectionUV.x + reflectionDistortion, 1.0 - reflectionPartiallySubmergedObjectsCoordY + reflectionDistortion);
								half4 reflectionColorPartiallySubmergedObjects = tex2D(_ReflectionTexturePartiallySubmergedObjects, reflectionTextureCoordPartiallySubmergedObjects);
								#if Is_Water2D_ReflectionFadeLinear_Enabled || Is_Water2D_ReflectionFadeExponentialTwo_Enabled || Is_Water2D_ReflectionFadeExponentialThree_Enabled || Is_Water2D_ReflectionFadeExponentialFour_Enabled
								reflectionColorPartiallySubmergedObjects = FadeReflectionColor(reflectionColorPartiallySubmergedObjects, reflectionPartiallySubmergedObjectsCoordY);
								#endif

								reflectionColor.rgb = lerp(reflectionColor.rgb, reflectionColorPartiallySubmergedObjects.rgb, reflectionColorPartiallySubmergedObjects.a);
								reflectionColor.a = reflectionColorPartiallySubmergedObjects.a + (1.0 - reflectionColorPartiallySubmergedObjects.a) * reflectionColor.a;
							}
						}
					#else
						i.refractionReflectionUV.y = 1.0 - i.refractionReflectionUV.y;
						reflectionColor = tex2D(_ReflectionTexture, float2(i.refractionReflectionUV.xy + reflectionDistortion));
						#if Is_Water2D_ReflectionFadeLinear_Enabled || Is_Water2D_ReflectionFadeExponentialTwo_Enabled || Is_Water2D_ReflectionFadeExponentialThree_Enabled || Is_Water2D_ReflectionFadeExponentialFour_Enabled
						reflectionColor = FadeReflectionColor(reflectionColor, i.uv.y);
						#endif
					#endif
				#endif

				#if Water2D_HasWaterTexture || Water2D_HasSurfaceTexture
					half4 textureSampledColor = 0;
				#endif

				// Sampling Water Surface Texture
				#if Water2D_HasSurfaceTexture
					if(isSurface){
						#if Is_Water2D_SurfaceNoise_Enabled
							i.surfaceTextureUV.xy += (tex2D(_NoiseTexture,i.surfaceTextureUV.zw).b - 0.5) * _SurfaceNoiseStrength;
						#endif

						#if Water2D_HasSurfaceTextureSheet
							#if Is_Water2D_SurfaceTextureSheetWithLerp_Enabled
							textureSampledColor = SampleTextureSheetLerp(_SurfaceTexture, i.surfaceTextureUV.xy);
							#else
							textureSampledColor = SampleTextureSheet(_SurfaceTexture, i.surfaceTextureUV.xy);
							#endif
						#else
							textureSampledColor = tex2D(_SurfaceTexture,i.surfaceTextureUV.xy);
						#endif

						textureSampledColor.a *= _SurfaceTextureOpacity;
					}
				#endif

				// Sampling Water Body Texture
				#if Water2D_HasWaterTexture
					#if Is_Water2D_Surface_Enabled
						if(!isSurface){
					#endif
							#if Is_Water2D_WaterNoise_Enabled
								i.waterTextureUV.xy += (tex2D(_NoiseTexture,i.waterTextureUV.zw).a - 0.5) * _WaterNoiseStrength;
							#endif

							#if Water2D_HasWaterTextureSheet
								#if Is_Water2D_WaterTextureSheetWithLerp_Enabled
								textureSampledColor = SampleTextureSheetLerp(_WaterTexture, i.waterTextureUV.xy);
								#else
								textureSampledColor = SampleTextureSheet(_WaterTexture, i.waterTextureUV.xy);
								#endif
							#else
								textureSampledColor = tex2D(_WaterTexture,i.waterTextureUV.xy);
							#endif

							textureSampledColor.a *= _WaterTextureOpacity;
					#if Is_Water2D_Surface_Enabled
						}
					#endif
				#endif
				
				// Tint Color
				half4 tintColor = 0;
				#if Is_Water2D_Surface_Enabled
					if(isSurface)
					{
						#if Is_Water2D_SurfaceColorGradient_Enabled
							tintColor = _SurfaceColorGradientEnd + saturate((i.uv.y - surfaceLevel) / (1.0 - surfaceLevel) + _SurfaceColorGradientOffset) * (_SurfaceColorGradientStart - _SurfaceColorGradientEnd);
						#else
							tintColor = _SurfaceColor;
						#endif
					}else{
				#endif
						#if Is_Water2D_ColorGradient_Enabled
							#if defined (Water2D_Surface)
								tintColor = _WaterColorGradientEnd + saturate((i.uv.y / surfaceLevel) + _WaterColorGradientOffset) * (_WaterColorGradientStart - _WaterColorGradientEnd);
							#else
								tintColor = _WaterColorGradientEnd + saturate(i.uv.y + _WaterColorGradientOffset) * (_WaterColorGradientStart - _WaterColorGradientEnd);
							#endif
						#else
							tintColor = _WaterColor;
						#endif
				#if Is_Water2D_Surface_Enabled
					}
				#endif

				// Applying Colors

				// Applying Refraction Render-Texture Color
				#if Is_Water2D_Refraction_Enabled
					finalColor = refractionColor;
				#endif

				// Applying Reflection Render-Texture Color
				#if Is_Water2D_Reflection_Enabled
					#if Is_Water2D_FakePerspective_Enabled
							if(isSurface){
					#endif
								reflectionColor.a *= step(i.reflectionUV.z, _ReflectionLowerLimit) * _ReflectionVisibility;
								finalColor = MixColors(finalColor, reflectionColor);
					#if Is_Water2D_FakePerspective_Enabled
							}
					#endif
				#endif

				#if Is_Water2D_ApplyTintColorBeforeTexture_Enabled
					// Applying Tint Color
					finalColor = MixColors(finalColor, tintColor);

					// Applying Water Body/Surface Texture Color
					#if Water2D_HasSurfaceTexture ||  Water2D_HasWaterTexture
					finalColor = MixColors(finalColor, textureSampledColor);
					#endif
				#else
					// Applying Water Body/Surface Texture Color
					#if Water2D_HasSurfaceTexture ||  Water2D_HasWaterTexture
					finalColor = MixColors(finalColor, textureSampledColor);
					#endif

					// Applying Tint Color
					finalColor = MixColors(finalColor, tintColor);
				#endif

				// Applying Partially Submerged Objects Render-Texture Color
				#if Is_Water2D_FakePerspective_Enabled
					#if defined(Game2DWaterKit_Unlit) || defined(Game2DWaterKit_SRP_Unlit)
						if (!isBelowSubmergeLevel) {
							#if Is_Water2D_Refraction_Enabled
								finalColor.rgb += refractionColorPartiallySubmergedObjects.rgb - finalColor.rgb * refractionColorPartiallySubmergedObjects.a;
							#else
								finalColor = MixColors(finalColor, refractionColorPartiallySubmergedObjects);
							#endif
						}
					#endif

					#if defined(Game2DWaterKit_SRP_Lit)
						frontColor.rgb = refractionColorPartiallySubmergedObjects.rgb; //out parameter
						frontColor.a = refractionColorPartiallySubmergedObjects.a * (isBelowSubmergeLevel ? 0.0 : 1.0); //out parameter
					#elif !defined(Game2DWaterKit_SRP_Unlit)
						#if defined(Game2DWaterKit_PixelLit_Add)
							frontColorOpacity = refractionColorPartiallySubmergedObjects.a * (isBelowSubmergeLevel ? 0.0 : 1.0); //out parameter
						#elif !defined(Game2DWaterKit_Unlit)
							frontColor.rgb = refractionColorPartiallySubmergedObjects.rgb; //out parameter
							frontColor.a = refractionColorPartiallySubmergedObjects.a * (isBelowSubmergeLevel ? 0.0 : 1.0); //out parameter
						#endif
					#endif
				#endif
				
				#if !Is_Water2D_Refraction_Enabled
				finalColor.rgb /= finalColor.a;
				#endif

				return finalColor;
			}

#endif // Game2D_WaterKit_Water_INCLUDED
