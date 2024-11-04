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
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Release\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                FlaUI.Core.AutomationElements.Button plus1Kaffe = window.FindFirstDescendant(cf.ByName("Kaffe +1")).AsButton();
                FlaUI.Core.AutomationElements.TextBox resultat = window.FindFirstDescendant(cf.ByName("Resultat")).AsTextBox();
                plus1Kaffe.Click();
                plus1Kaffe.Click();
                plus1Kaffe.Click();
                Trace.Assert(resultat.Text == "Antal kaffe: 3");







                //FlaUI.Core.AutomationElements.Button but = window.FindFirstDescendant(cf.ByName("Click me!")).AsButton();
                //FlaUI.Core.AutomationElements.TextBox tb = window.FindFirstDescendant(cf.ByAutomationId("myTextBox")).AsTextBox();
                //but.Click();
                //Trace.Assert(tb.Text == "Knappen", "button modified text");
            }
        }
    }
}