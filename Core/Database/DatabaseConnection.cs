using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Sahara.Core.Logging;

namespace Sahara.Core.Database
{
    internal class DatabaseConnection : IDisposable
    {
        private readonly MySqlConnection _mysqlConnection;
        private MySqlCommand _mysqlCommand;
        private MySqlTransaction _mysqlTransaction;
        private List<MySqlParameter> _mysqlParameters;
        private readonly LogManager _logManager;

        public DatabaseConnection(string connectionStr)
        {
            _mysqlConnection = new MySqlConnection(connectionStr);
            _mysqlCommand = _mysqlConnection.CreateCommand();
            _logManager = Sahara.GetServer().GetLogManager();
        }

        public void OpenConnection()
        {
            if (_mysqlConnection.State == ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection already open.");
            }

            _mysqlConnection.Open();
        }

        public void CloseConnection()
        {
            if (_mysqlConnection.State == ConnectionState.Closed)
            {
                throw new InvalidOperationException("Connection already closed.");
            }

            _mysqlConnection.Close();
        }

        public void AddParameter(string param, object value)
        {
            if (_mysqlParameters == null)
            {
                _mysqlParameters = new List<MySqlParameter>();
            }

            _mysqlCommand.Parameters.AddWithValue(param, value);
            _mysqlParameters.Add(new MySqlParameter(param, value));
        }

        public void SetQuery(string query, bool log = true)
        {
            if (log)
            _logManager.Log("Running Query1: " + query, LogType.Information);
            _mysqlCommand.CommandText = query;
        }

        public void RunQuery(string query)
        {
            _logManager.Log("Running Query2: " + query, LogType.Information);
            SetQuery(query, false);
            RunQuery();
        }

        public void RunQuery()
        {
            try
            {
                _mysqlCommand.ExecuteNonQuery();
                _mysqlParameters.Clear();
            }
            catch (Exception exception)
            {
                _logManager.Log("MySQL Error: " + exception.Message.ToString() + "\r" + exception.StackTrace, LogType.Error);
                throw;
            }
        }

        public DataTable GetTable()
        {
            var dataTable = new DataTable();

            try
            {
                using (var adapter = new MySqlDataAdapter(_mysqlCommand))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception exception)
            {
                _logManager.Log("MySQL Error: " + exception.ToString() + "\r" + exception.StackTrace, LogType.Error);
            }

            return dataTable;
        }

        public int GetInteger()
        {
            var result = 0;

            try
            {
                var obj2 = _mysqlCommand.ExecuteScalar();

                if (obj2 != null)
                {
                    int.TryParse(obj2.ToString(), out result);
                }
            }
            catch (Exception exception)
            {
                _logManager.Log("MySQL Error: " + exception.Message + "\r" + exception.StackTrace, LogType.Error);
            }

            return result;
        }

        public string GetString()
        {
            try
            {
                var obj2 = _mysqlCommand.ExecuteScalar();
                return obj2?.ToString() ?? string.Empty;
            }
            catch (Exception exception)
            {
                _logManager.Log("MySQL Error: " + exception.Message + "\r" + exception.StackTrace, LogType.Error);
            }

            return string.Empty;
        }

        public DataRow GetRow()
        {
            DataRow row = null;
            try
            {
                var dataSet = new DataSet();

                using (var adapter = new MySqlDataAdapter(_mysqlCommand))
                {
                    adapter.Fill(dataSet);
                }

                if ((dataSet.Tables.Count > 0) && (dataSet.Tables[0].Rows.Count == 1))
                {
                    row = dataSet.Tables[0].Rows[0];
                }
            }
            catch (Exception exception)
            {
                _logManager.Log("MySQL Error: " + exception.Message + "\r" + exception.StackTrace, LogType.Error);
            }

            return row;
        }

        public void Dispose()
        {
            if (_mysqlTransaction != null)
            {
                _mysqlTransaction.Dispose();
                _mysqlTransaction = null;
            }

            if (_mysqlCommand == null)
            {
                return;
            }

            _mysqlCommand.Dispose();
            _mysqlCommand = null;
        }
    }
}

