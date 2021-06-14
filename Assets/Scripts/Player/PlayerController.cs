using System;
using System.Collections;
using Enemies;
using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private WeaponController _weaponController;
        private PossessionManager _possessionManager;
        private AbilityManager _abilityManager;
        private HealthModifier _health;
        private ExitFinder _exitFinder;

        private Coroutine _tracker;

        public Transform model;
        public InputActionAsset inputAsset;
        public float maxMovementSpeed;
        public float accelerationSpeed;


        public WeaponController WeaponManager => _weaponController;
        public PossessionManager PossessionManager => _possessionManager;
        public HealthModifier HealthManager => _health;
        public AbilityManager AbilityManager => _abilityManager;
        public float currentAccelerationSpeed { get; set; }
        public float currentMaxMovementSpeed { get; set; }
        
        

        private void Awake()
        {
            CustomInput.ImportAsset(inputAsset,
                CustomInput.DirectionalKeys,
                "DirectionalShot",
                "Possess",
                "Fire",
                "ScrollSelect",
                "Select_One",
                "Select_Two",
                "Select_Three",
                "Select_Four",
                "Select_Five",
                "Select_Six",
                "Select_Seven",
                "Select_Eight",
                "Select_Nine");
            _rigidbody = GetComponent<Rigidbody>();
            _weaponController = GetComponent<WeaponController>();
            _weaponController.SetDesiredTarget(typeof(BaseEnemy));
            _possessionManager = GetComponent<PossessionManager>();
            _health = GetComponent<HealthModifier>();
            _abilityManager = GetComponent<AbilityManager>();
            _exitFinder = GetComponent<ExitFinder>();

            _possessionManager.ONPossessionEvent += enemy =>
                _weaponController.AddWeaponToLibrary(enemy.GetComponent<WeaponController>().weaponLibrary[0]);


            currentAccelerationSpeed = accelerationSpeed;
            currentMaxMovementSpeed = maxMovementSpeed;
        }

        private void Update()
        {
            if (_weaponController)
            {
                int val = (int) CustomInput.GetSingleDirectionInput("ScrollSelect");
                val = val == 0 ? 0 : (int) Mathf.Sign(val);
                if (val != 0)
                    _weaponController.SelectWeapon(_weaponController.CurrentWeapon + val);
                _weaponController.Aim(CustomInput.GetDirectionalInput("DirectionalShot"));
                _weaponController.Shoot(CustomInput.GetButton("Fire"));
            }

            if (_possessionManager)
            {
                _possessionManager.ShootPossessionShot(CustomInput.GetButton("Possess"));
            }

            if (_abilityManager)
            {
                _abilityManager.UseCurrentAbility(this, CustomInput.GetKeyDown(
                    "Select_One",
                    "Select_Two",
                    "Select_Three",
                    "Select_Four",
                    "Select_Five",
                    "Select_Six",
                    "Select_Seven",
                    "Select_Eight",
                    "Select_Nine"));
            }

            Vector3 movementDir = _rigidbody.velocity.normalized;
            model.rotation = Quaternion.LookRotation(movementDir, Vector3.up);
        }

        private void FixedUpdate()
        {
            Vector2 rawInput = CustomInput.GetDirectionalInput(CustomInput.DirectionalKeys);


            if (rawInput != Vector2.zero)
            {
                rawInput *= currentAccelerationSpeed * Time.fixedDeltaTime;
                _rigidbody.velocity += new Vector3(rawInput.x, 0, rawInput.y);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, currentMaxMovementSpeed);
            }
            else
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, 0.55f);
        }


        private void OnDisable()
        {
            CustomInput.SetInputActive(false);
        }

        public void SetControllerActive(bool state, bool isFlaggedForDeath)
        {
            _exitFinder.SetActive(state);
            _weaponController.displayer.SetActive(state);
            _possessionManager.SetUIActive(state);
            _possessionManager.SetPossessionsActive(state);
            _abilityManager.display.SetActive(state);
            if (isFlaggedForDeath)
            {
                _health.DestroyThis();
                return;
            }


            gameObject.SetActive(state);
        }

        public IEnumerator TeleportPlayerToPos(Vector3 pos)
        {
            gameObject.transform.position = pos;
            PossessionManager.TeleportPossessionsToPosition(pos);
            yield return null;
        }

        public void UpdateExitTracker(Detector foundDetector)
        {
            if(_tracker != null)
                StopCoroutine(_tracker);
            _tracker = StartCoroutine(UpdateTracker(foundDetector));
        }

        public void ManualUpdateExitTracker(Detector foundDetector)
        {
            if (!_exitFinder) return;
            _exitFinder.SetTarget(foundDetector);
        }

        private IEnumerator UpdateTracker(Detector foundDetector)
        {
            if (!_exitFinder) yield break;
            _exitFinder.SetTarget(null);

            while (true)
            {
                _exitFinder.SetTarget(
                    _possessionManager.possessedEntities.Count >= foundDetector.currentPossessionRequired
                        ? foundDetector
                        : null);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}