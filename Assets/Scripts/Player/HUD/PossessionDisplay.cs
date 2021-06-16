using System;
using System.Collections.Generic;
using Enemies;
using TMPro;
using UnityEngine;
using Utility;

namespace Player.HUD
{
    public class PossessionDisplay : MonoBehaviour
    {
        public TMP_Text textDisplay;
        public ParticleSystem possessionTether;

        private Dictionary<BaseEnemy, ParticleSystem> _possessionTethers = new();

        public void UpdatePossesionDisplay(List<BaseEnemy> possessedEnemies)
        {
            if (textDisplay)
                textDisplay.text = $"Possessed Enemies: {possessedEnemies.Count}";
        }

        public void CreatePossessionTether(BaseEnemy possessedEnemy)
        {
            ParticleSystem tether = ObjectPooler.DynamicInstantiate(possessionTether, transform.parent);
            _possessionTethers.Add(possessedEnemy, tether);
        }


        public void SetActive(bool state)
        {
            textDisplay.gameObject.SetActive(state);
            SetTetherActive(state);
        }

        private void Update()
        {
            UpdateTether();
        }

        private void UpdateTether()
        {
            foreach (var tether in _possessionTethers)
            {
                if (!tether.Key.gameObject.activeSelf)
                {
                    tether.Value.gameObject.SetActive(false);
                    continue;
                }
                
                ParticleSystem.ShapeModule shape = tether.Value.shape;
                ParticleSystem.EmissionModule emission = tether.Value.emission;

                Vector3 dir = (tether.Key.transform.position - transform.position);
                tether.Value.transform.position = transform.position +
                                                  GetCenterPositionBetweenTwoPoints(tether.Key.transform.position,
                                                      transform.position);
                shape.scale = new Vector3(1, 1, dir.magnitude);
                Quaternion rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
                rotation.ToAngleAxis(out var angle, out var axis);
                shape.rotation = axis * angle;

                Transform model = tether.Value.transform.GetChild(0);
                model.localScale = new Vector3(
                    model.localScale.x, model.localScale.y,
                    dir.magnitude);
                model.localRotation = rotation;
                emission.rateOverTime = new ParticleSystem.MinMaxCurve((dir.magnitude));
            }
        }

        public void ResetTether()
        {
            SetTetherActive(false);

            _possessionTethers = new Dictionary<BaseEnemy, ParticleSystem>();
        }

        public void SetTetherActive(bool state)
        {
            foreach (var tether in _possessionTethers)
            {
                tether.Value.gameObject.SetActive(state);
            }
        }

        private Vector3 GetCenterPositionBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return (a - b) / 2f;
        }
    }
}