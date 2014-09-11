// License: The MIT License (MIT) Copyright (c) 2010 Barend Gehrels

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

// SOCI-like ORM based on C# and Specialization by Traits

// Version 2, July 29, 2010

namespace Stormy
{
    // Originally based on SOCI soci::type_conversion<T> specialization

    public interface IConvertable<T>
    {
    }

    public interface ISelectable<T> : IConvertable<T>
    {
        T ApplySelect(SqlDataReader reader, Connection c, IEnumerable<T> list);
    }

    public interface IInsertable<T> : IConvertable<T>
    {
        // Might become "ref T obj" to generate ID, also for POCO struct
        void ApplyInsert(T obj, SqlCommand command);
        string InsertSql();
    }

    public interface IUpdateable<T> : IConvertable<T>
    {
        void ApplyUpdate(T obj, SqlCommand command);
        string UpdateSql();
    }

    public interface IDeleteable<T> : IConvertable<T>
    {
        void ApplyDelete(T obj, SqlCommand command);
        string DeleteSql();
    }


    public static class Orm
    {
        private static IDictionary<System.Type, object> m_select_register;
        private static IDictionary<System.Type, object> m_insert_register;
        private static IDictionary<System.Type, object> m_delete_register;
        private static IDictionary<System.Type, object> m_update_register;

        private static void Init()
        {
            if (m_select_register == null)
            {
                m_select_register = new Dictionary<System.Type, object>();
                m_insert_register = new Dictionary<System.Type, object>();
                m_delete_register = new Dictionary<System.Type, object>();
                m_update_register = new Dictionary<System.Type, object>();
            }
        }

        public static void RegisterSelect<T>(ISelectable<T> converter)
        {
            Init();
            m_select_register[typeof(T)] = converter;
        }

        public static void RegisterInsert<T>(IInsertable<T> converter)
        {
            Init();
            m_insert_register[typeof(T)] = converter;
        }

        public static void RegisterDelete<T>(IDeleteable<T> converter)
        {
            Init();
            m_delete_register[typeof(T)] = converter;
        }

        // Generic register function, in case the converter implements
        // all interfaces.
        public static void Register<T>(IConvertable<T> converter)
        {
            Init();
            if (converter is ISelectable<T>)
            {
                m_select_register[typeof(T)] = converter;
            }
            if (converter is IInsertable<T>)
            {
                m_insert_register[typeof(T)] = converter;
            }
            if (converter is IUpdateable<T>)
            {
                m_update_register[typeof(T)] = converter;
            }
            if (converter is IDeleteable<T>)
            {
                m_delete_register[typeof(T)] = converter;
            }
        }

        public static ISelectable<T> GetSelectable<T>()
        {
            return m_select_register[typeof(T)] as ISelectable<T>;
        }
        public static IInsertable<T> GetInsertable<T>()
        {
            return m_insert_register[typeof(T)] as IInsertable<T>;
        }
        public static IUpdateable<T> GetUpdateable<T>()
        {
            return m_update_register[typeof(T)] as IUpdateable<T>;
        }
        public static IDeleteable<T> GetDeleteable<T>()
        {
            return m_delete_register[typeof(T)] as IDeleteable<T>;
        }
    }

    public class Connection
    {
        private SqlConnection m_connection;

        public Connection(String connection_string)
        {
            m_connection = new SqlConnection(connection_string);
            m_connection.Open();
            // Closed automatically using its Dispose method
        }

        public IEnumerable<T> Select<T>(String statement, ISelectable<T> traits)
        {
            var list = new List<T>();

            using (SqlCommand cmd = new SqlCommand(statement, m_connection))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            T obj = traits.ApplySelect(rdr, this, list);
                            if (obj != null)
                            {
                                list.Add(obj);
                            }
                            //possibly: yield return traits.ApplySelect(rdr);
                        }
                    }
                    finally
                    {
                        rdr.Close();
                    }
                }
            }

            return list;
        }

        public IEnumerable<T> Select<T>(String statement)
        {
            return Select(statement, Orm.GetSelectable<T>());
        }

        // ---------------------------------------------------------------------
        // Insert
        // ---------------------------------------------------------------------
        public void Insert<T>(T model, String statement, IInsertable<T> traits)
        {
            using (SqlCommand cmd = new SqlCommand(statement, m_connection))
            {
                traits.ApplyInsert(model, cmd);
                cmd.ExecuteNonQuery();
            }
        }

        public void Insert<T>(T model, String statement)
        {
            Insert(model, statement, Orm.GetInsertable<T>());
        }

        public void Insert<T>(T model)
        {
            var traits = Orm.GetInsertable<T>();
            Insert(model, traits.InsertSql(), traits);
        }

        // ---------------------------------------------------------------------
        // Delete
        // ---------------------------------------------------------------------
        public void Delete<T>(T model, String statement, IDeleteable<T> traits)
        {
            using (SqlCommand cmd = new SqlCommand(statement, m_connection))
            {
                traits.ApplyDelete(model, cmd);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete<T>(T model, String statement)
        {
            Delete(model, statement, Orm.GetDeleteable<T>());
        }

        public void Delete<T>(T model)
        {
            var traits = Orm.GetDeleteable<T>();
            Delete(model, traits.DeleteSql(), traits);
        }

        // ---------------------------------------------------------------------
        // Any statment.
        // ---------------------------------------------------------------------
        public void Execute(String statement)
        {
            using (SqlCommand cmd = new SqlCommand(statement, m_connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    // Stubb class to create converters returning nothing.
    // Note that they might use "object" or even "int" as well
    // This underscore might be considered as a bit too subtile...
    public class _ { }
}
