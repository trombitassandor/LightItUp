using System;
using LightItUp.Data;
using LightItUp.Sound;
using UnityEngine;

namespace LightItUp.Game
{
    public class SeekingMissileController : PooledObject
    {
        [SerializeField] private Transform start;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 boundSize = Vector3.one;
        [SerializeField] private float cameraTargetTime = 0.5f;

        [Header("Movement")]
        [SerializeField] private Vector2 arcPeakHeightRange = new Vector2(3f, 7f);
        [SerializeField] private Vector2 speedRange = new Vector2(0.3f, 0.6f);
        [SerializeField] private float avoidanceDetectionRadius = 5f;
        [SerializeField] private float avoidanceReflectAngleModifier = 1.5f;

        public Action<SeekingMissileController> OnDestroy; 

        private Vector3 startPosition;
        private bool isPathCompleted = false;
        private bool isFired = false;
        private float moveTime;
        private bool isDestroyed = false;
        private int avoidanceCount = 0;

        private float speed;
        private float arcPeakHeight;
        private Vector3 avoidanceDirection;

        public Rect WorldRect => new Rect(transform.position, boundSize);
        public float CameraTargetTime => cameraTargetTime;

        public void Init(Transform start, Transform target)
        {
            isPathCompleted = false;
            isFired = false;
            isDestroyed = false;
            this.start = start;
            this.target = target;
            transform.position = start.position;
            avoidanceCount = 0;
        }

        public void Fire()
        {
            speed = Random(speedRange);
            arcPeakHeight = Random(arcPeakHeightRange);
            isFired = true;
            startPosition = start.position;
            moveTime = 0f;
            SoundManager.PlaySound("SeekingMissile");
        }

        public void Destroy()
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
                ObjectPool.GetSeekingMissileHitPs().transform.position = transform.position;
                ObjectPool.ReturnSeekingMissile(this);
                OnDestroy?.Invoke(this);
            }
        }

        private void OnEnable()
        {
            PlayerController.StateChangedEvent += OnPlayerStateChanged;
        }

        private void OnDisable()
        {
            PlayerController.StateChangedEvent -= OnPlayerStateChanged;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var blockController = collision.collider.GetComponentInParent<BlockController>();

            if (blockController == null || blockController.transform != target)
            {
                return;
            }

            blockController.PlayerHit();

            var force = -collision.contacts[0].normal * 100;
            blockController.rb2d.AddForce(force);

            SoundManager.PlaySound("SeekingMissile");

            Destroy();
        }

        private void Update()
        {
            if (!isFired)
            {
                return;
            }

            if (target == null || isPathCompleted)
            {
                Destroy();
                return;
            }

            Move();
        }

        private void OnPlayerStateChanged(PlayerController.State state)
        {
            if (state != PlayerController.State.Normal)
            {
                Destroy();
            }
        }

        private void Move()
        {
            moveTime += Mathf.Clamp01(Time.deltaTime * speed);
            var targetPosition = target.position;
            var newLinearPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            var yVerticalArcParabolicMotion = newLinearPosition.y + arcPeakHeight * Mathf.Sin(moveTime * Mathf.PI);
            var newParabolicPosition = new Vector3(newLinearPosition.x, yVerticalArcParabolicMotion, newLinearPosition.z);
            var avoidanceDirection = CalculateAvoidanceDirection(newParabolicPosition);

            transform.position = avoidanceDirection;

            isPathCompleted = Vector3.Distance(transform.position, targetPosition) < 0.1f;
        }

        private Vector3 CalculateAvoidanceDirection(Vector3 newPosition)
        {
            var rayDirection = (newPosition - transform.position).normalized;
            var layerMasks = LayerMask.GetMask("Solid", "SolidLit", "CollectibleTrigger", "CollectibleBody");
            var hit = Physics2D.CircleCast(transform.position,
                avoidanceDetectionRadius, rayDirection, avoidanceDetectionRadius,
                layerMasks);

            if (avoidanceCount == 0 && (!hit.collider || IsTargetBlock(hit.transform)))
            {
                return newPosition;
            }

            if (avoidanceCount > 0)
            {
                --avoidanceCount;
            }
            else
            {
                avoidanceCount = UnityEngine.Random.Range(10, 20);
                //avoidanceDirection = Vector3.Reflect(rayDirection, hit.normal).normalized;
                var reflectionDirection = Vector2.Reflect(rayDirection, hit.normal);
                var directionToAdjust = (reflectionDirection - hit.normal).normalized;
                avoidanceDirection = reflectionDirection + directionToAdjust * avoidanceReflectAngleModifier;
            }

            var newAvoidancePosition = Vector3.MoveTowards(transform.position,
                transform.position + avoidanceDirection, speed * Time.deltaTime);

            return newAvoidancePosition;
        }

        private bool IsTargetBlock(Transform obj)
        {
            var blockController = obj.GetComponentInParent<BlockController>();

            if (blockController == null)
            {
                return false;
            }

            return blockController.transform == target;
        }

        private float Random(Vector2 value)
        {
            return UnityEngine.Random.Range(value.x, value.y);
        }
    }
}