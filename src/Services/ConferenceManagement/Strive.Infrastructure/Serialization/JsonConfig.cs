using System.Linq;
using Autofac;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Infrastructure.Extensions;

namespace Strive.Infrastructure.Serialization
{
    public static class JsonConfig
    {
        public static JsonSerializerSettings Default
        {
            get
            {
                var settings = new JsonSerializerSettings();
                Apply(settings);
                return settings;
            }
        }

        public static void Apply(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            settings.Converters.Add(GetSceneConverter());
        }

        private static JsonConverter GetSceneConverter()
        {
            var baseType = typeof(AutonomousScene);

            var sceneTypes = baseType.Assembly.GetTypes()
                .Where(x => x.IsInNamespace(baseType.Namespace!) && x.IsAssignableTo<IScene>());

            var builder = JsonSubtypesConverterBuilder.Of<IScene>("type");
            foreach (var sceneType in sceneTypes)
            {
                var key = sceneType.Name.Replace("Scene", null).ToCamelCase();
                builder.RegisterSubtype(sceneType, key);
            }

            return builder.SerializeDiscriminatorProperty().Build();
        }
    }
}
