﻿using System;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Web.Mvc.Auditing;
using Abp.Web.Mvc.Authorization;
using Abp.Web.Mvc.Configuration;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.ModelBinding.Binders;
using Abp.Web.Mvc.Resources.Embedded;
using Abp.Web.Mvc.Security.AntiForgery;
using Abp.Web.Mvc.Uow;
using Abp.Web.Mvc.Validation;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Mvc
{
    /// <summary>
    /// 该模块用于使用Abp构建ASP.NET MVC网站。
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebMvcModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());

            IocManager.Register<IAbpMvcConfiguration, AbpMvcConfiguration>();

            Configuration.ReplaceService<IAbpAntiForgeryManager, AbpMvcAntiForgeryManager>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
            HostingEnvironment.RegisterVirtualPathProvider(IocManager.Resolve<EmbeddedResourceVirtualPathProvider>());
        }

        /// <inheritdoc/>
        public override void PostInitialize()
        {
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcAuthorizeFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpAntiForgeryMvcFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcAuditFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcValidationFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcUowFilter>());

            var abpMvcDateTimeBinder = new AbpMvcDateTimeBinder();
            ModelBinders.Binders.Add(typeof(DateTime), abpMvcDateTimeBinder);
            ModelBinders.Binders.Add(typeof(DateTime?), abpMvcDateTimeBinder);
        }
    }
}
