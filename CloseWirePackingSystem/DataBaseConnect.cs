using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloseWirePackingSystem
{
    abstract class DataBaseConnect
    {
        public string Server { get; set; }
        public string DefaultDataBase { get; set; }
        public string Usr { get; set; }
        public string Pwd { get; set; }

        public abstract bool OpenConnection();
        public abstract bool CloseConnection();
        public abstract bool DataInsert();
        public abstract bool DataDelete();
        public abstract bool DataUpdate();
        public abstract DataSet DataSelect();
    }
}
