using System;
using System.Linq;
using Autofac;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Strive.Core.Services.Poll;
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
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver
                {NamingStrategy = new CamelCaseNamingStrategy(true, true)};
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(true, true)));
            settings.Converters.Add(GetSceneConverter());
            settings.Converters.Add(new ErrorConverter());
            ConfigurePollConverters(settings);

            // This is a hack to accomplish the following:
            // we want to preserve the casing of dictionary keys (so dont camel case keys as they might be case sensitive ids etc.)
            // but we also want to convert enums to camel case
            // A potential fix would've been to set CamelCaseNamingStrategy.ProcessDictionaryKeys to false, but the problem is that 
            // enum values are then also not converted to camel case. This converter fixes it by serializing all dictionaries Dictionary<string,any>
            // without changing the keys
            settings.Converters.Add(new DictionaryStringKeyPreserveCasingConverter());
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

        private static void ConfigurePollConverters(JsonSerializerSettings settings)
        {
            settings.Converters.Add(CreateJsonConverter<PollInstruction>(typeof(PollInstruction), "type",
                x => x.TrimEnd("Instruction").ToCamelCase()));

            settings.Converters.Add(CreateJsonConverter<PollAnswer>(typeof(PollAnswer), "type",
                x => x.TrimEnd("Answer").ToCamelCase()));

            settings.Converters.Add(CreateJsonConverter<PollResults>(typeof(PollResults), "type",
                x => x.TrimEnd("Results").ToCamelCase()));
        }

        private static JsonConverter CreateJsonConverter<T>(Type baseType, string discriminatorProperty,
            Func<string, string> classNameFactory)
        {
            var subTypes = baseType.Assembly.GetTypes().Where(x =>
                x.IsInNamespace(baseType.Namespace!) && !x.IsAbstract && x.IsAssignableTo<T>());

            var builder = JsonSubtypesConverterBuilder.Of<T>(discriminatorProperty);
            foreach (var type in subTypes)
            {
                var key = classNameFactory(type.Name);
                builder.RegisterSubtype(type, key);
            }

            return builder.SerializeDiscriminatorProperty().Build();
        }
    }
}
