using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogFeature : ScriptableRendererFeature
{ 

    [SerializeField] Material fogMaterial;
    public Material runTimeMat { get; private set; }
    FogPass pass;
    //RendererFeature が初期化された時に “1回だけ” 呼ばれる関数
    public override void Create()
    {
        runTimeMat = new Material(fogMaterial);
        pass = new FogPass(runTimeMat);
    }

    //このフレーム・このカメラに、FogPass を追加するかどうかを決める関数」
    //毎フレーム、それぞれのカメラをチェックしてる,だからcameraに入るカメラは毎回一つ
    //この関数は毎フレームカメラの数だけ実行される
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var camera = renderingData.cameraData.camera;
        if (camera.TryGetComponent<FogCameraMaker>(out var _))
        {
            //このフレームのレンダリングパイプラインに FogPass を差し込む
            //この 1 行で：今フレーム,このカメラ,このレンダリング順序で,FogPass が実行されることが確定
            renderer.EnqueuePass(pass);
        }
    }
}
