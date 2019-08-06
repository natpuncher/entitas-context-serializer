using System;
using System.Collections.Generic;

namespace Ecs.Serialization.Infractructure
{
	public class ContextData
	{
		public Type[] Types;
		public List<List<ComponentData>> Entities;
	}
}