using System;
using System.Collections.Generic;
using Gurock.TestRail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestRails_Integration
{
    static class TestRails
    {
        private static string Email,LoggedUser;
        private static int user_id;
        private static APIClient client = new APIClient("https://testrail.landisgyr.net/");
        public static List<string> getProjectName(string username, string password)
        {
            client.User= Email = username;
            client.Password = password;
            JObject user = (JObject)client.SendGet("get_user_by_email&email=" + Email);
            int.TryParse(user["id"].ToString(),out user_id);
            LoggedUser = user["name"].ToString();
            List<string> pnames = new List<string>();
            JArray proj = (JArray)client.SendGet("get_projects");
            for (int i = 0; i < proj.Count; i++)
            {
                pnames.Add(proj[i]["name"].ToString() + ":" + proj[i]["id"].ToString());
            }
            return pnames;

        }
        public static List<string> getAllPlans(string ProjID)
        {
            List<string> plans = new List<string>();
            JArray Plans = (JArray)client.SendGet("get_plans/" + ProjID.Substring(ProjID.LastIndexOf(":") + 1));
            for (int i = 0; i < Plans.Count; i++)
            {
                plans.Add(Plans[i]["name"].ToString() + ":" + Plans[i]["id"].ToString());
            }
            return plans;
        }
        public static List<string> getAllSuites(string plansName)
        {
            List<string> suites = new List<string>();
            JObject plan = (JObject)client.SendGet("get_plan/" + plansName.Substring(plansName.LastIndexOf(":")+1));
            
            //Get all suite in a plan
            JArray entries = (JArray)plan["entries"];
            //end

            for (int i = 0; i < entries.Count; i++)
            {
                suites.Add(entries[i]["name"].ToString() + "(" + entries[i]["suite_id"].ToString()+"):"+plansName.Substring(0, plansName.IndexOf(":")));
            }
            return suites;
        }
        public static List<List<string>> getAllTestCases(string ProjID, string suites)
        {
            List<List<string>> TestCases = new List<List<string>>();
            
            //get all cases of a suite.
            JArray suite = (JArray)client.SendGet("get_cases/"+ ProjID.Substring(ProjID.LastIndexOf(":") + 1) + "&suite_id=" + suites.Split(new[] { "(",")"},StringSplitOptions.RemoveEmptyEntries)[1]);

            for (int i = 0; i < suite.Count; i++)
            {
                List<string> TestCase = new List<string>();
                //List of the element for the test case window
                TestCase.Add(suite[i]["id"].ToString());
                TestCase.Add(suite[i]["title"].ToString());
                TestCase.Add(suite[i]["priority_id"].ToString());
                TestCase.Add("IsAutomate not available");
                TestCase.Add("State NA");
                TestCase.Add(suite[i]["created_by"].ToString());
                TestCase.Add(suites.Substring(suites.LastIndexOf(":") + 1));
                TestCase.Add(suites.Substring(0,suites.LastIndexOf(":") - 1));
                TestCase.Add(suite[i]["custom_summary"].ToString());
                TestCase.Add(suite[i]["estimate_forecast"].ToString());
                TestCase.Add(suite[i]["estimate"].ToString());
                TestCase.Add(suite[i]["custom_mission"].ToString());
                TestCase.Add(suite[i]["custom_goals"].ToString());

                //List ends here.
                TestCases.Add(TestCase);
            }
            return TestCases;
        }
        public static List<List<string>> getAllTestSteps(string TestCaseID, out string description)
        {
            List<List<string>> tcase = new List<List<string>>();
            
            JObject tests = (JObject)client.SendGet("get_case/"+ TestCaseID.Substring(TestCaseID.LastIndexOf(":") + 1));
            
            JArray steps = (JArray)tests["custom_steps_separated"];
            description = tests["custom_summary"].ToString();
            for (int i = 0; i < steps.Count; i++)
            {
                List<string> step = new List<string>();
                step.Add(steps[i]["content"].ToString());
                step.Add(steps[i]["expected"].ToString());            
                tcase.Add(step);
            }
            return tcase;
        }

        public static void run(string projectID, List<List<object>> result)
        {
            Dictionary<string, string> suite_idVsTcase = new Dictionary<string, string>();
            List<string> suite_ids=new List<string>();


            foreach (List<object> lst in result)
            {
                try
                {
                    //Get testcased vs test suite(to create run). Run always need suite id
                    JObject tcase = (JObject)client.SendGet("get_case/" + lst[0]);
                    if (suite_idVsTcase.ContainsKey(tcase["suite_id"].ToString()))
                    {
                        suite_idVsTcase[tcase["suite_id"].ToString()] += "," + lst[0];
                    }
                    else
                    {
                        suite_idVsTcase.Add(tcase["suite_id"].ToString(), lst[0].ToString());
                    }
                }
                catch { continue; }
               
                
            }
            foreach (KeyValuePair<string, string> kvp in suite_idVsTcase)
            {
                ///*** Prepare data for run
                ///*** Run always include suite ID
                var data = new Dictionary<string, object>
                {
                    { "name", DateTime.Now.ToString("yyyyMMddHHmmss") },
                    { "description", "Run using ATM" },
                    { "assignedto_id", user_id },
                    { "suite_id", kvp.Key },
                    { "include_all", false },
                    { "case_ids", kvp.Value.ToString().Split(',')}
                };

                //Add run
                JObject apstr = (JObject)client.SendPost("add_run/" + projectID.Substring(projectID.LastIndexOf(":") + 1), data);
                
                //get last run for run ID.
                JArray runs = (JArray)client.SendGet("get_runs/" + projectID.Substring(projectID.LastIndexOf(":") + 1)+ "&created_by="+user_id+ "&suite_id="+kvp.Key+"&limit=1");
                
                //Prepare data for result to upload
                var res = new Dictionary<object, List<Dictionary<string, object>>>
                    { { "results",getRunresult(result, kvp.Value) } };
                
                //Add result.
                JArray adResult = (JArray)client.SendPost("add_results_for_cases/" + runs[0]["id"].ToString(), res);
                
            }

        }
        private static List<Dictionary<string, object>> getRunresult(List<List<object>> result, string kvpValue )
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            var caseNumber = kvpValue.ToString().Split(',');
            int CaseCounter = 0;
            foreach (List<object> lob in result)
            {
                try
                {
                    if (lob[0].ToString().Contains(caseNumber[CaseCounter]))
                    {
                        int id = 5;
                        if (lob[1].ToString().ToUpper() == "PASSED")
                        {
                            id = 1;
                        }
                        var resultData = new Dictionary<string, object>
                            {
                                { "case_id", caseNumber[CaseCounter] },
                                { "status_id", id },
                                { "elapsed", lob[2]+"s" },
                                { "comment", "Automated" },
                                { "assignedto_id", user_id }
                            };


                        results.Add(resultData);
                        CaseCounter++;
                    }

                }
                catch { continue; }

            }
            return results;
        }

    }
}
