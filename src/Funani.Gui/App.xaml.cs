
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

using Catel.IoC;
using Catel.Logging;
using Catel.Windows;

using Funani.Api;
using Funani.Engine;
using Funani.Gui.ViewModels;

namespace Funani.Gui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            //LogManager.RegisterDebugListener();
#endif
            log4net.Config.XmlConfigurator.Configure();
            StyleHelper.CreateStyleForwardersForDefaultStyles();

            // ensure the UI elements are drawn using the current UI culture
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));

            ServiceLocator.Default.RegisterType<IEngine, FunaniEngine>();
            ServiceLocator.Default.RegisterType<ICommandQueue, FunaniCommandQueue>();

            var modelView = new MongoDbViewModel(Dispatcher);
            ServiceLocator.Default.RegisterInstance<IConsoleRedirect>(modelView);

            base.OnStartup(e);
        }
    }
}