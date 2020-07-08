﻿namespace Game2DWaterKit.Ripples
{
    using Game2DWaterKit.Mesh;
    using Game2DWaterKit.Ripples.Effects;
    using Game2DWaterKit.Simulation;
    using Game2DWaterKit.Main;
    using Game2DWaterKit.AttachedComponents;
    using UnityEngine;
    using UnityEngine.Events;

    public class WaterCollisionRipplesModule
    {
        #region Variables
        private readonly Transform _onCollisionRipplesEffectsPoolsRoot;
        private readonly WaterRipplesParticleEffect _onWaterEnterRipplesParticleEffect;
        private readonly WaterRipplesParticleEffect _onWaterExitRipplesParticleEffect;
        private readonly WaterRipplesSoundEffect _onWaterEnterRipplesSoundEffect;
        private readonly WaterRipplesSoundEffect _onWaterExitRipplesSoundEffect;

        private bool _isOnWaterEnterRipplesActive;
        private bool _isOnWaterExitRipplesActive;
        private bool _collisionIgnoreTriggers;
        private float _minimumDisturbance;
        private float _maximumDisturbance;
        private float _velocityMultiplier;
        private LayerMask _collisionMask;
        private float _collisionMinimumDepth;
        private float _collisionMaximumDepth;
        private float _collisionRaycastMaximumDistance;
        private UnityEvent _onWaterEnter;
        private UnityEvent _onWaterExit;

        private Game2DWater _waterObject;
        private WaterMeshModule _meshModule;
        private WaterMainModule _mainModule;
        private WaterSimulationModule _simulationModule;

        private RaycastHit2D[] _raycastResults = new RaycastHit2D[8];
        #endregion

        public WaterCollisionRipplesModule(Game2DWater waterObject, WaterCollisionRipplesModuleParameters parameters, Transform ripplesEffectsPoolsRootParent)
        {
            _waterObject = waterObject;

            _isOnWaterEnterRipplesActive = parameters.ActivateOnWaterEnterRipples;
            _isOnWaterExitRipplesActive = parameters.ActivateOnWaterExitRipples;
            _collisionIgnoreTriggers = parameters.CollisionIgnoreTriggers;
            _minimumDisturbance = parameters.MinimumDisturbance;
            _maximumDisturbance = parameters.MaximumDisturbance;
            _velocityMultiplier = parameters.VelocityMultiplier;
            _collisionMask = parameters.CollisionMask;
            _collisionMinimumDepth = parameters.CollisionMinimumDepth;
            _collisionMaximumDepth = parameters.CollisionMaximumDepth;
            _collisionRaycastMaximumDistance = parameters.CollisionRaycastMaxDistance;
            _onWaterEnter = parameters.OnWaterEnter;
            _onWaterExit = parameters.OnWaterExit;

            _onCollisionRipplesEffectsPoolsRoot = CreateRipplesEffectsPoolsRoot(ripplesEffectsPoolsRootParent);

            _onWaterEnterRipplesParticleEffect = new WaterRipplesParticleEffect(parameters.WaterEnterParticleEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterExitRipplesParticleEffect = new WaterRipplesParticleEffect(parameters.WaterExitParticleEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterEnterRipplesSoundEffect = new WaterRipplesSoundEffect(parameters.WaterEnterSoundEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
            _onWaterExitRipplesSoundEffect = new WaterRipplesSoundEffect(parameters.WaterExitSoundEffectParameters, _onCollisionRipplesEffectsPoolsRoot);
        }

        #region Properties
        public bool IsOnWaterEnterRipplesActive { get { return _isOnWaterEnterRipplesActive; } set { _isOnWaterEnterRipplesActive = value; } }
        public bool IsOnWaterExitRipplesActive { get { return _isOnWaterExitRipplesActive; } set { _isOnWaterExitRipplesActive = value; } }
        public bool CollisionIgnoreTriggers { get { return _collisionIgnoreTriggers; } set { _collisionIgnoreTriggers = value; } }
        public LayerMask CollisionMask { get { return _collisionMask; } set { _collisionMask = value; } }
        public float CollisionMaximumDepth { get { return _collisionMaximumDepth; } set { _collisionMaximumDepth = value; } }
        public float CollisionMinimumDepth { get { return _collisionMinimumDepth; } set { _collisionMinimumDepth = value; } }
        public float CollisionRaycastMaximumDistance { get { return _collisionRaycastMaximumDistance; } set { _collisionRaycastMaximumDistance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MaximumDisturbance { get { return _maximumDisturbance; } set { _maximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumDisturbance { get { return _minimumDisturbance; } set { _minimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public float MinimumVelocityToCauseMaximumDisturbance { get { return _maximumDisturbance / _velocityMultiplier; } set { if (value != 0f) VelocityMultiplier = _maximumDisturbance / value; } }
        public UnityEvent OnWaterEnter { get { return _onWaterEnter; } set { _onWaterEnter = value; } }
        public UnityEvent OnWaterExit { get { return _onWaterExit; } set { _onWaterExit = value; } }
        public float VelocityMultiplier { get { return _velocityMultiplier; } set { _velocityMultiplier = Mathf.Clamp(value, 0f, float.MaxValue); } }
        public WaterRipplesParticleEffect OnWaterEnterRipplesParticleEffect { get { return _onWaterEnterRipplesParticleEffect; } }
        public WaterRipplesParticleEffect OnWaterExitRipplesParticleEffect { get { return _onWaterExitRipplesParticleEffect; } }
        public WaterRipplesSoundEffect OnWaterEnterRipplesSoundEffect { get { return _onWaterEnterRipplesSoundEffect; } }
        public WaterRipplesSoundEffect OnWaterExitRipplesSoundEffect { get { return _onWaterExitRipplesSoundEffect; } }
        #endregion

        #region Methods

        internal void Initialize()
        {
            _mainModule = _waterObject.MainModule;
            _meshModule = _waterObject.MeshModule;
            _simulationModule = _waterObject.SimulationModule;
        }

        internal void ResolveCollision(Collider2D objectCollider, bool isObjectEnteringWater)
        {
            if ((isObjectEnteringWater && !_isOnWaterEnterRipplesActive) || (!isObjectEnteringWater && !_isOnWaterExitRipplesActive))
                return;

            if (_collisionMask != (_collisionMask | (1 << objectCollider.gameObject.layer)))
                return;

            if (_collisionIgnoreTriggers && objectCollider.isTrigger)
                return;

            Vector3[] vertices = _meshModule.Vertices;
            Vector3 raycastDirection = _mainModule.UpDirection;

            int surfaceVerticesCount = _meshModule.SurfaceVerticesCount;
            int subdivisionsPerUnit = _meshModule.SubdivisionsPerUnit;

            Bounds objectColliderBounds = objectCollider.bounds;
            float minColliderBoundsX = _mainModule.TransformPointWorldToLocal(objectColliderBounds.min).x;
            float maxColliderBoundsX = _mainModule.TransformPointWorldToLocal(objectColliderBounds.max).x;

            int firstSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? 1 : 0;
            int lastSurfaceVertexIndex = _simulationModule.IsUsingCustomBoundaries ? surfaceVerticesCount - 2 : surfaceVerticesCount - 1;
            float firstSurfaceVertexPosition = vertices[firstSurfaceVertexIndex].x;
            int startIndex = Mathf.Clamp(Mathf.RoundToInt((minColliderBoundsX - firstSurfaceVertexPosition) * subdivisionsPerUnit), firstSurfaceVertexIndex, lastSurfaceVertexIndex);
            int endIndex = Mathf.Clamp(Mathf.RoundToInt((maxColliderBoundsX - firstSurfaceVertexPosition) * subdivisionsPerUnit), firstSurfaceVertexIndex, lastSurfaceVertexIndex);

            int hitPointsCount = 0;
            float hitPointsVelocitiesSum = 0f;
            Vector2 hitPointsPositionsSum = new Vector2(0f, 0f);

            for (int surfaceVertexIndex = startIndex; surfaceVertexIndex <= endIndex; surfaceVertexIndex++)
            {
                Vector2 surfaceVertexPosition = _mainModule.TransformPointLocalToWorld(vertices[surfaceVertexIndex]);

                var raycastOrigin = new Vector2(surfaceVertexPosition.x, objectColliderBounds.min.y - 0.01f);
                var raycastDistance = Mathf.Abs(surfaceVertexPosition.y - raycastOrigin.y) + _collisionRaycastMaximumDistance;

                var hitCount = Physics2D.RaycastNonAlloc(raycastOrigin, raycastDirection, _raycastResults, raycastDistance, _collisionMask, _collisionMinimumDepth, _collisionMaximumDepth);

                if (hitCount > 0)
                {
                    int index = GetObjectColliderIndexInRaycastResults(objectCollider, hitCount);

                    if (index < 0)
                        continue;

                    RaycastHit2D hit = _raycastResults[index];

                    float velocity = hit.rigidbody.GetPointVelocity(hit.point).y * _velocityMultiplier;
                    velocity = Mathf.Clamp(Mathf.Abs(velocity), _minimumDisturbance, _maximumDisturbance);
                    _simulationModule.DisturbSurfaceVertex(surfaceVertexIndex, velocity * (isObjectEnteringWater ? -1f : 1f));
                    hitPointsVelocitiesSum += velocity;
                    hitPointsPositionsSum += hit.point;
                    hitPointsCount++;
                }
            }

            if (hitPointsCount > 0)
            {
                _simulationModule.MarkVelocitiesArrayAsChanged();

                Vector2 hitPointsPositionsMean = hitPointsPositionsSum / hitPointsCount;
                Vector3 spawnPosition = new Vector3(hitPointsPositionsMean.x, hitPointsPositionsMean.y, _mainModule.Position.z);

                float hitPointsVelocitiesMean = hitPointsVelocitiesSum / hitPointsCount;
                float disturbanceFactor = Mathf.InverseLerp(_minimumDisturbance, _maximumDisturbance, hitPointsVelocitiesMean);

                if (isObjectEnteringWater)
                {
                    OnWaterEnterRipplesParticleEffect.PlayParticleEffect(spawnPosition);
                    OnWaterEnterRipplesSoundEffect.PlaySoundEffect(spawnPosition, disturbanceFactor);

                    if (_onWaterEnter != null)
                        _onWaterEnter.Invoke();
                }
                else
                {
                    OnWaterExitRipplesParticleEffect.PlayParticleEffect(spawnPosition);
                    OnWaterExitRipplesSoundEffect.PlaySoundEffect(spawnPosition, disturbanceFactor);

                    if (_onWaterExit != null)
                        _onWaterExit.Invoke();
                }
            }
        }

        internal void Update()
        {

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            _onWaterEnterRipplesSoundEffect.Update();
            _onWaterEnterRipplesParticleEffect.Update();
            _onWaterExitRipplesSoundEffect.Update();
            _onWaterExitRipplesParticleEffect.Update();
        }

        private int GetObjectColliderIndexInRaycastResults(Collider2D objectCollider, int hitCount)
        {
            for (int i = 0, imax = hitCount; i < imax; i++)
            {
                if (_raycastResults[i].collider == objectCollider)
                    return i;
            }

            return -1;
        }

        private static Transform CreateRipplesEffectsPoolsRoot(Transform parent)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return null;
#endif
            var root = new GameObject("OnCollisionRipplesEffects").transform;
            root.parent = parent;
            return root;
        }

        #endregion

        #region Editor Only Methods
#if UNITY_EDITOR
        internal void Validate(WaterCollisionRipplesModuleParameters parameters)
        {
            IsOnWaterEnterRipplesActive = parameters.ActivateOnWaterEnterRipples;
            IsOnWaterExitRipplesActive = parameters.ActivateOnWaterExitRipples;
            CollisionIgnoreTriggers = parameters.CollisionIgnoreTriggers;
            MinimumDisturbance = parameters.MinimumDisturbance;
            MaximumDisturbance = parameters.MaximumDisturbance;
            VelocityMultiplier = parameters.VelocityMultiplier;
            CollisionMask = parameters.CollisionMask;
            CollisionMinimumDepth = parameters.CollisionMinimumDepth;
            CollisionMaximumDepth = parameters.CollisionMaximumDepth;
            CollisionRaycastMaximumDistance = parameters.CollisionRaycastMaxDistance;
            OnWaterEnter = parameters.OnWaterEnter;
            OnWaterExit = parameters.OnWaterExit;

            OnWaterEnterRipplesParticleEffect.Validate(parameters.WaterEnterParticleEffectParameters);
            OnWaterEnterRipplesSoundEffect.Validate(parameters.WaterEnterSoundEffectParameters);
            OnWaterExitRipplesParticleEffect.Validate(parameters.WaterExitParticleEffectParameters);
            OnWaterExitRipplesSoundEffect.Validate(parameters.WaterExitSoundEffectParameters);
        }
#endif
        #endregion
    }

    public struct WaterCollisionRipplesModuleParameters
    {
        public bool ActivateOnWaterEnterRipples;
        public bool ActivateOnWaterExitRipples;
        public bool CollisionIgnoreTriggers;
        public float MinimumDisturbance;
        public float MaximumDisturbance;
        public float VelocityMultiplier;
        public LayerMask CollisionMask;
        public float CollisionMinimumDepth;
        public float CollisionMaximumDepth;
        public float CollisionRaycastMaxDistance;
        public UnityEvent OnWaterEnter;
        public UnityEvent OnWaterExit;
        public WaterRipplesParticleEffectParameters WaterEnterParticleEffectParameters;
        public WaterRipplesSoundEffectParameters WaterEnterSoundEffectParameters;
        public WaterRipplesParticleEffectParameters WaterExitParticleEffectParameters;
        public WaterRipplesSoundEffectParameters WaterExitSoundEffectParameters;
    }

}
