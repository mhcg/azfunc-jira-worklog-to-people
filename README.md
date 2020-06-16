# Jira Worklog to Zoho People Time Log

![.NET Core](https://github.com/mhcg/azfunc-jira-worklog-to-people/workflows/.NET%20Core/badge.svg)

## Overview

Rough and ready Azure Functions function to accept Jira Work Log data via a webhook call and add to Zoho People via the Time Logs API.

Currently expects a hardcoded Zoho People Job ID that all Worklogs will be added as. General idea being the time logging from Jira at least gets into Zoho People, the user can then allocate to the right Zoho People Project and Job as required.

## Usage Instructions

Designed to be used with Jira Automation Rules, triggered on Worklog Create. Use the Azure Function URL with the query parameters along the following lines.

The Zoho People API AuthToken needs to be stored in the environment variable ZOHO_PEOPLE_API_AUTHTOKEN.

_Replace **JOBID** with your own._

_Don't forget the urlEncode if using different Smart Values._

> worklogStarted={{worklog.started.format("yyyy-MM-dd HH:mm:ss").urlEncode}}

> worklogTimeSpentSeconds={{worklog.timeSpentSeconds}}

> worklogUserEmail={{worklog.author.emailAddress.urlEncode}}

> worklogJobID=**JOBID**

> worklogComment={{worklog.comment.urlEncode}}

> issueSummary={{issue.summary.urlEncode}}

> projectSummary={{issue.project.name.urlEncode}}

The function will use the worklog comment if there is one, otherwise will use the calculated work item field. Work item is calculated from the project name and issue summary text.

Strongly suggested that a function (or other) key is used with the Azure Function, and that it's passed as 'x-functions-key' header rather than in the query string.

### Example URL

_Replace functionname with your own._

`
https://functionname.azurewebsites.net/api/JiraWorklogToPeople?worklogStarted={{worklog.started.format("yyyy-MM-dd HH:mm:ss").urlEncode}}&worklogTimeSpentSeconds={{worklog.timeSpentSeconds}}&worklogUserEmail={{worklog.author.emailAddress.urlEncode}}&worklogJobID=126871000002508001&worklogComment={{worklog.comment.urlEncode}}&issueSummary={{issue.summary.urlEncode}}&projectSummary={{issue.project.name.urlEncode}}
`

## Known Issues

* Currently, whatever start date/time gets sent from Jira is what Zoho People will use. Only tested with UTC at this time.
