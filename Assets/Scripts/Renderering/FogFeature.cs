using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogFeature : ScriptableRendererFeature
{
    class FogPass : ScriptableRenderPass
    {
        public Material material;

        RTHandle source;
        RTHandle destination;

        public FogPass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // ÅVAPI
            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTargetHandle;

            // ˆêRTŠm•Û
            RenderingUtils.ReAllocateIfNeeded(
                ref destination,
                renderingData.cameraData.cameraTargetDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_FogTempTex"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("FogPass");

            // « URPŒö®„§‚ÌBlitter
            Blitter.BlitCameraTexture(cmd, source, destination, material, 0);
            Blitter.BlitCameraTexture(cmd, destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            destination?.Release();
        }
    }

    public Material fogMaterial;
    FogPass pass;

    public override void Create()
    {
        pass = new FogPass(fogMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
