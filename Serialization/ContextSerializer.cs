using System;
using System.Collections.Generic;
using Ecs.Serialization.Infractructure;
using Entitas;

namespace Ecs.Serialization
{
	public class ContextSerializer<TContext, TEntity> where TContext : IContext<TEntity> where TEntity : class, IEntity
	{
		private readonly IComponentSerializer _componentSerializer;
		private readonly EcsSerializationConfig _ecsSerializationConfig;

		public ContextSerializer(IComponentSerializer componentSerializer, EcsSerializationConfig ecsSerializationConfig)
		{
			_componentSerializer = componentSerializer;
			_ecsSerializationConfig = ecsSerializationConfig;
		}

		public ContextData Serialize(TContext context)
		{
			var contextType = typeof(TContext);
			
			var savedTypes = _ecsSerializationConfig.GetSavedTypes(contextType);
			var actualIndexToType = _ecsSerializationConfig.GetActualIndexToType(contextType);
			var typeToSavedIndex = _ecsSerializationConfig.GetTypeToSavedIndex(contextType);

			var entities = context.GetEntities();
			var entitiesLength = entities.Length;
			var data = new ContextData {Types = savedTypes, Entities = new List<List<ComponentData>>(entitiesLength)};

			for (var i = 0; i < entitiesLength; i++)
			{
				var entityData = SerializeEntity(entities[i], actualIndexToType, typeToSavedIndex);
				if (entityData.Count > 0)
				{
					data.Entities.Add(entityData);
				}
			}

			return data;
		}

		public void Deserialize(ContextData data, TContext context)
		{
			if (data == null)
			{
				return;
			}

			var contextType = typeof(TContext);

			var typeToActualIndex = _ecsSerializationConfig.GetTypeToActualIndex(contextType);
			
			var savedTypes = data.Types;
			var dataCount = data.Entities.Count;
			for (var i = 0; i < dataCount; i++)
			{
				var entityData = data.Entities[i];
				var entity = context.CreateEntity();
				var componentsCount = entityData.Count;
				for (var index = 0; index < componentsCount; index++)
				{
					var componentData = entityData[index];
					
					var savedTypeIndex = componentData.T;
					var savedType = savedTypes[savedTypeIndex];

					if (!typeToActualIndex.TryGetValue(savedType, out var actualTypeIndex))
					{
						continue;
					}
					
					entity.AddComponent(actualTypeIndex, _componentSerializer.Deserialize(componentData.C, savedType));
				}
			}
		}

		private List<ComponentData> SerializeEntity(TEntity entity, Dictionary<int, Type> actualToSavedIndexes, Dictionary<Type, int> typeToSavedIndex)
		{
			var entityData = new List<ComponentData>(entity.totalComponents);

			var componentIndexes = entity.GetComponentIndices();

			var componentIndexesLength = componentIndexes.Length;
			for (var index = 0; index < componentIndexesLength; index++)
			{
				var actualIndex = componentIndexes[index];
				if (!actualToSavedIndexes.ContainsKey(actualIndex))
				{
					continue;
				}

				if (!typeToSavedIndex.TryGetValue(actualToSavedIndexes[actualIndex], out var savedIndex))
				{
					continue;
				}

				entityData.Add(new ComponentData {T = savedIndex, C = _componentSerializer.Serialize(entity.GetComponent(actualIndex))});
			}

			return entityData;
		}
	}
}