using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Manila.Plugin {
	class PluginLoadContext : AssemblyLoadContext {
		private AssemblyDependencyResolver res;

		public PluginLoadContext(string pluginPath) {
			res = new AssemblyDependencyResolver(pluginPath);
		}

		protected override Assembly Load(AssemblyName assemblyName) {
			string assemblyPath = res.ResolveAssemblyToPath(assemblyName);
			if (assemblyPath != null) {
				return LoadFromAssemblyPath(assemblyPath);
			}

			return null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
			string libraryPath = res.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (libraryPath != null) {
				return LoadUnmanagedDllFromPath(libraryPath);
			}

			return IntPtr.Zero;
		}
	}
}
