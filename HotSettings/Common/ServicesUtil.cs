using Microsoft.VisualStudio.ComponentModelHost;
using System;

namespace HotSettings
{
    public class ServicesUtil
    {
        public static T GetMefService<T>(IServiceProvider serviceProvider) where T : class
        {
            IComponentModel componentModel = serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            return componentModel?.GetService<T>();
        }
    }
}
