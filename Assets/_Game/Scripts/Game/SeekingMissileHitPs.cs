using LightItUp.Data;
using UnityEngine;

namespace LightItUp.Game
{
    public class SeekingMissileHitPs : PooledObject
    {
        private void Start()
        {
            var main = GetComponent<ParticleSystem>().main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            ObjectPool.ReturnSeekingMissileHitPs(this);
        }
    }
}