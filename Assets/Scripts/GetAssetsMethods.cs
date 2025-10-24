using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IAssetSetter
{
    UniTask GetAsset();
}

public static class GetAssetsMethods
{
    public static async UniTask<T> GetAsset<T>(string key) where T : UnityEngine.Object
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.ToUniTask();
        T result = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result
                  : default(T);
        return result;
    }
}
