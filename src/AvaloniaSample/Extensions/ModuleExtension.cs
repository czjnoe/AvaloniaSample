using AvaloniaSample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Extensions
{
    /// <summary>
    /// 一键扫描程序集并注册 IModule
    /// </summary>
    public static class ModuleExtension
    {
        public static void ModuleRegister(
            this IModuleCatalog moduleCatalog,
            params Assembly[] assemblies)
        {
            if (moduleCatalog is not ModuleCatalog catalog)
                throw new ArgumentException(
                    "IModuleCatalog must be ModuleCatalog");

            if (assemblies == null || assemblies.Length == 0)
                return;

            foreach (var assembly in assemblies.Distinct())
            {
                RegisterFromAssembly(catalog, assembly);
            }
        }

        private static void RegisterFromAssembly(
            ModuleCatalog catalog,
            Assembly assembly)
        {
            var moduleTypes = assembly.GetTypes()
                .Where(t =>
                    typeof(IModule).IsAssignableFrom(t) &&
                    t.IsClass &&
                    !t.IsAbstract);

            foreach (var type in moduleTypes)
            {
                RegisterModule(catalog, type);
            }
        }

        private static void RegisterModule(
            ModuleCatalog catalog,
            Type moduleType)
        {
            var moduleName = moduleType.FullName!;
            var attribute = moduleType.GetCustomAttribute<AutoModuleAttribute>();

            // 防止重复注册
            if (catalog.Modules.Any(m => m.ModuleName == moduleName))
                return;

            var moduleInfo = new ModuleInfo
            {
                ModuleName = moduleName,
                ModuleType = moduleType.AssemblyQualifiedName!,
                InitializationMode =
                    attribute?.InitializationMode
                    ?? InitializationMode.WhenAvailable,
            };

            if (attribute?.DependsOn?.Length > 0)
            {
                foreach (var depend in attribute.DependsOn)
                {
                    moduleInfo.DependsOn.Add(depend);
                }
            }

            catalog.AddModule(moduleInfo);
        }
    }

    /// <summary>
    /// 自动模块注册特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AutoModuleAttribute : Attribute
    {
        public InitializationMode InitializationMode { get; }
        public string[] DependsOn { get; }

        public AutoModuleAttribute(
            InitializationMode initializationMode = InitializationMode.WhenAvailable,
            params string[] dependsOn)
        {
            InitializationMode = initializationMode;
            DependsOn = dependsOn ?? Array.Empty<string>();
        }
    }
}
