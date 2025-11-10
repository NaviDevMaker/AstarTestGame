using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public struct Vector3TweenSetup
{
    public float duration { get; set; }
    public Vector3 endValue { get; set; }
    public Ease ease { get; set; }

    public Vector3TweenSetup(Vector3 endvalue, float duration, Ease ease = Ease.OutQuad)
    {
        this.duration = duration;
        this.endValue = endvalue;
        this.ease = ease;
    }
}

public struct Vector2TweenSetup
{
    public float duration { get; set; }
    public Vector2 endValue { get; set; }
    public Ease ease { get; set; }

    public Vector2TweenSetup(Vector2 endvalue, float duration, Ease ease = Ease.OutQuad)
    {
        this.duration = duration;
        this.endValue = endvalue;
        this.ease = ease;
    }
}

public struct FadeSet
{
    public float alpha { get; set; }
    public float duration { get; set; }

    public Ease ease { get; set; }

    public FadeSet(float alpha, float duration, Ease ease = Ease.InOutQuad)
    {
        this.alpha = alpha;
        this.duration = duration;
        this.ease = ease;
    }
}

public struct ShakeSet
{ 
    public float duration { get; set; }
    public float strength { get; set; }
    public int vibrato { get; set; }
    public float randomness { get; set; }

    public ShakeSet(float duration,float strength,int vibrato,float randomness)
    {
        this.duration = duration;
        this.strength = strength;
        this.vibrato = vibrato;
        this.randomness = randomness;
    }
}


public static class ObjectTweenFunctions
{
    public static Tween Scaler(this GameObject origin, Vector3TweenSetup tweenSetup)
    {
        var duration = tweenSetup.duration;
        var endValue = tweenSetup.endValue;
        var ease = tweenSetup.ease;
        var tween = origin.transform.DOScale(endValue, duration).SetEase(ease);
        return tween;
    }
    public static Tween Mover(this GameObject origin, Vector3TweenSetup tweenSetup)
    {
        var duration = tweenSetup.duration;
        var endValue = tweenSetup.endValue;
        var ease = tweenSetup.ease;
        var tween = origin.transform.DOMove(endValue, duration).SetEase(ease);
        return tween;
    }

    public static Tween RectMover(this Component origin, Vector2TweenSetup tweenSetup)
    {
        var duration = tweenSetup.duration;
        var endValue = tweenSetup.endValue;
        var ease = tweenSetup.ease;

        var rectTransform = origin.GetComponent<RectTransform>();
        if (rectTransform == null) return null;
        var tween = rectTransform.DOAnchorPos(endValue, duration).SetEase(ease);
        return tween;
    }
    public static Tween Roter(this GameObject origin, Vector3TweenSetup tweenSetup
                             , RotateMode rotateMode = RotateMode.Fast,bool isLocal = false)
    {
        var duration = tweenSetup.duration;
        var endValue = tweenSetup.endValue;
        var ease = tweenSetup.ease;
        var tween = !isLocal
                    ? origin.transform.DORotate(endValue, duration, rotateMode).SetEase(ease)
                    : origin.transform.DOLocalRotate(endValue, duration, rotateMode).SetEase(ease);
        return tween;
    }

    public static Tween Fader(this Graphic graphic, FadeSet fadeSet)
    {
        var duration = fadeSet.duration;
        var alpha = fadeSet.alpha;
        var ease = fadeSet.ease;
        var tween = graphic.DOFade(alpha, duration).SetEase(ease);
        return tween;
    }

    public static Tween Shaker(this GameObject origin,ShakeSet shakeSet
                              ,ShakeRandomnessMode shakeRandomnessMode = ShakeRandomnessMode.Full)
    {
        var duration = shakeSet.duration;
        var strength = shakeSet.strength;
        var vibrato = shakeSet.vibrato;
        var randomness = shakeSet.randomness;
        var tween = origin.transform.DOShakePosition(duration, strength, vibrato,randomness
                                                    ,false,true);
        return tween;
    }
}