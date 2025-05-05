using System;
using System.Collections.Generic;

namespace AgaveCaseStudy
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Clear()
        {
            services.Clear();
        }

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
                services[type] = service;
            else
                services.Add(type, service);
        }

        public static T Resolve<T>()
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
                return (T)service;
            throw new Exception($"Service of type {type} is not registered.");
        }

        public static bool IsRegistered<T>()
        {
            return services.ContainsKey(typeof(T));
        }
    }
} 