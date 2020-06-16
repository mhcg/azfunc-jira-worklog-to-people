using System;
using System.Net.Http;
using System.Threading.Tasks;
using JiraToPeople.PeopleAPI.Model;

namespace JiraToPeople.PeopleAPI
{
    public partial class ZohoPeopleHttpClient : HttpClient
    {
        #region Constructors.

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

        #endregion

        /// <summary>
        /// Call the Add time log API for the specified <paramref name="timeLog"/>.
        /// </summary>
        /// <param name="timeLog">PeopleTimeLog object to add.</param>
        /// <returns>Newly created Zoho People Time Log entry (timeLogID); or empty string on failure.</returns>
        public async Task<string> AddTimeLogAsync(PeopleTimeLog timeLog)
        {
            ValidateTimeLog(timeLog);

            var uri = UriForMethod(
                ApiMethodUrls.Timelogs.AddTimeLog,
                QueryStringDictionary(timeLog));

            string responseBody = await GetStringAsync(uri);
            string timeLogID = ParseTimeLogIDFromJson(responseBody);
            return timeLogID;
        }
    }
}
