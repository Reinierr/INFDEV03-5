using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Assignment_1_INFDEV03_5
{
  class DBConnection
  {
    private MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;

    public DBConnection()
    {
      Initialize();
    }
    private void Initialize()
    {
      server = "localhost";
      database = "infdev03-5";
      uid = "root";
      password = "admin";
      string connectionString;
      connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

      connection = new MySqlConnection(connectionString);

    }
    private bool OpenConnection()
    {
      try
      {
        connection.Open();
        return true;
      }
      catch (MySqlException ex)
      {
        switch (ex.Number)
        {
          case 0:
            System.Diagnostics.Debug.WriteLine("Cant connect to server.");
            break;
          case 1045:
            System.Diagnostics.Debug.WriteLine("Invalid user/pass");
            break;
        }
        return false;
      }
    }
    private bool CloseConnection()
    {
      try
      {
        connection.Close();
        return true;
      }
      catch (MySqlException ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.Message);
        return false;
      }

    }

    public void Execute(string query)
    { 
      if(this.OpenConnection() == true)
      {
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.ExecuteNonQuery();
      }
      this.CloseConnection();
    }

    public int ExecutePK(string query)
    {
      int pk = 0;
      if (this.OpenConnection() == true)
      {
        MySqlCommand cmd = new MySqlCommand(query, connection);
        pk = Convert.ToInt32(cmd.ExecuteScalar());
      }
      this.CloseConnection();
      return pk;
    }

    public List<List<string>> Select(string query)
    {
      List<List<string>> results = new List<List<string>>();
      //Open connection
      if (this.OpenConnection() == true)
      {
        //Create Command
        MySqlCommand cmd = new MySqlCommand(query, connection);
        //Create a data reader and Execute the command
        MySqlDataReader dataReader = cmd.ExecuteReader();

        while (dataReader.Read())
        {
          List<string> result = new List<string>();
          var rowCount = dataReader.FieldCount;
          for (int i = 0; i < rowCount; i++)
          {
            result.Add(Convert.ToString(dataReader[i]));
          }
          results.Add(result);
        }

        //close Data Reader
        dataReader.Close();

        //close Connection
        this.CloseConnection();
      }
      return results;
    }

  }

}
