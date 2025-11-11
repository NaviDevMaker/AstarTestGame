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
    class StyleInfo
    {
        public readonly int clipLavelWidth = 110;
        public readonly int clipFieldWidth = 300;
        public readonly int lengthLavelWidth = 110;
        public readonly int lengthFieldWidth = 300;
        public readonly int speedLavelWidth = 150;
        public readonly int speedFieldWidth = 300;
        public readonly int popupLavelWidth = 90;
        public readonly int popupFieldWidth = 300;
        public readonly int spcContLavelWidth = 180;
        public readonly int spcContFieldWidth = 300;
        public readonly int boxHeight = 20;
        public readonly int helpBoxHeight = 40;//iconのheightと同じの予定
        public readonly int helpBoxWidth = 250;
        public readonly int firstaColumnSpace = 5;
        public readonly int columnSpace = 25;
        public readonly int rowSpace = 2;

        public readonly int lavelFontSize = 13;

        public readonly int iconWidth = 40;
        public readonly int iconHeight = 40;//helpBoxと同じの予定
        public readonly int helpBoxFontSize = 15;
        public readonly Color helpBoxTextCol = Color.yellow;
    }

    static StyleInfo styleInfo;
    Vector2 scrollPos = Vector2.zero;
    readonly string[] options = { "All", "Specific" };

    enum LavelName
    { 
        AnimationClip,
        ExpectedLength,
        RequiredSpeed,
        TargetMode,
        HelpBox,
        Specific
    }

    readonly Dictionary<LavelName, string> lavelContentDic = new Dictionary<LavelName, string>()
    {
        {LavelName.AnimationClip,"Animation Clip"},
        {LavelName.ExpectedLength,"Expected Length" },
        {LavelName.RequiredSpeed, "Required State Speed"},
        {LavelName.TargetMode,"Target Mode"},
        {LavelName.HelpBox,"Please set an animationClip !!"},
        {LavelName.Specific,"Specific Animator Controller"}
    };

    readonly Dictionary<LavelName, GUIContent> guiContentDic = new Dictionary<LavelName, GUIContent>()
    {
        {LavelName.AnimationClip,new GUIContent("!","You have to set an animationclip")},
        {LavelName.ExpectedLength,new GUIContent("!","Expected animation play time (in seconds)") },
        {LavelName.RequiredSpeed,new GUIContent("!","Caluculated speed based on \"Expected Length\"") },
        {LavelName.TargetMode,new GUIContent("!","Applicable Target : All animators or specific animator") },
    };

    List<ClipInfo> clipInfos = new();
    [MenuItem("Tools/Animation Speed Setter")]
    static void Open()
    {
        styleInfo ??= new StyleInfo();
        GetWindow<AnimationSpeedSetter>();
    }
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos,alwaysShowHorizontal:true,alwaysShowVertical:true);

        var lavelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = styleInfo.lavelFontSize
        };
        var helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
        { 
            fontSize = styleInfo.helpBoxFontSize,
            normal = {textColor = styleInfo.helpBoxTextCol}
        };

        for (int i = 0; i < clipInfos.Count; i++)
        {
            var space = i == 0
                        ? styleInfo.firstaColumnSpace
                        : styleInfo.columnSpace;
            GUILayout.Space(space);
            EditorGUILayout.BeginHorizontal(GUI.skin.box,GUILayout.Height(styleInfo.boxHeight));
            var info = clipInfos[i];

            GUILayout.Label(GetLavelContent(LavelName.AnimationClip), lavelStyle, GUILayout.Width(styleInfo.clipLavelWidth)); // ← ラベルは固定幅で
            info.animationClip = (AnimationClip)EditorGUILayout.ObjectField(
                GetGUIContent(LavelName.AnimationClip),
                info.animationClip
                ,typeof(AnimationClip)
                ,false
                ,GUILayout.Width(styleInfo.clipFieldWidth)
            );

            GUILayout.Space(styleInfo.columnSpace);

            GUILayout.Label(GetLavelContent(LavelName.ExpectedLength), lavelStyle, GUILayout.Width(styleInfo.lengthLavelWidth)) ;
            info.expectedLength = EditorGUILayout.FloatField(
                 GetGUIContent(LavelName.ExpectedLength)
                ,info.expectedLength
                ,GUILayout.Width(styleInfo.lengthFieldWidth)
                );

            var length = info.animationClip != null ? info.animationClip.GetLength() : 0f;
            info.speed = info.expectedLength > 0 ? length / info.expectedLength : float.NaN;                    

            GUILayout.Space(styleInfo.columnSpace);

            GUILayout.Label(GetLavelContent(LavelName.RequiredSpeed), lavelStyle, GUILayout.Width(styleInfo.speedLavelWidth));
            EditorGUILayout.FloatField(
                            GetGUIContent(LavelName.RequiredSpeed)
                　　　　　　,info.speed, GUILayout.Width(styleInfo.speedFieldWidth)
                      　　　);

            GUILayout.Space(styleInfo.columnSpace);

            GUILayout.Label(GetLavelContent(LavelName.TargetMode), lavelStyle, GUILayout.Width(styleInfo.popupLavelWidth));
            info.selectedIndex = EditorGUILayout.Popup(
                                 GetGUIContent(LavelName.TargetMode)
                                 ,info.selectedIndex,options
                                 ,GUILayout.Width(styleInfo.popupFieldWidth)
                                 );//,GUILayout.Width(rectInfo.popupWidth)
            if (options[info.selectedIndex] == "Specific")
            {
                GUILayout.Label(GetLavelContent(LavelName.Specific),lavelStyle, GUILayout.Width(styleInfo.spcContLavelWidth));
                info.specificController = (AnimatorController)EditorGUILayout.ObjectField(
                                            info.specificController, typeof(AnimatorController),false,GUILayout.Width(styleInfo.spcContFieldWidth));
            }

            GUILayout.Space(styleInfo.columnSpace);
            //var helpBoxSpace = options[info.selectedIndex] == "Specific"
            //                                               ? rectInfo.helpBoxSpace - (rectInfo.spcContLavelWidth + rectInfo.spcContFieldWidth)
            //                                               : rectInfo.helpBoxSpace;
           
            EditorGUILayout.EndHorizontal();
            if (info.animationClip == null)
            {
                AppearHelpBox(helpBoxStyle);
                //EditorGUILayout.HelpBoxの中身はこの二つで作られているらしい
                //var rect = EditorGUILayout.GetControlRect(false,styleInfo.helpBoxHeight,GUILayout.Width(styleInfo.helpBoxWidth));
                //EditorGUI.HelpBox(rect, "Please set an animationClip !!", helpBoxStyle, MessageType.Warning);
            }
        }

        GUILayout.Space(styleInfo.rowSpace);
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
    void AppearHelpBox(GUIStyle helpBoxStyle)
    {
        var icon = EditorGUIUtility.IconContent("console.warnicon");
        GUILayout.BeginHorizontal(GUI.skin.box,GUILayout.Height(styleInfo.boxHeight));
        GUILayout.Label(icon,GUILayout.Width(styleInfo.iconWidth),GUILayout.Height(styleInfo.iconHeight));
        GUILayout.Label(GetLavelContent(LavelName.HelpBox),helpBoxStyle,GUILayout.Width(styleInfo.helpBoxWidth),GUILayout.Height(styleInfo.helpBoxHeight));
        GUILayout.EndHorizontal();
    }

    string GetLavelContent(LavelName lavelName) => lavelContentDic[lavelName];

    GUIContent GetGUIContent(LavelName lavelName) => guiContentDic[lavelName];
}
