using System.Text.Json.Serialization;

namespace PracticaDock.Api.Entidades
{
    public class entResult
    {
        public bool bError { get; set; }
        public bool bValido { get; set;}
        public string Msg { get; set; }
    }



    public class entResultTrans:entResult
    {
        //public bool bError { get; set; }
        //public bool bValido { get; set; }
        //public string Msg { get; set; }
        [JsonInclude]
        public List<entTransaccionDTO>? ListTrans;

    }

    public class entResultCuentas : entResult
    {
        [JsonInclude]
        public List<entCuentaDTO>? ListCuentas;

    }

    public class entResultCuenta : entResult
    {
        [JsonInclude]
        public entCuentaDTO? eCuenta { get; set; }

        public decimal SaldoActual { get; set; }

    }
    public class eParamTrans
    {
        public int Cuenta { get; set; }

    }
}
