using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper;

namespace PBIRenamer
{
    public class AnalysisModel
    {
        //Microsoft.AnalysisServices.Tabular.Server asSrv = new Microsoft.AnalysisServices.Tabular.Server();
        // asSrv.Connect(@"Provider = MSOLAP; Data Source = powerbi://api.powerbi.com/v1.0/myorg/Condor;User ID=jchevalier@apspayroll.com;Password=FenRed88%%;Persist Security Info=True;Impersonation Level=Impersonate");
        public TabularModelHandler Connect()
        {
            var power_bi_endpoint = "powerbi://api.powerbi.com/v1.0/myorg/Condor";
            var dataBase_name = "Condor - ARR and Retention - V1";
            var username = "jchevalier@apspayroll.com";
            var password = "FenRed88%%";


            var tom = new TabularEditor.TOMWrapper.TabularModelHandler(power_bi_endpoint, dataBase_name, username, password);
            
            return tom;

        }
        
        
    }
}
