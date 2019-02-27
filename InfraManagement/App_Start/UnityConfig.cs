using InfraManagement.Database.Entity;
using InfraManagement.Models;
using InfraManagement.Services;
using System;
using System.Configuration;
using Unity;

namespace InfraManagement
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<IPaymentGateway, AuthorizeDotNetService>( new Unity.Injection.InjectionConstructor( ConfigurationManager.AppSettings.Get("PaymentGateway.Login"),
                ConfigurationManager.AppSettings.Get("PaymentGateway.Password"),
                ConfigurationManager.AppSettings.Get("PaymentGateway.EndPoint")));

            container.RegisterType<ICloudService, VCloudService>(new Unity.Injection.InjectionConstructor(ConfigurationManager.AppSettings.Get("VCloud.EndPoint"),
                ConfigurationManager.AppSettings.Get("VCloud.ApiVersion"),
                ConfigurationManager.AppSettings.Get("VCloud.VdcTemplateId"),
                ConfigurationManager.AppSettings.Get("VCloud.Login"),
                ConfigurationManager.AppSettings.Get("VCloud.Password")
                ));

           
        }
    }
}