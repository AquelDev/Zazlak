using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak;
using Zazlak.Storage;

namespace Zazlak.Storage
{
    class MySQL
    {
        public bool Enabled = false;
        DatabaseClient mysql;

        private DataTable Result;

        public MySQL(DatabaseManager Db)
        {
            mysql = Db.GetClient();
        }

        public void Query(string Query)
        {
            Result = this.mysql.query_read(Query);
            Enabled = true;
        }

        public DataRowCollection Fetch_Array()
        {
            if (Enabled)
            {
                DataRow Row = Result.Rows.Count > 0 ? Result.Rows[0] : null;
                return Row.Table.Rows;
            }
            else
                return null;
        }

        public DataRow Fetch_Assoc()
        {
            if (Enabled)
                return Result.Rows.Count > 0 ? Result.Rows[0] : null;
            else
                return null;
        }

        public int Num_Rows()
        {
            if (Enabled)
                return Result.Rows.Count;
            else
                return 0;
        }
    }
}
