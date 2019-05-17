using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class EntityPipeline : IEntityGameObjectCreator
    {
        private const string GameobjectNameFormat = "{0}(SpatialOS {1}, Worker: {2})";
        private const string WorkerAttributeFormat = "workerId:{0}";
        private const string PlayerMetadata = "Player";

        private Worker _worker;
        private IEntityGameObjectCreator _fallback;
        private Dictionary<EntityId, GameObject> _gameObjectsCreated = new Dictionary<EntityId, GameObject>();

        private readonly string _workerIdAttribute;
        private readonly GameObject _authPlayerPrefab;
        private readonly GameObject _nonAuthPlayerPrefab;

        public EntityPipeline(
            Worker worker,
            GameObject authPlayerPrefab,
            GameObject nonAuthPlayerPrefab,
            IEntityGameObjectCreator fallback)
        {
            _worker = worker;
            _fallback = fallback;
            _authPlayerPrefab = authPlayerPrefab;
            _nonAuthPlayerPrefab = nonAuthPlayerPrefab;
            _workerIdAttribute = EntityTemplate.GetWorkerAccessAttribute(_worker.WorkerId);
        }

        public void OnEntityCreated(SpatialOSEntity entity, EntityGameObjectLinker linker)
        {
            if (!entity.HasComponent<Metadata.Component>())
            {
                return;
            }

            var prefabName = entity.GetComponent<Metadata.Component>().EntityType;
            if (prefabName.Equals(PlayerMetadata))
            {
                var clientMovement = entity.GetComponent<Position.Component>();
                if (entity.GetComponent<EntityAcl.Component>().ComponentWriteAcl
                    .TryGetValue(clientMovement.ComponentId, out var clientMovementWrite))
                {
                    var authority = false;
                    foreach (var attributeSet in clientMovementWrite.AttributeSet)
                    {
                        if (attributeSet.Attribute.Contains(_workerIdAttribute))
                        {
                            authority = true;
                        }
                    }

                    var serverPosition = entity.GetComponent<Position.Component>();
                    var position = serverPosition.Coords.ToUnityVector() + _worker.Origin;

                    var prefab = authority ? _authPlayerPrefab : _nonAuthPlayerPrefab;
                    var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);

                    _gameObjectsCreated.Add(entity.SpatialOSEntityId, gameObject);
                    gameObject.name = GetGameObjectName(prefab, entity, _worker);
                    linker.LinkGameObjectToSpatialOSEntity(entity.SpatialOSEntityId, gameObject, typeof(Transform));
                    return;
                }
            }

            _fallback.OnEntityCreated(entity, linker);
        }

        public void OnEntityRemoved(EntityId entityId)
        {
            if (!_gameObjectsCreated.TryGetValue(entityId, out var go))
            {
                _fallback.OnEntityRemoved(entityId);
                return;
            }

            _gameObjectsCreated.Remove(entityId);
            Object.Destroy(go);
        }

        private static string GetGameObjectName(GameObject prefab, SpatialOSEntity entity, Worker worker)
        {
            return string.Format(GameobjectNameFormat, prefab.name, entity.SpatialOSEntityId, worker.WorkerType);
        }
    }
}
