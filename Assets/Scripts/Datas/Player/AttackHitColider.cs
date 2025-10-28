using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game.Player
{
    public class AttackHitColider : MonoBehaviour
    {
        PlayerController owner;
        EnemyController targetEnemy;
        Func<bool> OnAttacking;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!OnAttacking.Invoke())
            {
                targetEnemy = null;
                return;
            }
            if (targetEnemy != null) return;
            if (other.TryGetComponent<EnemyController>(out var enemy)) targetEnemy = enemy;
        }
    }
}

