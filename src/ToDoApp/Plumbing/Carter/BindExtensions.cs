using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Carter;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ToDoApp.Plumbing.Carter
{
    public static class BindExtensions
    {
        static JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            NullValueHandling = NullValueHandling.Ignore
        });
        
        static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> PropertiesByModel
            = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        public static async Task<T> AutoBind<T>(this HttpContext ctx) where T : new()
        {
            // bind from body
            var model = new T();

            if (ctx.Request.ContentLength != null)
            {
                await using var memStream = new MemoryStream();
                await ctx.Request.Body.CopyToAsync(memStream, ctx.RequestAborted);
                memStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(memStream);
                using var jsonReader = new JsonTextReader(reader);
                model = Serializer.Deserialize<T>(jsonReader);
            }
            
            // get props
            var props = PropertiesByModel.GetOrAdd(
                typeof(T), typeof(T)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(p => p.Name.ToLower(), p => p)
            );

            // bind from route
            foreach (var (key, value) in ctx.GetRouteData().Values)
                SetValue(key, value);

            // bind from query
            foreach (var (key, value) in ctx.Request.Query)
                SetValue(key, value);

            return model;

            void SetValue(string key, object value)
            {
                var property = props[GetPropertyName(key)];
                property?.SetValue(model, ConvertValue(property, value));
            }

            // convert parameter names from kebab-case and snake_case to lower case.
            static string GetPropertyName(string key) => key.Replace("-", string.Empty).Replace("_", string.Empty).ToLower();

            // convert value to typed one
            static object ConvertValue(PropertyInfo property, object value)
            {
                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                if (underlyingType != null)
                    return value == null ? default : Convert.ChangeType(value, underlyingType);

                if (property.PropertyType.IsArray)
                    return ((StringValues) value).ToArray();

                if (value is StringValues values)
                    value = values.Single();

                return Convert.ChangeType(value, property.PropertyType);
            }
        }

        public static async Task<(ValidationResult ValidationResult, T Data)> AutoBindAndValidate<T>(this HttpContext ctx) where T : new()
        {
            var model     = await ctx.AutoBind<T>();
            var validator = ctx.RequestServices.GetService<IValidatorLocator>().GetValidator<T>();

            return validator != null
                ? (validator.Validate(model), model)
                : (new ValidationResult(), model);
        }
    }
}