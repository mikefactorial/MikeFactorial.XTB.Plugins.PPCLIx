using Microsoft.VisualStudio.TestTools.UnitTesting;
using MikeFactorial.XTB.PACUI;
using System;

namespace MikeFactorial.XTB.Plugins.PACUI.Tests
{
    [TestClass]
    public class PACUIPluginControlTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var control = new PACUIPluginControl();
        }
    }
}
