﻿namespace MusicStore.Test.Controllers
{
    using MusicStore.Controllers;
    using MyTested.Mvc;
    using Xunit;

    public class HomeControllerTest
    {
        [Fact]
        public void ErrorShouldReturnCorrectView()
        {
            MyMvc
                .Controller<HomeController>()
                .Calling(c => c.Error())
                .ShouldReturn()
                .View("~/Views/Shared/Error.cshtml");
        }
    }
}