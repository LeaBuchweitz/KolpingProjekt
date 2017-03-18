Shader "Unlit/TransCutOff"
{

	// USE THIS SHADER FOR ALL ALPHA CUT OFF IMAGES IN FRONT OF OTHERS
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_CutOff("Base Alpha cutoff", Range(0,0.9)) = 0.5


	}
		Category{
		Tags{"QUEUE" = "AlphaTest+10000" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull off

		SubShader{ Pass{
		SetTexture[_MainTex]{ combine texture * constant ConstantColor[_Color] }
	} }
	}
}
