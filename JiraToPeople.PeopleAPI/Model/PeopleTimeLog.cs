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

using System;

namespace JiraToPeople.PeopleAPI.Model
{
    public class PeopleTimeLog
    {
        #region Required properties.

        public string UserEmail { get; set; }
        public string JobID { get; set; }

        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }

        public BillingStatusType BillingStatus { get; set; }

        #endregion

        #region  Optional properties.

        public string? ProjectID { get; set; }
        public string? ProjectName { get; set; }

        public string? JobName { get; set; }

        public string? WorkItem { get; set; }
        public string? Description { get; set; }

        #endregion

        #region Calculated properties.

        public DateTime WorkDate => StartDateTime.Date;
        public DateTime EndDateTime => StartDateTime.Add(Duration);

        #endregion

        #region Types.

        public enum BillingStatusType
        {
            Billable,
            NonBillable
        }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userEmail">User email address.</param>
        /// <param name="jobID">JobID.</param>
        /// <param name="startDateTime">Start DateTime.</param>
        /// <param name="duration">Duration as TimeSpan.</param>
        public PeopleTimeLog(string userEmail, string jobID, DateTime startDateTime, TimeSpan duration)
        {
            // Sanity check.
            if (string.IsNullOrWhiteSpace(jobID))
                throw new ArgumentException("JobID invalid.");
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new ArgumentException("UserEmail invalid.");

            // Required properties.
            UserEmail = userEmail;
            JobID = jobID;
            StartDateTime = startDateTime;
            Duration = duration;
            BillingStatus = BillingStatusType.NonBillable;
        }

        /// <summary>
        /// Creates a new PeopleTimeLog object with the specified parameters.
        /// </summary>
        /// <param name="userEmail">User email address.</param>
        /// <param name="jobID">JobID.</param>
        /// <param name="startDateTime">Start DateTime.</param>
        /// <param name="durationInSeconds">Duration in seconds.</param>
        public PeopleTimeLog(string userEmail, string jobID, DateTime startDateTime, int durationInSeconds)
            : this(userEmail, jobID, startDateTime, new TimeSpan(0, 0, durationInSeconds)) { }

        /// <summary>
        /// Creates a new PeopleTimeLog object, if possible, from the specified parameters.
        /// </summary>
        /// <param name="userEmail">User email address.</param>
        /// <param name="jobID">JobID.</param>
        /// <param name="startDateTime">Start DateTime.</param>
        /// <param name="durationInSeconds">Duration in seconds.</param>
        /// <returns>New PeopleTimeLog object if the specified parameters are sane; null otherwise.</returns>
        public static PeopleTimeLog? CreateFromStrings(string? userEmail, string? jobID, string? startDateTime, string? durationInSeconds)
        {
            // Sanity check.
            if (string.IsNullOrWhiteSpace(userEmail))
                return null;
            if (string.IsNullOrWhiteSpace(jobID))
                return null;
            if (!DateTime.TryParse(startDateTime, out var dateTime))
                return null;
            if (!int.TryParse(durationInSeconds, out var duration))
                return null;

            // Create object and return.
            return new PeopleTimeLog(
                userEmail: userEmail!,
                jobID: jobID!,
                startDateTime: dateTime,
                durationInSeconds: duration
                );
        }
    }
}
