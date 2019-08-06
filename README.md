Required Odin Inspector for config scriptable object

Setup:
1. Create EcsSerializationConfig scriptable object anywhere in your project by pressing 'Assets/Create/Ecs Serialization Config'
2. Create new lookup info in it and select your context type and component lookup type
3. Add [Serializable] attribute on any component you want to be serialized
4. Update EcsSerializationConfig by pressing button on it or 'Tools/Entitas/Update EcsSerializationConfig'
5. Create an default instance of context serializer var myContextSerializer = new ContextSerializer<MyContextType, MyEntityType>(new MyComponentSerializer(), EcsSerializationConfigLink)
6. Call var contextData = myContextSerializer.Serialize(MyContextInstance) to serialize you context data and store it wherever you want, for example convert it to json and place into PlayerPrefs
7. Load and deserialize you context data and call myContextSerializer.Deserialize(loadedContextData, MyContextInstance)

Don't forget to update EcsSerializationConfig every time after Entitas code gen!
