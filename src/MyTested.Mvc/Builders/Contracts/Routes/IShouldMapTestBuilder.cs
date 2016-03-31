﻿namespace MyTested.Mvc.Builders.Contracts.Routes
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Used for building and testing a route.
    /// </summary>
    public interface IShouldMapTestBuilder : IResolvedRouteTestBuilder
    {
        IAndResolvedRouteTestBuilder ToAction(string actionName);

        IAndResolvedRouteTestBuilder ToController(string controllerName);

        /// <summary>
        /// Tests whether the built route is resolved to the action provided by the expression.
        /// </summary>
        /// <typeparam name="TController">Type of expected resolved controller.</typeparam>
        /// <param name="actionCall">Method call expression indicating the expected resolved action.</param>
        /// <returns>The same route test builder.</returns>
        IAndResolvedRouteTestBuilder To<TController>(Expression<Action<TController>> actionCall)
            where TController : class;
        
        /// <summary>
        /// Tests whether the built route cannot be resolved.
        /// </summary>
        void ToNonExistingRoute();
    }
}
