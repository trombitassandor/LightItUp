using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightItUp.Data;
using UnityEngine;

namespace LightItUp.Game
{
    public class SeekingMissilesLauncher : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireDelaysInSeconds = 0.1f;

        private List<SeekingMissileController> missiles;

        private void Awake()
        {
            missiles = new List<SeekingMissileController>();
        }

        public void FireSeekingMissiles()
        {
            var closestUnlitBlocks = GetClosestUnlitBlocks();
            var seekingMissilesCount =
                BoosterService.Instance.boosterConfig.SeekingMissilesCount;

            for (int i = 0; i < seekingMissilesCount; i++)
            {
                var targetIndex = i < closestUnlitBlocks.Count ? i : closestUnlitBlocks.Count - 1;
                var target = closestUnlitBlocks[targetIndex];
                var delayInSeconds = fireDelaysInSeconds * i;
                StartCoroutine(FireSeekingMissile(target.transform, delayInSeconds));
            }
        }

        public IReadOnlyList<SeekingMissileController> GetActiveMissiles()
        {
            return missiles.AsReadOnly();
        }

        private List<BlockController> GetClosestUnlitBlocks()
        {
            var blocks = GameManager.Instance.currentLevel.blocks;

            var unlitBlocks = blocks.Where(block => !block.IsLit);

            var closestUnlitBlocks = unlitBlocks.OrderBy((block) =>
            {
                return ((Vector2)(block.transform.position - transform.position)).magnitude;
            });

            var closestUnlitRegularBlocks = closestUnlitBlocks.OrderBy((block) =>
            {
                return block.HasSpikes || block.IsBomb;
            });

            return closestUnlitRegularBlocks.ToList();
        }

        private IEnumerator FireSeekingMissile(Transform target, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            var seekingMissileController = ObjectPool.GetSeekingMissile();
            seekingMissileController.Init(firePoint, target);
            seekingMissileController.Fire();
            missiles.Add(seekingMissileController);
            seekingMissileController.OnDestroy += OnMissileDestroy;
        }

        private void OnMissileDestroy(SeekingMissileController missile)
        {
            missiles.Remove(missile);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}