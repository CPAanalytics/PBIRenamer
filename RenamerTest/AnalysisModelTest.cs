using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using PBIRenamer;
using Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper;

namespace RenamerTest
{
    [TestClass]
    public class AnalysisModelTest
    {
        public AnalysisModel anal;
        public AnalysisModelTest()
        {
            this.anal = new AnalysisModel();
        }

        [TestMethod]
        public void TestConnect()
        {
            var tome = anal.Connect();

            foreach (var measure in tome.Model.AllMeasures)
            {
                measure.SetName()
            }
        }
        
    }
}
