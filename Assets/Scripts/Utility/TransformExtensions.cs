using UnityEngine;

namespace Utility
{
    public static class TransformExtensions
    {
        public static Transform FindChildOfTag(this Transform transform, string tag)
        {
            Transform result = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.childCount > 0)
                {
                    result = FindChildOfTag(child, tag);
                }

                if (child.CompareTag(tag))
                {
                    result = child;
                    break;
                }
            }

            return result;
        }
    }
}