using System;
using LedMusic2.Nodes.NodeOptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LedMusic2Test.Nodes.NodeOptions
{
    [TestClass]
    public class ColorOptionTest
    {
        [TestMethod]
        public void IsSetReactive()
        {

            var opt = new ColorOption("Test");
            opt.GetFullState();

            opt.HandleCommand("setValue", Convert.ToBase64String(new byte[] { 255, 0, 0 }));

            var res = opt.GetStateUpdates();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count > 0);

        }
    }
}
