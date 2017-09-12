using System;
using System.Collections.Generic;
using Gurock.TestRail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestRails_Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> plans = new List<string>();
            List<string> suites = new List<string>();
            List<List<string>> TestCases = new List<List<string>>();
            List<string> Projects = new List<string>();
            List<List<string>> steps = new List<List<string>>();
            
            string description;
            string User = "mohd.ziya@landisgyr.com";
            string Password = "Qwerty#3";
			//this is for testing
            try
            {
                Projects = TestRails.getProjectName(User, Password);
            }
            catch(Exception ex) { Projects.Add(ex.Message); }
            plans = TestRails.getAllPlans(Projects[0]);
            suites = TestRails.getAllSuites(plans[1]);

            TestCases = TestRails.getAllTestCases(Projects[0], suites[0]);
            steps = TestRails.getAllTestSteps(TestCases[0][0], out description);
            TestRails.run(Projects[0], results());
        }
        public static List<List<object>> results()
        {
            List<List<object>> result = new List<List<object>>();
            //static list to check the run.
            List<object> r = new List<object>();
            r.Add(1000729);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(1000730);
            r.Add("failed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(1000731);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(1000765);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(1000766);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(970058);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(970059);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(970060);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(970061);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(996656);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r.Add(996665);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            r = new List<object>();
            r = new List<object>();
            r.Add(1000765);
            r.Add("passed");
            r.Add(100);
            r.Add(DateTime.Now.ToString());
            result.Add(r);
            //List Ends here.
            return result;
        }
    }
}
