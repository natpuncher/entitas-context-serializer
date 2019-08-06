using System;
using Ecs.Serialization.Infractructure;
using Entitas;
using Newtonsoft.Json;

namespace Ecs.Serialization.Implementations
{
	public class JsonComponentSerializer : IComponentSerializer
	{
		public object Serialize(IComponent component)
		{
			return JsonConvert.SerializeObject(component);
		}

		public IComponent Deserialize(object componentData, Type componentType)
		{
			return JsonConvert.DeserializeObject((string)componentData, componentType) as IComponent;
		}
	}
}