Shader "Custom/DisableWrite"
{
    SubShader{
        Tags{
            "RenderType" = "Opaque"
        }

        Pass{
            ZWrite Off
        }
    }
}