using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

//URP の 「レンダリングの途中に差し込む1ステップ」 を自作してるクラス

//この FogPass がやりたいこと、ざっくり   
//今カメラに描かれてる絵（colorTarget）を一回テクスチャにコピー
//そのテクスチャに fog 用のマテリアル（ポストエフェクト） をかける
//結果をまた画面に書き戻す
//= “画面全体にフォグをかける後処理” を担当するやつ。
public class FogPass : ScriptableRenderPass
{
    Material material; //シェーダーマテリアル、今回はFogShader
    private RTHandle tempTarget;//一時的な描画先（バッファ = 一時的な記憶領域、メモリ）
                                //そのGPUのバッファの内部を操作する = Handle

    //RenderFeature 側からマテリアルを渡して初期化, rendererFeatureのinspecter？の部分にアタッチされてるMaterial
    //renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    //「レンダリングのどのタイミングでこのパスを挟むか」を決めてる
    //AfterRenderingPostProcessing = ポストプロセスのあと、最終出力直前あたり
    //→ フォグを一番最後にかけたいからここにしてる
    public FogPass(Material mat)
    {
        material = mat;
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }
    //毎フレーム、毎カメラ呼ばれる処理
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null) return;

        //GPUに「これこれこういう描画命令してね」ってまとめて渡すための CommandBuffer
        //CommandBufferPool から借りてきて名前を "FogPass" にしてるだけ
        //→ デバッグ時にどのコマンドか分かりやすくなる
        var cmd = CommandBufferPool.Get("FogPass");

        // 正しい取得方法（URP14+）
        //今そのカメラに描かれている “色バッファ”（画面） を取ってくる
        //以前は cameraColorTarget（RenderTargetIdentifier）だったけど、
        //URP14 からは RTHandle で持つように変わった
        //colorTarget = 「今の画面」って覚えておけばOK
        RTHandle colorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        // 一時RTのDescriptor
        //これから作る一時テクスチャ（tempTarget）の 解像度やフォーマットの情報
        //cameraTargetDescriptor = カメラの出力と同じ設定
        //depthBufferBits = 0;
        //深度(Depth)バッファはいらない（フォグは色いじるだけ）ので 0 にして軽くしてる
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        // RTHandleとして確保
        //tempTarget という一時テクスチャを 必要なら作る or サイズが変わってたら作り直す
        //ReAllocateIfNeeded がやってくれること：
        //tempTarget が null → 新しく RTHandle を作る
        //解像度が変わった（画面サイズ変わった）→ 作り直す
        //変わってない → そのまま使う

        //ref tempTarget → ここにRTHandleが入る（参照渡し）
        //descriptor → 解像度・フォーマット情報
        //FilterMode.Point → 拡大縮小のときのフィルタ
        //TextureWrapMode.Clamp → UVがはみ出したとき端の色で伸ばす
        //"_FogTempTex" → このRTの名前
        //tempTargetのメモリを再利用

        //「画面と同じサイズの一時的な描画先」を用意してると思ってOK
        RenderingUtils.ReAllocateIfNeeded(
            ref tempTarget,
            descriptor,
            FilterMode.Point,
            TextureWrapMode.Clamp,
            name: "_FogTempTex"
        );

        //Blit（RTHandle 同士）

        //colorTarget（今の画面） → tempTarget にコピー
        //まだ fog はかけてない「生の絵」の状態
        Blitter.BlitCameraTexture(cmd, colorTarget, tempTarget);

        //tempTarget → colorTarget に戻すときに、
        //material のシェーダーを通して描き戻している
        //第4引数 material → どのマテリアルを使うか
        //第5引数 0 → マテリアルの 0 番目のパス
        //ここで 「画面全体に fog シェーダーを適用」 してる。
        Blitter.BlitCameraTexture(cmd, tempTarget, colorTarget, material, 0);

        //ここまで cmd にたまっていた命令を GPU に流す
        //使い終わった cmd をプールに返す（メモリ節約）
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // ここでは解放しない（URPが管理）
    }
}