using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;

namespace TestSystem
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                var testplus1Kaffe = window.FindFirstDescendant(cf.ByAutomationId("plus1Kaffe"))?.AsButton();
                var resultat = window.FindFirstDescendant(cf.ByAutomationId("ResultatListBox"))?.AsListBoxItem();

                if (testplus1Kaffe == null)
                {
                    Trace.WriteLine("Button 'plus1Kaffe' not found.");
                    Assert.Fail("Button 'plus1Kaffe' not found.");
                }

                if (resultat == null)
                {
                    Trace.WriteLine("TextBox 'Resultat' not found.");
                    Assert.Fail("TextBox 'Resultat' not found.");
                }

                testplus1Kaffe.Click();
                testplus1Kaffe.Click();
                testplus1Kaffe.Click();
                Trace.Assert(resultat.Text == "3 Kaffe");
            }
        }
    }
}
