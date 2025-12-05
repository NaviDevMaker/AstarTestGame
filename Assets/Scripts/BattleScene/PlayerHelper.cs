using Cysharp.Threading.Tasks;
using Game.Player;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Player
{
    public class PlayerHelper : MonoBehaviour
    {
        [SerializeField] PlayerController player;
        [SerializeField] GameObject helpArrowPrefab;
        [SerializeField] GameObject helpUIPrefab;

        const float appearSeconds = 2.0f;
        float elapsedTime = 0f;
        public bool isSetUped { get; set; } = false;
        bool isHelping = false;
        readonly List<(GameObject arrow,Material mat)> pairs = new List<(GameObject arrow,Material mat)>();
        readonly List<Tween> currentTweens = new List<Tween>();
        const float duration = 1.5f;
        const float offsetY = 10.0f;
        const float arrowOffset = 0.5f;
        GameObject arrowParent;
        // Update is called once per frame
        void Update()
        {
            if (!isSetUped) return;
            if (player.currentState is not PlayerIdleState)
            {
                HelpClose();
                return;
            }
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= appearSeconds) StartHelpAction();         
        }

        public void Initialize()
        {
            SetUpArrows();
        }
        void SetUpArrows()
        {
            arrowParent = new GameObject("ArrowParent");
            //None‚ª‚ ‚é‚½‚ß-1
            var dirCount = Enum.GetValues(typeof(PressedKey)).Length - 1;
            var parentPos = arrowParent.transform.position;
            Func<PressedKey,(Vector3 pos,Quaternion rot,string keyName)> getTransform = (index) =>
            {
                return index switch
                {
                    PressedKey.Foward =>(parentPos + Vector3.forward * arrowOffset,Quaternion.identity,"W"),
                    PressedKey.Back => (parentPos + Vector3.back * arrowOffset,Quaternion.Euler(0f, 180f, 0f),"S"),
                    PressedKey.Right =>(parentPos + Vector3.right * arrowOffset,Quaternion.Euler(0f, 90f, 0f),"D"),
                    PressedKey.Left => (parentPos + Vector3.left * arrowOffset,Quaternion.Euler(0f, -90f, 0f),"A"),
                    _ => default
                };
            };
            for(int i = 1; i <= dirCount; i++) 
            {
                var keyType = (PressedKey)i;
                var values = getTransform(keyType);
                var pos = values.pos;
                var rot = values.rot;
                var arrow = Instantiate(helpArrowPrefab, pos, rot, arrowParent.transform);
                var mat = arrow.GetComponentInChildren<MeshRenderer>().material;
                pairs.Add((arrow,mat));
                var newColor = mat.color;
                newColor.a = 0f;
                mat.color = newColor;
            }
        }
        void SetupHelpUI(MeshRenderer renderer,string keyName)
        {
            var rawSize = renderer.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            var boundsSize = Vector3.Scale(rawSize, renderer.transform.lossyScale);
            var z = boundsSize.z;
        }
        void StartHelpAction()
        {
            if (isHelping) return;
            arrowParent.transform.position = player.transform.position + Vector3.up * offsetY;
            isHelping = true;
            var targetAlpha = 1.0f;
            pairs.ForEach(pair =>
            {
                pair.arrow.SetActive(true);
                var tween = DOTween.To(
                () => pair.mat.color.a,
                a =>
                {
                    var newColor = pair.mat.color;
                    newColor.a = a;
                    pair.mat.color = newColor;
                },
                targetAlpha,
                duration              
                );
                currentTweens.Add(tween);
            });
        }
        void HelpClose()
        {
            if (!isHelping) return;
            elapsedTime = 0f;
            isHelping = false;
            currentTweens.ForEach(tween => { tween?.Kill();});
            pairs.ForEach(pair =>
            {
                var newColor = pair.mat.color;
                newColor.a = 0f;
                pair.mat.color = newColor;
                pair.arrow.SetActive(false);
            });
            currentTweens.Clear();
            currentTweens.TrimExcess();
        }
    }
}

