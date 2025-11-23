using UnityEngine;

namespace Game.Stage
{

    [CreateAssetMenu]
    public class MapSettingDatas : ScriptableObject
    {
        public bool isSetuped;

        [SerializeField] bool isSetupedSizeW;
        public int mapSizeW;
        [SerializeField] bool isSetupedSizeH;
        public int mapSizeH;
        [SerializeField] bool isSetupedDefaultPosition;
        public Vector3 defaultPostion;

        public bool IsSetupedSizeW  => isSetupedSizeW;
        public bool IsSetupedSizeH  => isSetupedSizeH;
        public bool IsSetupedDefaultPosition => isSetupedDefaultPosition; 
    }
}
