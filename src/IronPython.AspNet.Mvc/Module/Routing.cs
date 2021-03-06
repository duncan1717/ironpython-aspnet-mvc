﻿using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace IronPython.AspNet.Mvc
{
    /// <summary>
    /// Python asp.net mvc api
    /// </summary>
    public static partial class AspNetMvcAPI
    {
        /// <summary>
        /// Handle routing options
        /// </summary>
        [PythonType("Routing")]
        public static class Routing
        {
            /// <summary>
            /// Module description
            /// </summary>
            public const string __doc__ = "Module which represents the main asp.net mvc ironpython application";

            private static PythonDictionary __controllers;

            /// <summary>
            /// Create routing system
            /// </summary>
            static Routing()
            {
                __controllers = new PythonDictionary();
            }

            /// <summary>
            /// Detect and register all available controller in the system
            /// </summary>
            /// <param name="context">CodeContext, passed from IronPython automatically</param>
            public static void register_all(CodeContext context)
            {
                // Get all Controller in the current scope
                var items = MvcApplication.Host.DefaultScope.ScriptScope.GetItems();

                var pythonController = DynamicHelpers.GetPythonTypeFromType(typeof(AspNetMvcAPI.Controller));

                foreach (var item in items)
                {
                    // Check type and get controller
                    if (item.Value is PythonType)
                    {
                        var pt = (PythonType)item.Value;
                        var baseType = pt.__getattribute__(context, "__base__");

                        // If is controlelr
                        if (baseType == pythonController)
                        {
                            // Add to controller list
                            __controllers.Add(item.Key.Replace("Controller", ""), pt);
                        }
                    }
                }

                // Register routes
                var routes = System.Web.Routing.RouteTable.Routes;
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


                routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );

                IControllerFactory factory = new CustomControllerFactory();
                ControllerBuilder.Current.SetControllerFactory(factory);
            }

            /// <summary>
            /// List of available controller
            /// </summary>
            public static PythonDictionary controllers
            {
                get
                {
                    return __controllers;
                }
            }
        }
    }
}