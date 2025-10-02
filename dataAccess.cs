using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management_System
{
    internal class dataAccess
    {
        private SqlConnection sqlcon;

        public SqlConnection Sqlcon

        {

            get { return this.sqlcon; }

            set { this.sqlcon = value; }

        }

        private SqlCommand sqlcom;

        public SqlCommand Sqlcom

        {

            get { return this.sqlcom; }

            set { this.sqlcom = value; }

        }

        private SqlDataAdapter sda;

        public SqlDataAdapter Sda

        {

            get { return this.sda; }

            set { this.sda = value; }

        }

        private DataSet ds;

        public DataSet Ds

        {

            get { return this.ds; }

            set { this.ds = value; }

        }

        //internal DataTable dt;

        public dataAccess()

        {

            this.Sqlcon = new SqlConnection(@"Data Source=DESKTOP-BIU9IHC\SQLEXPRESS;Initial Catalog=JobConnect;Persist Security Info=True;User ID=sa;Password=shadhin");

            this.Sqlcon.Open();

        }

        private void QueryText(string query)

        {

            this.Sqlcom = new SqlCommand(query, this.Sqlcon);

        }

        public DataSet ExecuteQuery(string sql)

        {

            try

            {

                this.QueryText(sql);

                this.Sda = new SqlDataAdapter(this.Sqlcom);

                this.Ds = new DataSet();

                this.Sda.Fill(this.Ds);

                return this.Ds;

            }

            catch (Exception exc)

            {

                return null;

            }

        }

        public DataTable ExecuteQueryTable(string sql)

        {

            this.QueryText(sql);

            this.Sda = new SqlDataAdapter(this.Sqlcom);

            this.Ds = new DataSet();

            this.Sda.Fill(this.Ds);

            return this.Ds.Tables[0];

        }

        public DataSet ExecuteQuery(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            try
            {
                cmd.Connection = this.Sqlcon;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing query: " + ex.Message);
            }
            return ds;
        }


        public int ExecuteUpdateQuery(string sql)

        {

            this.QueryText(sql);

            int u = this.Sqlcom.ExecuteNonQuery();

            return u;

        }

        public object ExecuteScalarQuery(string sql)
        {
            try
            {
                //this.Sqlcon.Open();
                SqlCommand cmd = new SqlCommand(sql, this.Sqlcon);
                object result = cmd.ExecuteScalar();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing scalar query: " + ex.Message);
            }
            
        }

       
        public int ExecuteUpdateQueryWithParameters(SqlCommand cmd)
        {
            try
            {
                cmd.Connection = this.Sqlcon;
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing update query: " + ex.Message);
            }
        }

    }
}
