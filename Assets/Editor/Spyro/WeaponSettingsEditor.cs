using Player;
using UnityEditor;

namespace Editor.PropertyDrawers
{
    [CustomEditor(typeof(WeaponSettings))]
    public class WeaponSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
        }
    }
}