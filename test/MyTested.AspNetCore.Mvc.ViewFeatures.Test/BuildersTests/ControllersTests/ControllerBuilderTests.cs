﻿namespace MyTested.AspNetCore.Mvc.Test.BuildersTests.ControllersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Builders.Contracts.Actions;
    using Builders.Contracts.Base;
    using Exceptions;
    using Internal.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Primitives;
    using Setups;
    using Setups.Controllers;
    using Setups.Models;
    using Setups.Services;
    using Xunit;

    public class ControllerBuilderTests
    {
        [Fact]
        public void WithResolvedDependencyForShouldWorkCorrectlyWithNullValues()
        {
            MyMvc
                .Controller<MvcController>()
                .WithServiceFor<IInjectedService>(null)
                .WithServiceFor<RequestModel>(null)
                .WithServiceFor<IAnotherInjectedService>(null)
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithResolvedDependencyForShouldNotThrowExceptionWithNullValuesAndMoreThanOneSuitableConstructor()
        {
            MyMvc
                .Controller<MvcController>()
                .WithServiceFor<IInjectedService>(null)
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithNoResolvedDependencyForShouldNotThrowException()
        {
            MyMvc
                .Controller<MvcController>()
                .WithNoServiceFor<IInjectedService>()
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithTempDataShouldPopulateTempDataCorrectly()
        {
            MyMvc
                .Controller<MvcController>()
                .WithTempData(tempData =>
                {
                    tempData.WithEntry("test", "value");
                })
                .Calling(c => c.TempDataAction())
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void WithTempDataShouldPopulateTempData()
        {
            MyMvc
                .Controller<MvcController>()
                .WithTempData(tempData => tempData
                    .WithEntry("key", "value"))
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.Equal(1, controller.TempData.Count);
                });
        }

        [Fact]
        public void WithResolvedDependencyForShouldWorkCorrectlyWithNullValuesForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor<IInjectedService>(null)
                .WithServiceFor<RequestModel>(null)
                .WithServiceFor<IAnotherInjectedService>(null)
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithResolvedDependencyForShouldNotThrowExceptionWithNullValuesAndMoreThanOneSuitableConstructorForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor<IInjectedService>(null)
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithNoResolvedDependencyForShouldNotThrowExceptionForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithNoServiceFor<IInjectedService>()
                .Calling(c => c.DefaultView())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void WithTempDataShouldPopulateTempDataCorrectlyForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithTempData(tempData =>
                {
                    tempData.WithEntry("test", "value");
                })
                .Calling(c => c.TempDataAction())
                .ShouldReturn()
                .Ok();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithTempDataShouldPopulateTempDataForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithTempData(tempData => tempData
                    .WithEntry("key", "value"))
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.Equal(1, controller.CustomTempData.Count);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void RouteDataShouldBePopulatedWhenRequestAndPathAreProvided()
        {
            MyMvc.IsUsingDefaultConfiguration();

            MyMvc
                .Controller<MvcController>()
                .WithHttpRequest(req => req.WithPath("/Mvc/WithRouteData/1"))
                .Calling(c => c.WithRouteData(1))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void RouteDataShouldBePopulatedWhenRequestAndPathAreProvidedForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpRequest(req => req.WithPath("/Mvc/WithRouteData/1"))
                .Calling(c => c.WithRouteData(1))
                .ShouldReturn()
                .View();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateCorrectActionDescriptorForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.OkResultAction())
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull((controller as FullPocoController).CustomControllerContext);
                    Assert.NotNull((controller as FullPocoController).CustomControllerContext.ActionDescriptor);
                    Assert.Equal("OkResultAction", (controller as FullPocoController).CustomControllerContext.ActionDescriptor.Name);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithoutValidationShouldNotValidateTheRequestModelForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithoutValidation()
                .Calling(c => c.ModelStateCheck(TestObjectFactory.GetRequestModelWithErrors()))
                .ShouldHave()
                .ValidModelState();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldHaveValidModelStateWhenThereAreNoModelErrorsForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var requestModel = TestObjectFactory.GetValidRequestModel();

            MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.OkResultActionWithRequestBody(1, requestModel))
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    var modelState = (controller as FullPocoController).CustomControllerContext.ModelState;

                    Assert.True(modelState.IsValid);
                    Assert.Equal(0, modelState.Values.Count());
                    Assert.Equal(0, modelState.Keys.Count());
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void NormalIndexOutOfRangeExceptionShouldShowNormalMessageForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            Assert.Throws<ActionCallAssertionException>(
                () =>
                {
                    MyMvc
                        .Controller<FullPocoController>()
                        .Calling(c => c.IndexOutOfRangeException())
                        .ShouldReturn()
                        .Ok();
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateCorrectActionNameAndActionResultWithAsyncActionCallForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var actionResultTestBuilder = MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.AsyncOkResultAction());

            this.CheckActionResultTestBuilder(actionResultTestBuilder, "AsyncOkResultAction");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateCorrectActionNameAndActionResultWithNormalActionCallForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var actionResultTestBuilder = MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.OkResultAction());

            this.CheckActionResultTestBuilder(actionResultTestBuilder, "OkResultAction");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateCorrectActionNameWithNormalVoidActionCallForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var voidActionResultTestBuilder = MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.EmptyAction());

            this.CheckActionName(voidActionResultTestBuilder, "EmptyAction");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateCorrectActionNameWithTaskActionCallForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var voidActionResultTestBuilder = MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.EmptyActionAsync());

            this.CheckActionName(voidActionResultTestBuilder, "EmptyActionAsync");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CallingShouldPopulateModelStateWhenThereAreModelErrorsForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var requestModel = TestObjectFactory.GetRequestModelWithErrors();

            MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.OkResultActionWithRequestBody(1, requestModel))
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    var modelState = (controller as FullPocoController).CustomControllerContext.ModelState;

                    Assert.False(modelState.IsValid);
                    Assert.Equal(2, modelState.Values.Count());
                    Assert.Equal("Integer", modelState.Keys.First());
                    Assert.Equal("RequiredString", modelState.Keys.Last());

                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithAuthenticatedUserShouldPopulateUserPropertyWithDefaultValuesForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithAuthenticatedUser()
                .Calling(c => c.AuthorizedAction())
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    var controllerUser = (controller as FullPocoController).CustomHttpContext.User;

                    Assert.Equal(false, controllerUser.IsInRole("Any"));
                    Assert.Equal("TestUser", controllerUser.Identity.Name);
                    Assert.True(controllerUser.HasClaim(ClaimTypes.Name, "TestUser"));
                    Assert.Equal("Passport", controllerUser.Identity.AuthenticationType);
                    Assert.Equal(true, controllerUser.Identity.IsAuthenticated);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithAuthenticatedUserShouldPopulateProperUserWhenUserWithUserBuilderForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithAuthenticatedUser(user => user
                    .WithUsername("NewUserName")
                    .WithAuthenticationType("Custom")
                    .InRole("NormalUser")
                    .AndAlso()
                    .InRoles("Moderator", "Administrator")
                    .InRoles(new[]
                    {
                        "SuperUser",
                        "MegaUser"
                    }))
                .Calling(c => c.AuthorizedAction())
                .ShouldReturn()
                .Ok()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    var controllerUser = (controller as FullPocoController).CustomHttpContext.User;

                    Assert.Equal("NewUserName", controllerUser.Identity.Name);
                    Assert.Equal("Custom", controllerUser.Identity.AuthenticationType);
                    Assert.Equal(true, controllerUser.Identity.IsAuthenticated);
                    Assert.Equal(true, controllerUser.IsInRole("NormalUser"));
                    Assert.Equal(true, controllerUser.IsInRole("Moderator"));
                    Assert.Equal(true, controllerUser.IsInRole("Administrator"));
                    Assert.Equal(true, controllerUser.IsInRole("SuperUser"));
                    Assert.Equal(true, controllerUser.IsInRole("MegaUser"));
                    Assert.Equal(false, controllerUser.IsInRole("AnotherRole"));
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithAuthenticatedNotCalledShouldNotHaveAuthorizedUserForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.AuthorizedAction())
                .ShouldReturn()
                .NotFound()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    var controllerUser = (controller as FullPocoController).CustomHttpContext.User;

                    Assert.Equal(false, controllerUser.IsInRole("Any"));
                    Assert.Equal(null, controllerUser.Identity.Name);
                    Assert.Equal(null, controllerUser.Identity.AuthenticationType);
                    Assert.Equal(false, controllerUser.Identity.IsAuthenticated);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithResolvedDependencyForShouldChooseCorrectConstructorWithLessDependenciesForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor<IInjectedService>(new InjectedService())
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.InjectedService);
                    Assert.Null(controller.AnotherInjectedService);
                    Assert.Null(controller.InjectedRequestModel);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithResolvedDependencyForShouldChooseCorrectConstructorWithMoreDependenciesForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor<IAnotherInjectedService>(new AnotherInjectedService())
                .WithServiceFor<IInjectedService>(new InjectedService())
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.InjectedService);
                    Assert.NotNull(controller.AnotherInjectedService);
                    Assert.Null(controller.InjectedRequestModel);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithResolvedDependencyForShouldChooseCorrectConstructorWithAllDependenciesForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor<IAnotherInjectedService>(new AnotherInjectedService())
                .WithServiceFor<RequestModel>(new RequestModel())
                .WithServiceFor<IInjectedService>(new InjectedService())
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.InjectedService);
                    Assert.NotNull(controller.AnotherInjectedService);
                    Assert.NotNull(controller.InjectedRequestModel);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithResolvedDependencyForShouldContinueTheNormalExecutionFlowOfTestBuildersForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor(new RequestModel())
                .WithServiceFor(new AnotherInjectedService())
                .WithServiceFor(new InjectedService())
                .WithAuthenticatedUser()
                .Calling(c => c.OkResultAction())
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void AndAlsoShouldContinueTheNormalExecutionFlowOfTestBuildersForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServiceFor(new RequestModel())
                .AndAlso()
                .WithServiceFor(new AnotherInjectedService())
                .AndAlso()
                .WithServiceFor(new InjectedService())
                .AndAlso()
                .WithAuthenticatedUser()
                .Calling(c => c.OkResultAction())
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void WithResolvedDependenciesShouldWorkCorrectWithCollectionOfObjectsForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServices(new List<object> { new RequestModel(), new AnotherInjectedService(), new InjectedService() })
                .WithAuthenticatedUser()
                .Calling(c => c.OkResultAction())
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void WithResolvedDependenciesShouldWorkCorrectWithParamsOfObjectsForPocoController()
        {
            MyMvc
                .Controller<FullPocoController>()
                .WithServices(new RequestModel(), new AnotherInjectedService(), new InjectedService())
                .WithAuthenticatedUser()
                .Calling(c => c.OkResultAction())
                .ShouldReturn()
                .Ok();
        }

        [Fact]
        public void WithResolvedDependencyForShouldThrowExceptionWhenSameDependenciesAreRegisteredForPocoController()
        {
            Test.AssertException<InvalidOperationException>(
                () =>
                {
                    MyMvc
                        .Controller<FullPocoController>()
                        .WithServiceFor<RequestModel>(new RequestModel())
                        .WithServiceFor<IAnotherInjectedService>(new AnotherInjectedService())
                        .WithServiceFor<IInjectedService>(new InjectedService())
                        .WithServiceFor<IAnotherInjectedService>(new AnotherInjectedService());
                },
                "Dependency AnotherInjectedService is already registered for FullPocoController controller.");
        }
        
        [Fact]
        public void PrepareControllerShouldSetCorrectPropertiesWithCustomSetupForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithSetup(c =>
                {
                    c.PublicProperty = new object();
                })
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.PublicProperty);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithHttpContextSessionShouldThrowExceptionIfSessionIsNotRegisteredForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var httpContext = new DefaultHttpContext();

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpContext(httpContext)
                .ShouldPassFor()
                .TheHttpContext(context =>
                {
                    Assert.Throws<InvalidOperationException>(() => context.Session);
                });
        }

        [Fact]
        public void WithHttpContextShouldPopulateCustomHttpContextForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "Custom";
            httpContext.Response.StatusCode = 404;
            httpContext.Response.ContentType = ContentType.ApplicationJson;
            httpContext.Response.ContentLength = 100;

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpContext(httpContext)
                .ShouldPassFor()
                .TheHttpContext(setHttpContext =>
                {
                    Assert.Equal("Custom", setHttpContext.Request.Scheme);
                    Assert.IsAssignableFrom<MockedHttpResponse>(setHttpContext.Response);
                    Assert.Same(httpContext.Response.Body, setHttpContext.Response.Body);
                    Assert.Equal(httpContext.Response.ContentLength, setHttpContext.Response.ContentLength);
                    Assert.Equal(httpContext.Response.ContentType, setHttpContext.Response.ContentType);
                    Assert.Equal(httpContext.Response.StatusCode, setHttpContext.Response.StatusCode);
                    Assert.Same(httpContext.Items, setHttpContext.Items);
                    Assert.Same(httpContext.Features, setHttpContext.Features);
                    Assert.Same(httpContext.RequestServices, setHttpContext.RequestServices);
                    Assert.Same(httpContext.TraceIdentifier, setHttpContext.TraceIdentifier);
                    Assert.Same(httpContext.User, setHttpContext.User);
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpContext(httpContext)
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.Equal("Custom", controller.CustomHttpContext.Request.Scheme);
                    Assert.IsAssignableFrom<MockedHttpResponse>(controller.CustomHttpContext.Response);
                    Assert.Same(httpContext.Response.Body, controller.CustomHttpContext.Response.Body);
                    Assert.Equal(httpContext.Response.ContentLength, controller.CustomHttpContext.Response.ContentLength);
                    Assert.Equal(httpContext.Response.ContentType, controller.CustomHttpContext.Response.ContentType);
                    Assert.Equal(httpContext.Response.StatusCode, controller.CustomHttpContext.Response.StatusCode);
                    Assert.Same(httpContext.Items, controller.CustomHttpContext.Items);
                    Assert.Same(httpContext.Features, controller.CustomHttpContext.Features);
                    Assert.Same(httpContext.RequestServices, controller.CustomHttpContext.RequestServices);
                    Assert.Same(httpContext.TraceIdentifier, controller.CustomHttpContext.TraceIdentifier);
                    Assert.Same(httpContext.User, controller.CustomHttpContext.User);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithHttpContextSetupShouldPopulateContextPropertiesForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpContext(httpContext =>
                {
                    httpContext.Request.ContentType = ContentType.ApplicationOctetStream;
                })
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.Equal(ContentType.ApplicationOctetStream, controller.CustomHttpContext.Request.ContentType);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithRequestShouldNotWorkWithDefaultRequestActionForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .Calling(c => c.WithRequest())
                .ShouldReturn()
                .BadRequest();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithRequestShouldWorkWithSetRequestActionForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpRequest(req => req.WithFormField("Test", "TestValue"))
                .Calling(c => c.WithRequest())
                .ShouldReturn()
                .Ok();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithRequestAsObjectShouldWorkWithSetRequestActionForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var httpContext = new MockedHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues> { ["Test"] = "TestValue" });

            MyMvc
                .Controller<FullPocoController>()
                .WithHttpRequest(httpContext.Request)
                .Calling(c => c.WithRequest())
                .ShouldReturn()
                .Ok();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void UsingUrlHelperInsideControllerShouldWorkCorrectlyForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithRouteData()
                .Calling(c => c.UrlAction())
                .ShouldReturn()
                .Ok()
                .WithResponseModel("/FullPoco/UrlAction");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void UnresolvedRouteValuesShouldHaveFriendlyExceptionForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            Test.AssertException<InvalidOperationException>(
                () =>
                {
                    MyMvc
                        .Controller<FullPocoController>()
                        .Calling(c => c.UrlAction())
                        .ShouldReturn()
                        .Ok()
                        .WithResponseModel("");
                },
                "Route values are not present in the action call but are needed for successful pass of this test case. Consider calling 'WithResolvedRouteValues' on the controller builder to resolve them from the provided lambda expression or set the HTTP request path by using 'WithHttpRequest'.");

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void PrepareControllerShouldSetCorrectPropertiesWithDefaultServicesForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.CustomHttpContext);
                    Assert.NotNull(controller.CustomActionContext);
                    Assert.NotNull(controller.CustomControllerContext);
                    Assert.NotNull(controller.CustomControllerContext.HttpContext);
                    Assert.NotNull(controller.CustomControllerContext.InputFormatters);
                    Assert.NotEmpty(controller.CustomControllerContext.InputFormatters);
                    Assert.NotNull(controller.CustomControllerContext.ValidatorProviders);
                    Assert.NotEmpty(controller.CustomControllerContext.ValidatorProviders);
                    Assert.NotNull(controller.CustomControllerContext.ValueProviders);
                    Assert.NotNull(controller.CustomHttpContext.Request);
                    Assert.NotNull(controller.CustomHttpContext.Response);
                    Assert.NotNull(controller.CustomControllerContext.ModelState);
                    Assert.NotNull(controller.CustomHttpContext.User);
                    Assert.Null(controller.CustomControllerContext.ActionDescriptor);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void PrepareControllerShouldSetCorrectPropertiesWithDefaultServices()
        {
            MyMvc
                .Controller<MvcController>()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.HttpContext);
                    Assert.NotNull(controller.HttpContext.RequestServices);
                    Assert.NotNull(controller.ControllerContext);
                    Assert.NotNull(controller.ControllerContext.HttpContext);
                    Assert.NotNull(controller.ControllerContext.InputFormatters);
                    Assert.NotEmpty(controller.ControllerContext.InputFormatters);
                    Assert.NotNull(controller.ControllerContext.ValidatorProviders);
                    Assert.NotEmpty(controller.ControllerContext.ValidatorProviders);
                    Assert.NotNull(controller.ControllerContext.ValueProviders);
                    Assert.NotNull(controller.ViewBag);
                    Assert.NotNull(controller.ViewData);
                    Assert.NotNull(controller.TempData);
                    Assert.NotNull(controller.Request);
                    Assert.NotNull(controller.Response);
                    Assert.NotNull(controller.MetadataProvider);
                    Assert.NotNull(controller.ModelState);
                    Assert.NotNull(controller.ObjectValidator);
                    Assert.NotNull(controller.Url);
                    Assert.NotNull(controller.User);
                    Assert.Null(controller.ControllerContext.ActionDescriptor);
                });
        }
        
        [Fact]
        public void WithControllerContextShouldSetControllerContextForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            var controllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    Name = "Test"
                }
            };

            MyMvc
                .Controller<FullPocoController>()
                .WithControllerContext(controllerContext)
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.CustomControllerContext);
                    Assert.Equal("Test", controller.CustomControllerContext.ActionDescriptor.Name);
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void WithControllerContextSetupShouldSetCorrectControllerContextForPocoController()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services =>
                {
                    services.AddHttpContextAccessor();
                });

            MyMvc
                .Controller<FullPocoController>()
                .WithControllerContext(controllerContext =>
                {
                    controllerContext.RouteData.Values.Add("testkey", "testvalue");
                })
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.NotNull(controller.CustomControllerContext);
                    Assert.True(controller.CustomControllerContext.RouteData.Values.ContainsKey("testkey"));
                });

            MyMvc.IsUsingDefaultConfiguration();
        }

        private void CheckActionResultTestBuilder<TActionResult>(
            IActionResultTestBuilder<TActionResult> actionResultTestBuilder,
            string expectedActionName)
        {
            this.CheckActionName(actionResultTestBuilder, expectedActionName);

            actionResultTestBuilder
                .ShouldPassFor()
                .TheActionResult(actionResult =>
                {
                    Assert.NotNull(actionResult);
                    Assert.IsAssignableFrom<OkResult>(actionResult);
                });
        }

        private void CheckActionName(IBaseTestBuilderWithInvokedAction testBuilder, string expectedActionName)
        {
            testBuilder
                .ShouldPassFor()
                .TheAction(actionName =>
                {
                    Assert.NotNull(actionName);
                    Assert.NotEmpty(actionName);
                    Assert.Equal(expectedActionName, actionName);
                });
        }
    }
}
