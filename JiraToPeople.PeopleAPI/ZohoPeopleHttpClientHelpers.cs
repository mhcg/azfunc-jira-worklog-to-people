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

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JiraToPeople.PeopleAPI.Extensions;
using JiraToPeople.PeopleAPI.Model;
using static Microsoft.AspNetCore.WebUtilities.QueryHelpers;

namespace JiraToPeople.PeopleAPI
{
    public partial class ZohoPeopleHttpClient
    {
        /// <summary>
        /// Validate the specified <paramref name="timeLog"/>.
        /// </summary>
        /// <param name="timeLog">PeopleTimeLog object to validate.</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        private static void ValidateTimeLog(PeopleTimeLog? timeLog)
        {
            if (timeLog == null)
                throw new System.ArgumentNullException();

            if (string.IsNullOrWhiteSpace(timeLog.UserEmail))
                throw new System.ArgumentException("UserEmail is empty.");
            if (string.IsNullOrWhiteSpace(timeLog.JobID))
                throw new System.ArgumentException("JobID is empty.");
            if (timeLog.StartDateTime == null)
                throw new System.ArgumentException("StartDateTime is null.");
        }

        /// <summary>
        /// Looks for 'timeLogId' in the Zoho People API response.
        /// </summary>
        /// <param name="responseBody">Json to parse.</param>
        /// <returns>The timeLogId or empty string if can't be found.</returns>
        private static string ParseTimeLogIDFromJson(string responseBody)
        {
            string timeLogID = "";
            var parse = JsonDocument.Parse(responseBody);
            var results = parse.RootElement
                .GetProperty("response")
                .GetProperty("result")
                .EnumerateArray();

            if (results.Count() > 0)
            {
                var field = results.First();
                timeLogID = field.GetProperty("timeLogId").GetString() ?? string.Empty;
            }

            return timeLogID;
        }

        /// <summary>
        /// Returns Uri object for the specified API method.
        /// </summary>
        /// <param name="url">API URL.</param>
        /// <param name="queryString">Dictionary with query string parameters.</param>
        /// <returns>New Uri object.</returns>
        private static System.Uri UriForMethod(string url, IDictionary<string, string> queryString)
            => new System.UriBuilder(AddQueryString(url, queryString)).Uri;

        /// <summary>
        /// Dictionary with relevant query string parameters from the specified <paramref name="timeLog"/> object.
        /// </summary>
        /// <param name="timeLog">PeopleTimeLog object to use.</param>
        /// <returns>New Dictionary object with all the required parameters set
        /// including the AuthToken.</returns>
        private IDictionary<string, string> QueryStringDictionary(PeopleTimeLog timeLog)
        {
            // Required stuff.
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "authtoken", AuthToken },
                { "user", timeLog.UserEmail },
                { "jobId", timeLog.JobID },
                { "workDate", timeLog.WorkDate.ToString("yyyy-MM-dd") },
                { "dateFormat", @"yyyy-MM-dd" },
                { "billingStatus", timeLog.BillingStatus.ConvertToString() },
                { "fromTime", timeLog.StartDateTime.ToString("hh:mmtt") },
                { "toTime", timeLog.EndDateTime.ToString("hh:mmtt") },
            };

            // Optional stuff.
            dictionary.Add("projectId", timeLog.ProjectID ?? string.Empty);
            dictionary.Add("projectName", timeLog.ProjectName ?? string.Empty);
            dictionary.Add("jobName", timeLog.JobName ?? string.Empty);
            dictionary.Add("workItem", timeLog.WorkItem ?? string.Empty);
            dictionary.Add("description", timeLog.Description ?? string.Empty);

            return dictionary;
        }
    }
}
