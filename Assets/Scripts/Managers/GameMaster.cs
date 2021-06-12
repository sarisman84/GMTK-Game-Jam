using System.Collections.Generic;
using Player;
using UnityEngine;
using Utility;

namespace Managers
{
    public class GameMaster : MonoBehaviour
    {
        private static GameMaster _ins;

        public static GameMaster SingletonAccess
        {
            get
            {
                if (!_ins)
                {
                    _ins = FindObjectOfType<GameMaster>() is { } gm
                        ? gm
                        : new GameObject("Game Master").AddComponent<GameMaster>();
                    _ins._playerController = FindObjectOfType<PlayerController>()?.gameObject;
                }


                return _ins;
            }
        }

        private GameObject _playerController;

        public GameObject GetPlayer()
        {
            if (!_playerController)
                return null;
            return _playerController;
        }

        public void InitializePlayer(GameObject controller, Vector3 position)
        {
            _playerController = ObjectPooler.DynamicInstantiate(controller, position, Quaternion.identity, 1);
            _playerController = _playerController.GetComponentInChildren<PlayerController>().gameObject;
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