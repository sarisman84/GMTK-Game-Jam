using System;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Agent))]
    public class IntelligentEnemy : BaseEnemy
    {
        private Agent _agent;
        public Agent AStarAgent => _agent;

        public override float movementSpeed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        protected override void Awake()
        {
            _agent = GetComponent<Agent>();
            base.Awake();
        }

        public override void SetTarget(Type type)
        {
            Debug.Log($"[{name}]: Setting target");
            base.SetTarget(type);
            _agent.target = GetTargetFromDetectionArea() is { } targetFound ? targetFound.transform : null;
            
        }

        private void Update()
        {
            _agent.target = GetTargetFromDetectionArea() is { } targetFound ? targetFound.transform : null;
            UseWeapon(_agent.target ? _agent.target.gameObject : null);
        }
    }
}