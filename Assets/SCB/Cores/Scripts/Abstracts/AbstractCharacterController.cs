using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCB.Cores
{
    public abstract class AbstractCharacterController<T> : MonoBehaviour where T : Component
    {
        public abstract void Move(Vector3 direction);
        public abstract void Jump();
        public abstract void Attack();
        public abstract void Skill();
        public abstract void Die();
        public abstract void Animate(string animationName);
    }
}
