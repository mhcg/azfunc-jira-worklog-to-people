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
using Xunit;
using JiraToPeople.PeopleAPI.Model;
using JiraToPeople.PeopleAPI.Extensions;

namespace JiraToPeople.Tests
{
    public class ModelTests
    {
        #region Some known stuff for testing.

        internal const string TestUserEmail = @"whatev@example.com";
        internal const string TestJobID = @"111";

        internal const int OneHourInSeconds = 3600;
        internal const int OneAndHalfHoursInSeconds = 5400;

        internal static TimeSpan OneHourInTimeSpan => new TimeSpan(1, 0, 0);
        internal static TimeSpan OneAndHalfHoursTimeSpan => new TimeSpan(1, 30, 0);


        #endregion

        #region Type tests.

        [Fact]
        public void TypeSeconds()
        {
            var timeLog = new PeopleTimeLog(TestUserEmail, TestJobID, DateTime.Now, OneHourInSeconds);
            Assert.IsType<PeopleTimeLog>(timeLog);
        }

        [Fact]
        public void TypeTimeSpan()
        {
            var timeLog = new PeopleTimeLog(TestUserEmail, TestJobID, DateTime.Now, OneHourInTimeSpan);
            Assert.IsType<PeopleTimeLog>(timeLog);
        }

        #endregion

        #region Date tests.

        [Fact]
        public void StartDateTest()
        {
            var timeNow = DateTime.Now;
            var timeLog = new PeopleTimeLog(TestUserEmail, TestJobID, timeNow, OneHourInSeconds);

            Assert.Equal<DateTime>(timeNow, timeLog.StartDateTime);
            Assert.Equal<DateTime>(timeNow.Date, timeLog.WorkDate);
        }

        [Fact]
        public void EndDateTestOneHour()
        {
            var startDateTime = new DateTime(2012, 1, 1, 10, 0, 0);
            var expected = new DateTime(2012, 1, 1, 11, 0, 0);

            var timeLog = new PeopleTimeLog(TestUserEmail, TestJobID, startDateTime, OneHourInSeconds);
            Assert.Equal<DateTime>(expected, timeLog.EndDateTime);
        }

        [Fact]
        public void EndDateTestOneAndHalfHours()
        {
            var startDateTime = new DateTime(2012, 1, 1, 10, 0, 0);
            var expected = new DateTime(2012, 1, 1, 11, 30, 0);

            var timeLog = new PeopleTimeLog(TestUserEmail, TestJobID, startDateTime, OneAndHalfHoursInSeconds);
            Assert.Equal<DateTime>(expected, timeLog.EndDateTime);
        }

        #endregion

        #region BillingStatusType tests.

        [Fact]
        public void BillingStatusBillable()
        {
            var timelog = new PeopleTimeLog(TestUserEmail, TestJobID, DateTime.Now, OneAndHalfHoursInSeconds)
            {
                BillingStatus = PeopleTimeLog.BillingStatusType.Billable
            };
            Assert.Equal("billable", timelog.BillingStatus.ConvertToString());
        }

        [Fact]
        public void BillingStatusNonBillable()
        {
            var timelog = new PeopleTimeLog(TestUserEmail, TestJobID, DateTime.Now, OneAndHalfHoursInSeconds)
            {
                BillingStatus = PeopleTimeLog.BillingStatusType.NonBillable
            };
            Assert.Equal("non-billable", timelog.BillingStatus.ConvertToString());
        }

        #endregion

        #region String convert tests.

        [Fact]
        public void ConvertStringsInvalid()
        {
            var invalid = PeopleTimeLog.CreateFromStrings(null, null, null, null);
            Assert.Null(invalid);
        }

        [Fact]
        public void ConvertStringsInvalidJobID()
        {
            var invalid = PeopleTimeLog.CreateFromStrings(
                userEmail: TestUserEmail,
                jobID: null,
                DateTime.Now.ToString(),
                OneHourInSeconds.ToString()
                );
            Assert.Null(invalid);
        }

        [Fact]
        public void ConvertStringsInvalidStartDateTime()
        {
            var invalid = PeopleTimeLog.CreateFromStrings(
                userEmail: TestUserEmail,
                TestJobID.ToString(),
                startDateTime: null,
                OneHourInSeconds.ToString()
                );
            Assert.Null(invalid);
        }

        [Fact]
        public void ConvertStringsInvalidDuration()
        {
            var invalid = PeopleTimeLog.CreateFromStrings(
                userEmail: TestUserEmail,
                TestJobID.ToString(),
                DateTime.Now.ToString(),
                durationInSeconds: null
                );
            Assert.Null(invalid);
        }

        [Fact]
        public void ConvertStringsInvalidEmail()
        {
            var invalid = PeopleTimeLog.CreateFromStrings(
                userEmail: null,
                TestJobID.ToString(),
                DateTime.Now.ToString(),
                OneHourInSeconds.ToString()
                );
            Assert.Null(invalid);
        }

        [Fact]
        public void ConvertStringsValid()
        {
            var valid = PeopleTimeLog.CreateFromStrings(
                TestUserEmail,
                TestJobID.ToString(),
                DateTime.Now.ToString(),
                OneHourInSeconds.ToString()
                );
            Assert.NotNull(valid);
            Assert.IsType<PeopleTimeLog>(valid);
        }

        #endregion
    }
}
