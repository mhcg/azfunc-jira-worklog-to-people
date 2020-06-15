// Copyright (c) 2020 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#nullable enable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using JiraToPeople.PeopleAPI.Model;
using System.Linq;
using System;
using JiraToPeople.PeopleAPI;

namespace JiraToPeople.AzFunctions
{
    public static class JiraWorklogToPeople
    {
        internal static readonly ZohoPeopleHttpClient PeopleHttpClient
            = new ZohoPeopleHttpClient(Environment.GetEnvironmentVariable("ZOHO_PEOPLE_API_AUTHTOKEN")!);

        [FunctionName("JiraWorklogToPeople")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("JiraWorklogToPeople started.");

            // Take values passed in from Jira webhook and call the
            // Zoho People Time Log API to add new record.
            string worklogStarted = req.Query["worklogStarted"].FirstOrDefault();
            string worklogTimeSpentSeconds = req.Query["worklogTimeSpentSeconds"].FirstOrDefault();
            string worklogUserEmail = req.Query["worklogUserEmail"].FirstOrDefault();
            string worklogJobID = req.Query["worklogJobID"].FirstOrDefault();

            string worklogComment = req.Query["worklogComment"].FirstOrDefault();
            string issueSummary = req.Query["issueSummary"].FirstOrDefault();
            string projectSummary = req.Query["projectSummary"].FirstOrDefault();

#if DEBUG
            log.LogDebug("worklogStarted", worklogStarted);
            log.LogDebug("worklogTimeSpentSeconds", worklogTimeSpentSeconds);
            log.LogDebug("worklogUserEmail", worklogUserEmail);
            log.LogDebug("worklogJobID", worklogJobID);
            log.LogDebug("worklogComment", worklogComment);
            log.LogDebug("issueSummary", issueSummary);
            log.LogDebug("projectSummary", projectSummary);
#endif

            // Format the TimeLog entry as required.
            PeopleTimeLog? timelog = PeopleTimeLog.CreateFromStrings(
                userEmail: worklogUserEmail,
                jobID: worklogJobID,
                startDateTime: worklogStarted,
                durationInSeconds: worklogTimeSpentSeconds
                );

            var result = new { timeLogId = "" };
            if (timelog != null)
            {
                timelog.WorkItem = $"{projectSummary}: {issueSummary}";
                timelog.Description = !string.IsNullOrWhiteSpace(worklogComment) ? worklogComment : timelog.WorkItem;

                log.LogInformation("Timelog created from query string parameters.", timelog);
                string logID = "";
                try
                {
                    logID = await PeopleHttpClient.AddTimeLogAsync(timelog);
                    log.LogInformation("AddtimeLogAsync called without issues.", logID);
                    result = new { timeLogId = logID.ToString() };
                }
                catch (Exception ex)
                {
                    log.LogCritical("Unable to call AddTimeLogAsync.", ex);
                    throw ex;
                }
            }
            else
            {
                log.LogError("Unable to create timelog object from specified parameters.");
                throw new Exception();
            }

            log.LogInformation("JiraWorklogToPeople finished.");
            return new JsonResult(result);
        }
    }
}
