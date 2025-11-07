using UnityEngine;
using System.Reflection;
using System.Linq;
using Game.Player;
using UnityEngine.Events;
using System;
public interface IDamageable
{
    void TakeDamage(int damage);
}
public static class DamageProvider
{
    //本来はDamageableのチェック見れば一発だけど勉強の為
    public static void TryAddDamage(GameObject other,int damage)
    {
        if (other == null) return;
        var components = other.GetComponents<MonoBehaviour>();
        foreach(var cmp in components)
        {
            var otherType = cmp.GetType();
            var interfaces = otherType.GetInterfaces();//インターフェイスの取得
            interfaces.ToList().ForEach(i => Debug.Log($"インターフェースの名前:{i.Name}"));
            //if (interfaces.Length == 0) return;
            var iplayerInterface = interfaces
                                  .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPlayer<>));//インターフェイスがジェネリックタイプかの確認とそのタイプがIPlayer
            if (iplayerInterface == null) continue;
            var invincibleProp = otherType.GetProperty("isInvincible", BindingFlags.Instance | BindingFlags.Public);
            var deadProp = otherType.GetProperty("isDead", BindingFlags.Instance | BindingFlags.Public);
            var isInvincible = Convert.ToBoolean(invincibleProp.GetValue(cmp));//不確定の要素（今回はobject型だからboolかはわからない）の時に使う、value がどんな型でも、true/false に変換できるなら変換したい時に使う
            var isDead = Convert.ToBoolean(deadProp.GetValue(cmp));// ,,
            if (isInvincible || isDead) return;
            var takeDamageMethod = otherType.GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.Public);
            var shakeCameraProp = otherType.GetProperty("OnHitEnemyAction", BindingFlags.Instance | BindingFlags.Public);
            if (takeDamageMethod == null || shakeCameraProp == null) continue;
            var action = shakeCameraProp.GetValue(cmp) as UnityAction;
            action?.Invoke();
            takeDamageMethod.Invoke(cmp, new object[] { damage });
            break;
        }
    }
}
