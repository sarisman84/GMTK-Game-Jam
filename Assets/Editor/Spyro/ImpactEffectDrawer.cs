// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using Player;
// using TPUModelerEditor;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Editor.PropertyDrawers
// {
//     [CustomPropertyDrawer(typeof(ImpactEffect))]
//     public class ImpactEffectDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property,
//             GUIContent label)
//         {
//             int index = EditorGUI.Popup(position, "Impact Type:", GetCurrentTypeAsIndex(property),
//                 GetAllTypesAsDisplayOptions(property));
//             EditorGUI.LabelField(position, GetCurrentTypeAsName(property));
//             if (!property.objectReferenceValue)
//                 SelectAndCreateImpactEffect(index, property);
//             if (!property.objectReferenceValue) return;
//             position.y += EditorGUIUtility.singleLineHeight *1.15f;
//             position.height = EditorGUIUtility.singleLineHeight;
//             EditorGUI.PropertyField(position, property);
//             property.serializedObject.ApplyModifiedProperties();
//         }
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return base.GetPropertyHeight(property, label) * (2f + 1.15f);
//         }
//
//         private string[] GetAllTypesAsDisplayOptions(SerializedProperty serializedProperty)
//         {
//             Type[] foundTypes = InheritedTypesOfProperty(serializedProperty);
//             string[] result = new string[foundTypes.Length];
//
//             for (var i = 0; i < foundTypes.Length; i++)
//             {
//                 var type = foundTypes[i];
//                 result[i] = type.FullName.Replace("Player.", "");
//             }
//
//             return result;
//         }
//
//         private int GetCurrentTypeAsIndex(SerializedProperty serializedProperty)
//         {
//             Type[] foundTypes = InheritedTypesOfProperty(serializedProperty);
//             for (int i = 0; i < foundTypes.Length; i++)
//             {
//                 if (serializedProperty.objectReferenceValue &&
//                     serializedProperty.objectReferenceValue.GetType() == foundTypes[i])
//                     return i;
//             }
//
//             return -1;
//         }
//
//         private string GetCurrentTypeAsName(SerializedProperty property)
//         {
//             if (property.objectReferenceValue)
//                 return property.objectReferenceValue.GetType().FullName.Replace("Player.", "");
//             return "";
//         }
//
//         private void SelectAndCreateImpactEffect(int popup, SerializedProperty property)
//         {
//             Type[] foundTypes = InheritedTypesOfProperty(property);
//             for (int i = 0; i < foundTypes.Length; i++)
//             {
//                 if (i == popup)
//                 {
//                     property.objectReferenceValue =
//                         CreateAsset(foundTypes[i], "Assets/Settings/Weapon Settings/Impact", true);
//                     return;
//                 }
//             }
//         }
//
//         private string PrintInputedTypes(Type[] inheritedTypesOfProperty)
//         {
//             string output = "Current Types are: ";
//             foreach (var iType in inheritedTypesOfProperty)
//             {
//                 output += $" {iType.Name}";
//             }
//
//             return output;
//         }
//
//         private Type[] InheritedTypesOfProperty(SerializedProperty property)
//         {
//             return GetTypesFromType<ImpactEffect>();
//         }
//
//
//         private ScriptableObject CreateAsset(Type type, string path, bool selectCreatedAsset = false)
//         {
//             //Taken from Jon Skeet:  https://stackoverflow.com/questions/4667981/c-sharp-use-system-type-as-generic-parameter
//             // Get the generic type definition
//             MethodInfo method = typeof(ScriptableObject).GetMethods(BindingFlags.Public | BindingFlags.Static)
//                 .Where(x => x.Name == "CreateInstance").FirstOrDefault(x => x.IsGenericMethod);
//
//             // Build a method with the specific type argument you're interested in
//
//             method = method.MakeGenericMethod(type);
//             // The "null" is because it's a static method
//             ScriptableObject scriptableObject = (ScriptableObject) method.Invoke(null, null);
//             Debug.Log(scriptableObject);
//             AssetDatabase.CreateAsset(scriptableObject,
//                 path +
//                 $"/{(string.IsNullOrEmpty(scriptableObject.name) ? "Impact Effect" : scriptableObject.name)}.asset");
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//             if (selectCreatedAsset)
//             {
//                 EditorUtility.FocusProjectWindow();
//                 Selection.activeObject = scriptableObject;
//             }
//
//             return scriptableObject;
//         }
//
//         //Taken from Jacobs Data Solutions: https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class/6944605
//         public static Type[] GetTypesFromType<T>() where T : class, IComparable<T>
//         {
//             List<Type> objects = new List<Type>();
//             Type[] allTypes = Assembly.GetAssembly(typeof(T)).GetTypes();
//
//
//             foreach (Type type in
//                 allTypes.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
//             {
//                 objects.Add(type);
//             }
//
//             objects.Sort();
//             return objects.ToArray();
//         }
//     }
// }