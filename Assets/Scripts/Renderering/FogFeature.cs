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
            // 最新API
            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTargetHandle;

            // 一時RT確保
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
            Debug.Log("FogPass Execute");  // ★追加：1フレーム毎にログが出れば実行されてる
            // ↓ URP公式推奨のBlitter
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
        var camera = renderingData.cameraData.camera;
        if(camera.TryGetComponent<FogCameraMaker>(out var _)) renderer.EnqueuePass(pass);
    }
}
