using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Funcoes
    {

        public static string MontaStringDeConexao(string datasource, string initialcatalog, string charset = "WIN1252", string password = "masterkey", string userid = "SYSDBA")
        {
            //return String.Format(@"data source={0};initial catalog={1};user id={2};Password={3};character set={4}", datasource, initialcatalog, userid, password, charset);
            return $@"initial catalog={initialcatalog};data source={datasource};user id={userid};Password={password};encoding={charset};charset={charset}";
        }

    }
}
