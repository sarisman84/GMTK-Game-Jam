using System;
using System.Collections.Generic;
using General;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameMaster : MonoBehaviour
    {
        private static GameMaster _ins;

        public static GameMaster singletonAccess
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
        private Scene _playerScene;
        public Scene playerScene => _playerScene;

        public event Action ONUpdate;
        public event Action ONPlayerGameOver;


        public GameObject playerObject => _playerController;
        public PossessionManager possessor => _playerController.GetComponent<PossessionManager>();
        public HealthModifier playerHealth => _playerController.GetComponent<HealthModifier>();
        public ExitFinder playerCompass => _playerController.GetComponent<ExitFinder>();
        public WeaponController playerWeaponManager => _playerController.GetComponent<WeaponController>();

        public void InitializePlayer(GameObject controller, Vector3 position)
        {
            _playerController = ObjectPooler.DynamicInstantiate(controller, position, Quaternion.identity, 1);
            _playerController = _playerController.GetComponentInChildren<PlayerController>().gameObject;
        }

        public void RegisterPlayerScene(Scene scene)
        {
            _playerScene = scene;
        }


        public T GetNearestObjectOfType<T>(GameObject center, float radius,
            List<T> blacklist = null, params string[] mask) where T : MonoBehaviour
        {
            if (blacklist == null)
                blacklist = new List<T>();
            Debug.Log($"Checking nearby area with {LayerMask.GetMask(mask)}");
            Collider[] foundElements = Physics.OverlapSphere(center.transform.position, radius, LayerMask.GetMask(mask));
            if (foundElements == null) return null;
            float minDist = float.MaxValue;
            T result = null;
            foreach (var foundElement in foundElements)
            {
                if (foundElement.GetComponent<T>() is { } foundElementOfType &&
                    foundElementOfType.gameObject.GetInstanceID() != center.GetInstanceID() &&
                    !blacklist.Contains(foundElementOfType))
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

        public Vector3 GetRandomPositionAroundPoint(Vector3 point, float radius)
        {
            Vector3 randomResult = point + Random.onUnitSphere * (radius);
            randomResult = new Vector3(Mathf.Clamp(randomResult.x, point.x - radius, point.x + radius), 0,
                Mathf.Clamp(randomResult.z, point.z - radius, point.z + radius));
            return randomResult;
        }

        public void ClearUpdateEvents()
        {
            ONUpdate = null;
        }

        private void Update()
        {
            ONUpdate?.Invoke();
        }
    }
}