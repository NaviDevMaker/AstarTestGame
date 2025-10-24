#if UNITY_EDITOR
namespace AutoSave
{
	public class AutoSave : UnityEditor.AssetModificationProcessor
	{
		static string[] OnWillSaveAssets(string[] paths)
		{
			AutoSaveProvider.Backup();
			return paths;
		}
	}
}
#endif