using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data;
using System;

namespace TwionTech
{
    namespace DBConnector
    {
        public class DB : IDisposable
        {
            private object CLink = null;
            private object CCommand = null;
            private int CType;

            public DB(int type, string Host, string DataBase, string User, string Password)
            {
                CType = type;

                switch (CType)
                {
                    case 0:
                        CLink = new MySqlConnection("data source=" + Host + ";DATABASE=" + DataBase + ";UID=" + User + ";PWD=" + Password);
                        ((MySqlConnection)CLink).Open();
                        CCommand = new MySqlCommand("", (MySqlConnection)CLink);
                        break;
                    case 1:
                        CLink = new SqlConnection("data source=" + Host + ";DATABASE=" + DataBase + ";UID=" + User + ";PWD=" + Password);
                        ((SqlConnection)CLink).Open();
                        CCommand = new SqlCommand("", (SqlConnection)CLink);
                        break;
                    case 2:
                        CLink = new SQLiteConnection("data source=" + Host);
                        ((SQLiteConnection)CLink).Open();
                        CCommand = new SQLiteCommand("", (SQLiteConnection)CLink);
                        break;
                }
            }

            public int cType
            {
                get
                {
                    return CType;
                }
            }

            public System.Data.ConnectionState State
            {
                get
                {
                    switch (CType)
                    {
                        case 0:
                            return ((MySqlConnection)CLink).State;
                        case 1:
                            return ((SqlConnection)CLink).State;
                        case 2:
                            return ((SQLiteConnection)CLink).State;
                        default:
                            return System.Data.ConnectionState.Closed;
                    }
                }
            }

            public void ClearParams()
            {
                switch (CType)
                {
                    case 0:
                        ((MySqlCommand)CCommand).Parameters.Clear();
                        break;
                    case 1:
                        ((SqlCommand)CCommand).Parameters.Clear();
                        break;
                    case 2:
                        ((SQLiteCommand)CCommand).Parameters.Clear();
                        break;
                }
            }

            public void AddParam(string name, object param)
            {
                switch (CType)
                {
                    case 0:
                        ((MySqlCommand)CCommand).Parameters.AddWithValue(name, param);
                        break;
                    case 1:
                        ((SqlCommand)CCommand).Parameters.AddWithValue(name, param);
                        break;
                    case 2:
                        ((SQLiteCommand)CCommand).Parameters.AddWithValue(name, param);
                        break;
                }
            }

            public int RunQuery(string Query)
            {
                switch (CType)
                {
                    case 0:
                        ((MySqlCommand)CCommand).CommandText = Query;
                        return ((MySqlCommand)CCommand).ExecuteNonQuery();
                    case 1:
                        ((SqlCommand)CCommand).CommandText = Query;
                        return ((SqlCommand)CCommand).ExecuteNonQuery();
                    case 2:
                        ((SQLiteCommand)CCommand).CommandText = Query;
                        return ((SQLiteCommand)CCommand).ExecuteNonQuery();
                }
                return -1;
            }

            public DBReader GetReader()
            {
                switch (CType)
                {
                    case 0:
                        return new DBReader(0, ((MySqlCommand)CCommand).ExecuteReader());
                    case 1:
                        return new DBReader(1, ((SqlCommand)CCommand).ExecuteReader());
                    case 2:
                        return new DBReader(2, ((SQLiteCommand)CCommand).ExecuteReader());
                }
                return null;
            }

            public DBAdapter GetAdapter()
            {
                switch (CType)
                {
                    case 0:
                        return new DBAdapter(0, (MySqlConnection)CLink, ((MySqlCommand)CCommand).CommandText);
                    case 1:
                        return new DBAdapter(1, (SqlConnection)CLink, ((SqlCommand)CCommand).CommandText);
                    case 2:
                        return new DBAdapter(2, (SQLiteConnection)CLink, ((SQLiteCommand)CCommand).CommandText);
                }
                return null;
            }

            public void Dispose()
            {
                switch (CType)
                {
                    case 0:
                        ((MySqlConnection)CLink).Close();
                        ((MySqlConnection)CLink).Dispose();
                        break;
                    case 1:
                        ((SqlConnection)CLink).Close();
                        ((SqlConnection)CLink).Dispose();
                        break;
                    case 2:
                        ((SQLiteConnection)CLink).Close();
                        ((SQLiteConnection)CLink).Dispose();
                        break;
                }
                GC.SuppressFinalize(this);
            }
        }

