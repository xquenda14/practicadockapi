using libCuentas;
using libTransacciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PracticaDock.Api.Entidades;

namespace PracticaDock.Api.Controllers
{
    [Route("api/Transacciones")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public TransaccionesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

       
        
        [Route("RegistrarTransaccion")]
        [HttpPost]
        public async Task<ActionResult<string>> RegistrarTransaccion(entTransaccion eTrans)
        {
            entResult eResult = new entResult();

            eResult.Msg = "";
            eResult.bError = false;
            eResult.bValido = false;
            try
            {
                rnCuentas oCuenta = new rnCuentas(configuration);

                using (rnTransacciones oTrans = new rnTransacciones(configuration))
                {
                    decimal MontoMaximoDepositoMes = configuration.GetValue<decimal>("MontoMaximoDepositoMes");

                    oCuenta.Cuenta = eTrans.Acount;
                    await oCuenta.ValidarCuenta();

                    if (!oCuenta.bExisteCuenta)
                    {
                        eResult.Msg = "La cuenta no existe, favor de verificar";
                    }
                    else if (eTrans.TypeOperation == "Deposit" )
                    {
                        oTrans.IdTipoOperación = (Int16)eTypeOperation.Deposit;

                        oTrans.Cuenta = eTrans.Acount;
                        oTrans.Monto = eTrans.Mount;
                        oTrans.FechaOperacion = eTrans.DateTransaction;
                        await oTrans.ObtieneMontoDepositosMes();

                        if (oTrans.objError.bError) throw oTrans.objError.uException;

                        if(oTrans.MontoDepositosMes > MontoMaximoDepositoMes)
                        {
                            //eResult.bValido = false;
                            eResult.Msg = "Monto ingresado no es permitido en esta transacción, el monto máximo permitido por mes es de " +  MontoMaximoDepositoMes.ToString("C") ;
                            //throw new Exception(eResult.Msg);
                        }

                    }
                    else if(eTrans.TypeOperation == "Withdrawal")
                    {
                        oTrans.IdTipoOperación = (Int16)eTypeOperation.Withdrawal;
                        oTrans.Cuenta = eTrans.Acount;
                        oTrans.Monto = eTrans.Mount;
                        await oTrans.ObtieneBalanceCuenta();

                        if (oTrans.objError.bError) throw oTrans.objError.uException;  

                        if (oTrans.BalanceCuenta < 0)
                        {
                            //eResult.bValido = false;
                            eResult.Msg = "¡No es posible realizar la transacción por fondos insuficientes!";
                        }

                        
                    }
                   
                    eResult.bValido = eResult.Msg != "" ? false : true;

                    if (eResult.bValido)
                    {
                        oTrans.Cuenta = eTrans.Acount;
                        oTrans.Monto = eTrans.Mount;
                        oTrans.FechaOperacion = eTrans.DateTransaction;
                        await oTrans.RegistrarTransaccion() ;

                        if (!oTrans.objError.bError)
                        {
                            eResult.bError = false;
                            
                            eResult.Msg = "¡Transacción realizada con exito, su saldo actual es de " + oTrans.SaldoActual.ToString("C") + "!";
                        }
                        else
                        {
                            throw oTrans.objError.uException;
                        }

                    }
                   
                }
                oCuenta.Dispose();



            }
            catch (Exception ex)
            {

                eResult.bError = true;
                eResult.Msg = "Se genero un error interno, al momento de registrar la transacción";
            }

            if (eResult.bError)
            {
                return BadRequest(eResult);
            }
            else if (eResult.bValido) {
                return Ok(eResult);
            }
            else
            {
                return BadRequest(eResult);
            }

            

        }

        [Route("GetTransacciones")]
        [HttpGet]
        public async Task<ActionResult<entResultTrans>> GetTransacciones(int Cuenta)
        {
            entResultTrans eResult = new entResultTrans();
            //JsonSerializer serializer = new JsonSerializer();

            //eParamTrans? eParam = new eParamTrans();

            //eParam= JsonConvert.DeserializeObject<eParamTrans>(sParam);


            try
            {
                using (rnTransacciones oTrans = new rnTransacciones(configuration))
                {

                    oTrans.Cuenta = Cuenta;
                    await oTrans.ListarTransacciones();

                    if (oTrans.objError.bError) throw oTrans.objError.uException;

                    string jsonAux = JsonConvert.SerializeObject(oTrans.dt);
                    eResult.ListTrans = JsonConvert.DeserializeObject<List<entTransaccionDTO>>(jsonAux);

                    eResult.bValido = true;
                    eResult.Msg = "Exitoso";
                }

            }
            catch (Exception ex)
            {
                eResult.bError = true;
                eResult.Msg = "¡Se genero un error interno al momento de consultar el listado de transacciones!";

            }

           
            return eResult;
        }



    }
}
