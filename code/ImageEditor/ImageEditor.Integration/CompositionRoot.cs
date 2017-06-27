using System;

using ImageEditor.Interface.ViewModel;
using ImageEditor.ViewModel;

using Autofac;

namespace ImageEditor.Integration
{
    public class CompositionRoot
    {
        private IMainViewModel _MainViewModel;

        public IMainViewModel MainViewModel
        {
            get
            {
                if (_MainViewModel == null)
                {
                    var builder = new ContainerBuilder();
                    var viewModelsAssembly = typeof(MainViewModel).Assembly;

                    builder.RegisterAssemblyTypes(viewModelsAssembly)
                        .Where(x => x.Name.EndsWith("ViewModel", StringComparison.InvariantCultureIgnoreCase))
                        .AsImplementedInterfaces()
                        .PropertiesAutowired()
                        .SingleInstance();

                    var container = builder.Build();

                    _MainViewModel = container.Resolve<IMainViewModel>();
                }

                return _MainViewModel;
            }
        }
    }
}
