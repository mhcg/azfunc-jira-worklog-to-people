using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using JiraToPeople.PeopleAPI.Model;

namespace JiraToPeople.PeopleAPI
{
    public class ZohoPeopleHttpClient : HttpClient
    {
        private const string ZOHO_PEOPLE_API = "https://people.zoho.com/people/api";
        private readonly string AuthToken;

        public ZohoPeopleHttpClient()
        {
            throw new ArgumentNullException("Missing AuthToken.");
        }

        public ZohoPeopleHttpClient(string authToken) : base()
        {
            if (string.IsNullOrWhiteSpace(authToken))
                throw new ArgumentException("Invalid AuthToken - it's empty.");

            AuthToken = authToken;
        }

        #region API Methods supported.

        /// <summary>
        /// Supported API Methods.
        /// </summary>
        public enum ApiMethod
        {
            TimeLogs
        }

        /// <summary>
        /// Returns Uri object for the specified API method.
        /// </summary>
        /// <param name="apiMethod">API Method.</param>
        /// <param name="method">Function within the method such as 'addtimelog'.</param>
        /// <returns>New Uri object.</returns>
        private static Uri ApiUriPathForMethod(ApiMethod apiMethod, string method)
        {
            string methodPath = apiMethod switch
            {
                ApiMethod.TimeLogs => "timetracker",
                _ => throw new NotImplementedException($"Method '{apiMethod}' not implemented.")
            };
            return new Uri($"{ZOHO_PEOPLE_API}/{methodPath}/{method}");
        }

        #endregion

        #region Helper methods.

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

        /// <summary>
        /// Validate the specified <paramref name="timeLog"/>.
        /// </summary>
        /// <param name="timeLog">PeopleTimeLog object to validate.</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        private static void ValidateTimeLog(PeopleTimeLog? timeLog)
        {
            if (timeLog == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(timeLog.UserEmail))
                throw new ArgumentException("UserEmail is empty.");
            if (string.IsNullOrWhiteSpace(timeLog.JobID))
                throw new ArgumentException("JobID is empty.");
            if (timeLog.StartDateTime == null)
                throw new ArgumentException("StartDateTime is null.");
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

        #endregion

        /// <summary>
        /// Call the Add time log API for the specified <paramref name="timeLog"/>.
        /// </summary>
        /// <param name="timeLog">PeopleTimeLog object to add.</param>
        /// <returns>Newly created Zoho People Time Log entry (timeLogID); or empty string on failure.</returns>
        public async Task<string> AddTimeLogAsync(PeopleTimeLog timeLog)
        {
            ValidateTimeLog(timeLog);

            var uriBuilder = new UriBuilder(
                Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
                    ApiUriPathForMethod(ApiMethod.TimeLogs, "addtimelog").ToString(),
                    QueryStringDictionary(timeLog)
                ));

            string responseBody = await GetStringAsync(uriBuilder.Uri);
            string timeLogID = ParseTimeLogIDFromJson(responseBody);
            return timeLogID;
        }
    }
}
