﻿namespace MyTested.Mvc.Test
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internal;
    using Internal.Application;
    using Internal.Caching;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Caching.Memory;
    using Setups.Common;
    using Setups.Controllers;
    using Setups.Startups;
    using Xunit;

    public class MyMvcTests
    {
        [Fact]
        public void MockedMemoryCacheShouldBeRegistedWithAddedCaching()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services => services.AddMemoryCache());

            Assert.IsAssignableFrom<MockedMemoryCache>(TestServiceProvider.GetService<IMemoryCache>());

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void MockedMemoryCacheShouldBeDifferentForEveryCallSynchronously()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services => services.AddMemoryCache());

            // second call should not have cache entries
            MyMvc
                .Controller<MvcController>()
                .WithMemoryCache(cache => cache.WithEntry("test", "value"))
                .Calling(c => c.MemoryCacheAction())
                .ShouldReturn()
                .Ok();

            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.MemoryCacheAction())
                .ShouldReturn()
                .BadRequest();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void MockedMemoryCacheShouldBeDifferentForEveryCallSynchronouslyWithCachedControllerBuilder()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services => services.AddMemoryCache());

            var controller = MyMvc.Controller<MvcController>();

            // second call should not have cache entries
            controller
                .WithMemoryCache(cache => cache.WithEntry("test", "value"))
                .Calling(c => c.MemoryCacheAction())
                .ShouldReturn()
                .Ok();

            controller
                .Calling(c => c.MemoryCacheAction())
                .ShouldReturn()
                .BadRequest();

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void DefaultConfigurationShouldSetMockedMemoryCache()
        {
            MyMvc
                .IsUsingDefaultConfiguration()
                .WithServices(services => services.AddMemoryCache());

            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();

            Assert.NotNull(memoryCache);
            Assert.IsAssignableFrom<MockedMemoryCache>(memoryCache);

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void CustomMemoryCacheShouldOverrideTheMockedOne()
        {
            MyMvc.StartsFrom<CachingDataStartup>();

            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();

            Assert.NotNull(memoryCache);
            Assert.IsAssignableFrom<CustomMemoryCache>(memoryCache);

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void ExplicitMockedMemoryCacheShouldOverrideIt()
        {
            MyMvc
                .StartsFrom<DataStartup>()
                .WithServices(services =>
                {
                    services.ReplaceMemoryCache();
                });

            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();

            Assert.NotNull(memoryCache);
            Assert.IsAssignableFrom<MockedMemoryCache>(memoryCache);

            MyMvc.IsUsingDefaultConfiguration();
        }

        [Fact]
        public void MockedMemoryCacheShouldBeDifferentForEveryCallAsynchronously()
        {
            Task
                .Run(async () =>
                {
                    MyMvc
                        .IsUsingDefaultConfiguration()
                        .WithServices(services => services.AddMemoryCache());

                    TestHelper.GlobalTestCleanup += () => TestServiceProvider.GetService<IMemoryCache>()?.Dispose();
                    TestHelper.ExecuteTestCleanup();

                    string firstValue = null;
                    string secondValue = null;
                    string thirdValue = null;
                    string fourthValue = null;
                    string fifthValue = null;

                    var tasks = new List<Task>
                    {
                        Task.Run(() =>
                        {
                            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();
                            memoryCache.Set("test", "first");
                            firstValue = TestServiceProvider.GetService<IMemoryCache>().Get<string>("test");
                            TestHelper.ExecuteTestCleanup();
                        }),
                        Task.Run(() =>
                        {
                            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();
                            memoryCache.Set("test", "second");
                            secondValue = TestServiceProvider.GetService<IMemoryCache>().Get<string>("test");
                            TestHelper.ExecuteTestCleanup();
                        }),
                        Task.Run(() =>
                        {
                            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();
                            memoryCache.Set("test", "third");
                            thirdValue = TestServiceProvider.GetService<IMemoryCache>().Get<string>("test");
                            TestHelper.ExecuteTestCleanup();
                        }),
                        Task.Run(() =>
                        {
                            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();
                            memoryCache.Set("test", "fourth");
                            fourthValue = TestServiceProvider.GetService<IMemoryCache>().Get<string>("test");
                            TestHelper.ExecuteTestCleanup();
                        }),
                        Task.Run(() =>
                        {
                            var memoryCache = TestServiceProvider.GetService<IMemoryCache>();
                            memoryCache.Set("test", "fifth");
                            fifthValue = TestServiceProvider.GetService<IMemoryCache>().Get<string>("test");
                            TestHelper.ExecuteTestCleanup();
                        })
                    };

                    await Task.WhenAll(tasks);

                    Assert.Equal("first", firstValue);
                    Assert.Equal("second", secondValue);
                    Assert.Equal("third", thirdValue);
                    Assert.Equal("fourth", fourthValue);
                    Assert.Equal("fifth", fifthValue);

                    MyMvc.IsUsingDefaultConfiguration();
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}
