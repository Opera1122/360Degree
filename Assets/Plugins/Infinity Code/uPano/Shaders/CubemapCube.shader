/*       INFINITY CODE 2018         */
/*   https://www.infinity-code.com   */

Shader "uPano/Cubemap"
{
	Properties
	{
		_cubemap("Cubemap", Cube) = "" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag

			uniform samplerCUBE _cubemap;

			struct vertexInput 
			{
				float4 vertex : POSITION;
			};
			
			struct vertexOutput 
			{
				float4 pos : SV_POSITION;
				float3 texDir : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				output.texDir = input.vertex;
				output.pos = UnityObjectToClipPos(input.vertex);
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return texCUBE(_cubemap, input.texDir);
			}

			ENDCG
		}
	}
}