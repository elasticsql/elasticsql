using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESQLWebSericeCSharpAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ESQLWebService ws = new ESQLWebService("localhost:8088/api/v1/");

            ws.Connect("admin", "admin");

            Console.Read();
        }

        static void TestReset(ESQLWebService ws)
        {
            Console.WriteLine(ws.Reset("admin", "admin"));
        }

        static void TestQuery(ESQLWebService ws)
        {
            try
            {
                System.Data.DataTable d = ws.ExecuteSelectQueryAndReturnDatatable("select top 1 * from table1");
                Console.WriteLine(d);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void TestPing(ESQLWebService ws)
        {
            while (true)
            {
                Console.WriteLine(DateTime.Now + " - " + ws.Ping());
                Thread.Sleep(1200);
            }
        }
    }


}
