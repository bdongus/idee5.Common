using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;

namespace idee5.Common.Net46.Plugins {
    /// <summary>
    /// Simple generic plugin loader. Searches the subdirectory "Plugins" for classes implementing
    /// the given type
    /// </summary>
    /// <typeparam name="TContract">The type of the plugins.</typeparam>
    public class Mef2PluginLoader<TContract> : IPluginLoader<TContract> where TContract : IPluginContract {
        public IEnumerable<TContract> Plugins { get; protected set; }
        private CompositionContainer _container;
        private FilteredCatalog _filteredCatalog;

        public void LoadPlugins(PluginConfiguration config)
        {
            // Use RegistrationBuilder to set up our MEF parts.
            var regBuilder = new RegistrationBuilder();
            regBuilder.ForTypesDerivedFrom<IPluginContract>().Export<TContract>().SetCreationPolicy(CreationPolicy.NonShared);

            // We look only for the property called "Plugins"
            Predicate<PropertyInfo> propertyFilter = pi => pi.Name.Equals(value: "Plugins");
            System.Action<PropertyInfo, ImportBuilder> importConfiguration = (propInfo, builder) => {
                builder.AsMany();
                builder.AllowRecomposition();
                //builder.RequiredCreationPolicy(CreationPolicy.NonShared);
                //and many other configurations...
            };

            PartBuilder pb = regBuilder.ForType<Mef2PluginLoader<TContract>>();
            pb.Export();
            pb.ImportProperties(propertyFilter, importConfiguration);

            var aggCat = new AggregateCatalog();

            if (config.SearchRecursive) {
                var dcat = new RecursiveDirectoryCatalog(config.PluginsPath, regBuilder);
                _filteredCatalog = dcat.Filter(c => c.ExportDefinitions.Any(e => config.PluginsEnabled[e.ToString().ExtractString(beginDelim: ".", endDelim: " ")]));
            } else {
                var dcat = new DirectoryCatalog(config.PluginsPath, regBuilder);
                // Filter plugins by the class name
                _filteredCatalog = dcat.Filter(c => c.ExportDefinitions.Any(e => config.PluginsEnabled[e.ToString().ExtractString(beginDelim: ".", endDelim: " ")]));
            }
            aggCat.Catalogs.Add(_filteredCatalog);

            _container = new CompositionContainer(aggCat);
            Plugins = _container.GetExportedValues<TContract>();
        }
    }
}