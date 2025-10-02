using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Employee_Management_System
{
    public static class SessionTracker
    {
        private static string _currentSessionId = string.Empty;

        public static string CurrentSessionId
        {
            get { return _currentSessionId; }
        }

        // Generate unique session ID starting from LOG-12002 (since LOG-12001 already exists)
        private static string GenerateSessionId()
        {
            try
            {
                dataAccess da = new dataAccess();
                string query = @"
SELECT ISNULL(MAX(CAST(SUBSTRING(logid, 5, LEN(logid)-4) AS INT)), 12001) 
FROM LogsTable 
WHERE logid LIKE 'Log-%' AND ISNUMERIC(SUBSTRING(logid, 5, LEN(logid)-4)) = 1";

                int maxId = Convert.ToInt32(da.ExecuteScalarQuery(query));
                return $"Log-{maxId + 1}"; // Changed from "LOG-" to "Log-"
            }
            catch (Exception)
            {
                // Fallback: if query fails, use random number starting from 12002
                return $"Log-{12002 + new Random().Next(1, 999)}"; // Changed from "LOG-" to "Log-"
            }
        }

        // Record login time in exact format: '9:50 p.m. | 07 Dec, 2024'
        public static string RecordLogin(string userId)
        {
            try
            {
                dataAccess da = new dataAccess();
                _currentSessionId = GenerateSessionId();

                // Format: '9:50 p.m. | 07 Dec, 2024'
                string loginTime = DateTime.Now.ToString("h:mm tt | dd MMM, yyyy").Replace("AM", "a.m.").Replace("PM", "p.m.");

                string query = $@"
INSERT INTO LogsTable (logid, userID, loginDateTime, logoutDateTime, duration)
VALUES ('{_currentSessionId}', '{userId}', '{loginTime}', NULL, NULL)";

                int result = da.ExecuteUpdateQuery(query);

                if (result > 0)
                {
                    return _currentSessionId;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording login: " + ex.Message, "Session Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        // Record logout time and calculate duration in exact format
        public static bool RecordLogout(string sessionId, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    // If no session ID, find the latest active session for this user
                    sessionId = FindLatestActiveSession(userId);
                    if (string.IsNullOrEmpty(sessionId))
                        return false;
                }

                dataAccess da = new dataAccess();

                // Format: '10:00 p.m. | 07 Dec, 2024'
                string logoutTime = DateTime.Now.ToString("h:mm tt | dd MMM, yyyy").Replace("AM", "a.m.").Replace("PM", "p.m.");

                // Get login time to calculate duration
                string getLoginTimeQuery = $"SELECT loginDateTime FROM LogsTable WHERE logid = '{sessionId}'";
                string loginTimeStr = da.ExecuteScalarQuery(getLoginTimeQuery)?.ToString();

                // Calculate duration in format: '0hr 10m 0s'
                string duration = CalculateDuration(loginTimeStr, logoutTime);

                string query = $@"
UPDATE LogsTable 
SET logoutDateTime = '{logoutTime}', duration = '{duration}'
WHERE logid = '{sessionId}' AND userID = '{userId}'";

                int result = da.ExecuteUpdateQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording logout: " + ex.Message, "Session Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Find latest active session for a user
        private static string FindLatestActiveSession(string userId)
        {
            try
            {
                dataAccess da = new dataAccess();
                string query = $@"
SELECT TOP 1 logid 
FROM LogsTable 
WHERE userID = '{userId}' AND logoutDateTime IS NULL 
ORDER BY loginDateTime DESC";

                object result = da.ExecuteScalarQuery(query);
                return result?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // Calculate duration between login and logout in format: '0hr 10m 0s'
        private static string CalculateDuration(string loginTimeStr, string logoutTimeStr)
        {
            try
            {
                // Replace a.m./p.m. back to AM/PM for parsing, then convert back
                DateTime loginTime = DateTime.ParseExact(loginTimeStr.Replace("a.m.", "AM").Replace("p.m.", "PM"), "h:mm tt | dd MMM, yyyy",
                                                       System.Globalization.CultureInfo.InvariantCulture);
                DateTime logoutTime = DateTime.ParseExact(logoutTimeStr.Replace("a.m.", "AM").Replace("p.m.", "PM"), "h:mm tt | dd MMM, yyyy",
                                                        System.Globalization.CultureInfo.InvariantCulture);

                TimeSpan duration = logoutTime - loginTime;

                int hours = (int)duration.TotalHours;
                int minutes = duration.Minutes;
                int seconds = duration.Seconds;

                return $"{hours}hr {minutes}m {seconds}s";
            }
            catch (Exception)
            {
                return "0hr 0m 0s";
            }
        }
    }
}