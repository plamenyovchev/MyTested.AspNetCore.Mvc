﻿namespace ApplicationParts.Test
{
    using ApplicationParts.Controllers;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class RouteTest
    {
        [Fact]
        public void HomeIndexShouldMatchCorrectController()
        {
            MyMvc
                .Routes()
                .ShouldMap("/")
                .To<HomeController>(c => c.Index());
        }
    }
}
