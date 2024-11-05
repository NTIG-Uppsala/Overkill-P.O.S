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
                var resultat = window.FindFirstDescendant(cf.ByAutomationId("ResultatListBox"))?.AsListBox();
                var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

                if (testplus1Kaffe == null)
                {
                    Trace.WriteLine("Button 'plus1Kaffe' not found.");
                    Assert.Fail("Button 'plus1Kaffe' not found.");
                }

                if (resultat == null)
                {
                    Trace.WriteLine("ListBox 'Resultat' not found.");
                    Assert.Fail("ListBox 'Resultat' not found.");
                }

                if (totalPriceTextBlock == null)
                {
                    Trace.WriteLine("TextBlock 'TotalPriceTextBlock' not found.");
                    Assert.Fail("TextBlock 'TotalPriceTextBlock' not found.");
                }

                testplus1Kaffe.Click();
                testplus1Kaffe.Click();
                testplus1Kaffe.Click();
                Assert.AreEqual("3 Kaffe", resultat.Items[0].Text);

                var nollställButton = window.FindFirstDescendant(cf.ByAutomationId("NollställButton"))?.AsButton();
                if (nollställButton == null)
                {
                    Trace.WriteLine("Button 'Nollställ' not found.");
                    Assert.Fail("Button 'Nollställ' not found.");
                }

                nollställButton.Click();
                nollställButton.Click();
                Assert.AreEqual(0, resultat.Items.Length);
                Assert.AreEqual("Totalpris: 0 kr", totalPriceTextBlock.Text);
            }
        }
    }
}
