using System;
using System.Collections.Generic;
using System.Linq;
using Ecs.Serialization.Infractructure;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ecs.Serialization
{
	[CreateAssetMenu]
	public class EcsSerializationConfig : SerializedScriptableObject
	{
		[SerializeField] private List<LookupInfo> LookupTypeNames;
		[ReadOnly, SerializeField, Required] private Dictionary<Type, Type[]> _savedTypes;
		[ReadOnly, SerializeField, Required] private Dictionary<Type, Dictionary<int, Type>> _actualIndexToType;
		[ReadOnly, SerializeField, Required] private Dictionary<Type, Dictionary<Type, int>> _typeToSavedIndex;
		[ReadOnly, SerializeField, Required] private Dictionary<Type, Dictionary<Type, int>> _typeToActualIndex;

		public Type[] GetSavedTypes(Type contextType)
		{
			return _savedTypes.TryGetValue(contextType, out var serializedComponents) ? serializedComponents : null;
		}

		public Dictionary<int, Type> GetActualIndexToType(Type contextType)
		{
			return _actualIndexToType.TryGetValue(contextType, out var serializedComponents) ? serializedComponents : null;
		}

		public Dictionary<Type, int> GetTypeToSavedIndex(Type contextType)
		{
			return _typeToSavedIndex.TryGetValue(contextType, out var serializedComponents) ? serializedComponents : null;
		}
		
		public Dictionary<Type, int> GetTypeToActualIndex(Type contextType)
		{
			return _typeToActualIndex.TryGetValue(contextType, out var serializedComponents) ? serializedComponents : null;
		}

#if UNITY_EDITOR
		private const string TypesFieldName = "componentTypes";

		[UnityEditor.MenuItem("Tools/Entitas/Update EcsSerializationConfig %&v")]
		public static void Generate()
		{
			var config = LoadEcsSerializationConfig();
			if (config == null)
			{
				Debug.LogError("Can't find EcsSerializationConfig");
				return;
			}

			config.SearchForSerializableComponents();
			UnityEditor.EditorUtility.SetDirty(config);
			Debug.Log("EcsSerializationConfig updated!");
		}

		[Button(ButtonSizes.Large)]
		public void SearchForSerializableComponents()
		{
			_actualIndexToType = new Dictionary<Type, Dictionary<int, Type>>();
			_typeToSavedIndex = new Dictionary<Type, Dictionary<Type, int>>();
			_typeToActualIndex = new Dictionary<Type, Dictionary<Type, int>>();
			_savedTypes = new Dictionary<Type, Type[]>();

			var serializableType = typeof(SerializableAttribute);

			foreach (var lookupInfo in LookupTypeNames)
			{
				var field = lookupInfo.LookupType.GetField(TypesFieldName);
				var componentTypes = field.GetValue(null) as Type[];
				FindIndexes(lookupInfo.ContextType, componentTypes, serializableType);
			}
		}
		
		private static EcsSerializationConfig LoadEcsSerializationConfig()
		{
			return UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(EcsSerializationConfig).Name)).Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
				.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<EcsSerializationConfig>).FirstOrDefault();
		}

		private void FindIndexes(Type lookupInfoContextType, Type[] componentTypes, Type attributeType)
		{
			_actualIndexToType.Add(lookupInfoContextType, new Dictionary<int, Type>());
			_typeToSavedIndex.Add(lookupInfoContextType, new Dictionary<Type, int>());
			_typeToActualIndex.Add(lookupInfoContextType, new Dictionary<Type, int>());

			var componentTypesLength = componentTypes.Length;
			for (var i = 0; i < componentTypesLength; i++)
			{
				var index = i;
				var componentType = componentTypes[i];
				if (componentType.IsDefined(attributeType, true) && !_actualIndexToType[lookupInfoContextType].ContainsKey(index))
				{
					_typeToSavedIndex[lookupInfoContextType].Add(componentType, _actualIndexToType[lookupInfoContextType].Count);
					_actualIndexToType[lookupInfoContextType].Add(index, componentType);
					_typeToActualIndex[lookupInfoContextType].Add(componentType, index);
				}
			}

			_savedTypes.Add(lookupInfoContextType, _typeToSavedIndex[lookupInfoContextType].Keys.ToArray());
		}
#endif
	}
}