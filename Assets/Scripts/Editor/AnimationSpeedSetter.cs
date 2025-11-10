using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Animations;
public class AnimationSpeedSetter : EditorWindow
{
    class ClipInfo
    {
        public AnimationClip animationClip;
        public float expectedLength;
        public float speed;
        public int selectedIndex;
        public AnimatorController specificController;
    }
    class RectInfo
    {
        public int clipLavelWidth = 90;
        public int clipFieldWidth = 180;
        public int lengthLavelWidth = 110;
        public int lengthFieldWidth = 60;
        public int speedLavelWidth = 140;
        public int speedFieldWidth = 60;
        public int popupLavelWidth = 80;
        public int popupFieldWidth = 100;
        public int spcContLavelWidth = 170;
        public int spcContFieldWidth = 200;
        public int boxHeight = 20;
        public int helpBoxHeight = 30;
        public int helpBoxWidth = 200;
        public int columnSpace = 5;
        public int rowSpace = 2;
        //public int helpBoxSpace = 1400;
    }

    static RectInfo rectInfo;
    Vector2 scrollPos = Vector2.zero;
    public readonly string[] options = { "All", "Specific" };
    List<ClipInfo> clipInfos = new();
    [MenuItem("Tools/Animation Speed Setter")]
    static void Open()
    {
        rectInfo ??= new RectInfo();
        GetWindow<AnimationSpeedSetter>();
    }
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos,alwaysShowHorizontal:true,alwaysShowVertical:true);
        for (int i = 0; i < clipInfos.Count; i++)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 10;
            GUILayout.Space(rectInfo.columnSpace);
            EditorGUILayout.BeginHorizontal(GUI.skin.box,GUILayout.Height(rectInfo.boxHeight));
            var info = clipInfos[i];

            GUILayout.Label("Animation Clip", GUILayout.Width(rectInfo.clipLavelWidth)); // ← ラベルは固定幅で
            info.animationClip = (AnimationClip)EditorGUILayout.ObjectField(
                info.animationClip, typeof(AnimationClip), false, GUILayout.Width(rectInfo.clipFieldWidth)
            );

            GUILayout.Space(rectInfo.columnSpace);

            GUILayout.Label("Expected Length", GUILayout.Width(rectInfo.lengthLavelWidth)) ;
            info.expectedLength = EditorGUILayout.FloatField(info.expectedLength, GUILayout.Width(rectInfo.lengthFieldWidth));

            var length = info.animationClip != null ? info.animationClip.GetLength() : 0f;
            info.speed = info.expectedLength > 0 ? length / info.expectedLength : float.NaN;                    

            GUILayout.Space(rectInfo.columnSpace);

            GUILayout.Label("Required State Speed", GUILayout.Width(rectInfo.speedLavelWidth));
            EditorGUILayout.FloatField(info.speed, GUILayout.Width(rectInfo.speedFieldWidth));

            GUILayout.Space(rectInfo.columnSpace);

            GUILayout.Label("Target Mode", GUILayout.Width(rectInfo.popupLavelWidth));
            info.selectedIndex = EditorGUILayout.Popup(info.selectedIndex,options,GUILayout.Width(rectInfo.popupFieldWidth));//,GUILayout.Width(rectInfo.popupWidth)
            if (options[info.selectedIndex] == "Specific")
            {
                GUILayout.Label("Specific Animator Controller", GUILayout.Width(rectInfo.spcContLavelWidth));
                info.specificController = (AnimatorController)EditorGUILayout.ObjectField(
                                            info.specificController, typeof(AnimatorController),false,GUILayout.Width(rectInfo.spcContFieldWidth));
            }

            GUILayout.Space(rectInfo.columnSpace);
            //var helpBoxSpace = options[info.selectedIndex] == "Specific"
            //                                               ? rectInfo.helpBoxSpace - (rectInfo.spcContLavelWidth + rectInfo.spcContFieldWidth)
            //                                               : rectInfo.helpBoxSpace;
           
            EditorGUILayout.EndHorizontal();
            if (info.animationClip == null)
            {
                //EditorGUILayout.HelpBoxの中身はこの二つで作られているらしい
                var rect = EditorGUILayout.GetControlRect(false,rectInfo.helpBoxHeight,GUILayout.Width(rectInfo.helpBoxWidth));
                EditorGUI.HelpBox(rect, "Please set an animationClip !!", MessageType.Warning);
            }
        }

        GUILayout.Space(rectInfo.rowSpace);
        if (GUILayout.Button("Add New Clip"))
        {
            clipInfos.Add(new ClipInfo());
        }

        if(GUILayout.Button("Apply Speeds"))
        {
            var isAcceped = EditorUtility.DisplayDialog(
                  "Confirm Apply",
                  "Do you want to apply the speed to all AnimatorControllers?",
                  "Ok",
                  "Cancell"
                );
            if(isAcceped) foreach (var clipInfo in clipInfos) ApplySpeeds(clipInfo);
        }

        EditorGUILayout.EndScrollView();
    }
    void ApplySpeeds(ClipInfo clipInfo)
    {
        var clip = clipInfo.animationClip;
        var expectedLength = clipInfo.expectedLength;
        if (clip == null || expectedLength <= 0) return;
        var selectedIndex = clipInfo.selectedIndex;
        var controllers = selectedIndex == 0
                        ? GetControllers()
                        : new AnimatorController[] { clipInfo.specificController };
        var speed = clipInfo.speed;
        ApplySpeedToTargetControllers(clip, speed, controllers);
    }
    AnimatorController[] GetControllers()
    {
        var allControllerGUIs = AssetDatabase.FindAssets("t:AnimatorController");//アセットのGUI
        var controllers = new List<AnimatorController>();
        foreach (var controllerGUI in allControllerGUIs)
        {
            var path = AssetDatabase.GUIDToAssetPath(controllerGUI);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller != null) controllers.Add(controller);
        }

        return controllers.ToArray();
    }
    void ApplySpeedToTargetControllers(AnimationClip targetClip,float targetSpeed,AnimatorController[] controllers)
    {
        foreach (var controller in controllers)
        {
            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if(state.state.motion == targetClip)
                    {
                        Undo.RecordObject(state.state, "Revert to previous speed");
                        state.state.speed = targetSpeed;
                    }
                }
            }

            //Unityに「このアセットが変更された」と通知
            //メモリ上で変更を加えた時（まだ保存してない）
            //SerializeFieldの内部の処理？
            EditorUtility.SetDirty(controller);
        }

        //Dirty状態のアセットをディスクに書き出す
        //実際に.controller ファイルなどへ保存するとき
        AssetDatabase.SaveAssets();//外部スクリプトや別ツールで .controller をいじった直後に再読込したい場合,File.WriteAllText() とかで直接 Assets/ 内を書き換えた時にRefleshがいるからこの場合はいらない
    }
}
