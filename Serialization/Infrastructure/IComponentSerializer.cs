using System;
using Entitas;

namespace Ecs.Serialization.Infractructure
{
	public interface IComponentSerializer
	{
		object Serialize(IComponent component);
		IComponent Deserialize(object componentData, Type componentType);
	}
}