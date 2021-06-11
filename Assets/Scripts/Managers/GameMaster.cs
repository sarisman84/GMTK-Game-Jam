using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Managers
{
    public class GameMaster : MonoBehaviour
    {
        private static GameMaster _ins;

        public static GameMaster SingletonAccess
        {
            get
            {
                _ins = !_ins
                    ? FindObjectOfType<GameMaster>() is { } gm ? gm :
                    new GameObject("Game Master").AddComponent<GameMaster>()
                    : _ins;
                _ins._playerController = FindObjectOfType<PlayerController>();
                return _ins;
            }
        }

        private PlayerController _playerController;

        public GameObject GetPlayer()
        {
            return _playerController.gameObject;
        }

        public T GetNearestObjectOfType<T>(GameObject center, float radius, LayerMask mask,
            List<GameObject> blacklist = null) where T : MonoBehaviour
        {
            if (blacklist == null)
                blacklist = new List<GameObject>();
            Collider[] foundElements = Physics.OverlapSphere(center.transform.position, radius, mask);
            if (foundElements == null) return null;
            float minDist = float.MaxValue;
            T result = null;
            foreach (var foundElement in foundElements)
            {
                if (foundElement.GetComponent<T>() is { } foundElementOfType &&
                    foundElementOfType.gameObject.GetInstanceID() != center.GetInstanceID() &&
                    !blacklist.Contains(foundElementOfType.gameObject))
                {
                    float dist = Vector3.Distance(foundElementOfType.transform.position, center.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        result = foundElementOfType;
                    }
                }
            }

            return result;
        }
    }
}