        public class DBReader
        {
            private object ReaderObject = null;
            int ConType;

            public DBReader(int type, object reader)
            {
                ConType = type;
                ReaderObject = reader;
            }

            public bool Read()
            {
                switch (ConType)
                {
                    case 0:
                        return ((MySqlDataReader)ReaderObject).Read();
                    case 1:
                        return ((SqlDataReader)ReaderObject).Read();
                    case 2:
                        return ((SQLiteDataReader)ReaderObject).Read();
                }
                return false;
            }

            public object this[string column]
            {
                get
                {
                    switch (ConType)
                    {
                        case 0:
                            return ((MySqlDataReader)ReaderObject)[column];
                        case 1:
                            return ((SqlDataReader)ReaderObject)[column];
                        case 2:
                            return ((SQLiteDataReader)ReaderObject)[column];
                    }
                    return null;
                }
            }

            public object this[int column]
            {
                get
                {
                    switch (ConType)
                    {
                        case 0:
                            return ((MySqlDataReader)ReaderObject)[column];
                        case 1:
                            return ((SqlDataReader)ReaderObject)[column];
                        case 2:
                            return ((SQLiteDataReader)ReaderObject)[column];
                    }
                    return null;
                }
            }

            public int GetColumns(object[] array)
            {
                switch (ConType)
                {
                    case 0:
                        return ((MySqlDataReader)ReaderObject).GetValues(array);
                    case 1:
                        return ((SqlDataReader)ReaderObject).GetValues(array);
                    case 2:
                        return ((SQLiteDataReader)ReaderObject).GetValues(array);
                }
                return 0;
            }

            public void Dispose()
            {
                switch (ConType)
                {
                    case 0:
                        ((MySqlDataReader)ReaderObject).Close();
                        break;
                    case 1:
                        ((SqlDataReader)ReaderObject).Close();
                        break;
                    case 2:
                        ((SQLiteDataReader)ReaderObject).Close();
                        break;
                }
                GC.SuppressFinalize(this);
            }
        }

        public class DBAdapter
        {
            private object cLink = null;
            private object Adapter = null;
            private int? cType = null;
            
            public DBAdapter(int cType, object cLink,string Query)
            {
                this.cType = cType;
                this.cLink = cLink;

                switch (cType)
                {
                    case 0:
                        Adapter = new MySqlDataAdapter(Query, (MySqlConnection)cLink);
                        break;
                    case 1:
                        Adapter = new SqlDataAdapter(Query, (SqlConnection)cLink);
                        break;
                    case 2:
                        Adapter = new SQLiteDataAdapter(Query, (SQLiteConnection)cLink);
                        break;
                }
            }

            public int Fill(DataTable DT)
            {
                switch (cType)
                {
                    case 0:
                        return ((MySqlDataAdapter)Adapter).Fill(DT);
                    case 1:
                        return ((SqlDataAdapter)Adapter).Fill(DT);
                    case 2:
                        return ((SQLiteDataAdapter)Adapter).Fill(DT);
                }
                return 0;
            }

            public int Fill(DataSet DS)
            {
                switch (cType)
                {
                    case 0:
                        return ((MySqlDataAdapter)Adapter).Fill(DS);
                    case 1:
                        return ((SqlDataAdapter)Adapter).Fill(DS);
                    case 2:
                        return ((SQLiteDataAdapter)Adapter).Fill(DS);
                }
                return 0;
            }

            public int Update(DataTable DT)
            {
                switch (cType)
                {
                    case 0:
                        return ((MySqlDataAdapter)Adapter).Update(DT);
                    case 1:
                        return ((SqlDataAdapter)Adapter).Update(DT);
                    case 2:
                        return ((SQLiteDataAdapter)Adapter).Update(DT);
                }
                return 0;
            }

            public int Update(DataSet DS)
            {
                switch (cType)
                {
                    case 0:
                        return ((MySqlDataAdapter)Adapter).Update(DS);
                    case 1:
                        return ((SqlDataAdapter)Adapter).Update(DS);
                    case 2:
                        return ((SQLiteDataAdapter)Adapter).Update(DS);
                }
                return 0;
            }


        }
    }
}