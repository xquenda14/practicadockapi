using libTransacciones;
using libCuentas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticaDock.Api.Entidades;
using Newtonsoft.Json;

namespace PracticaDock.Controllers
{
    [Route("api/Cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly IConfiguration configuracion;
        public CuentasController(IConfiguration configuration)
        {
            configuracion = configuration;
        }

        [Route("RegistrarCuenta")]
        [HttpPost]
        public async Task<ActionResult<entResult>> RegistrarCuenta(entCuenta eCuenta)
        {
            entResult eResult = new entResult();
           
            try
            {
                eResult.bError = false;
                eResult.bValido = false;
                eResult.Msg = "";

                using (rnCuentas oCuenta = new rnCuentas(configuracion))
                {
                    oCuenta.Saldo = eCuenta.Balance;
                    oCuenta.Cuenta = eCuenta.Acount;
                    await oCuenta.ValidarCuenta();

                    if (oCuenta.objError.bError) throw oCuenta.objError.uException;

                    if (!oCuenta.bExisteCuenta){ 

                        await oCuenta.RegsitrarCuenta();

                        if (!oCuenta.objError.bError)
                        {
                            eResult.bValido = true;
                            eResult.Msg = "¡La cuenta fue registrada exitosamente!";
                        }
                        else{
                            throw oCuenta.objError.uException;
                        }

                    }
                    else
                    {
                        eResult.bValido = false;
                        eResult.Msg = "¡La cuenta ya existe, favor de capturar una diferente!";
                    }
                }

            }
            catch (Exception ex)
            {

               eResult.bError = true;
                eResult.Msg = "Se genero un error interno, al momento de registrar la cuenta";
            }



            return Ok(eResult);

        }


       

        [Route("Configuracion")]
        [HttpGet]
        public ActionResult<string> GetConfiguracion()
        {
            return configuracion.GetConnectionString("cnDock");

        }

        [Route("GetCuentas")]
        [HttpGet]
        public async Task<ActionResult<entResultCuentas>> GetCuentas()
        {
            entResultCuentas eResult = new entResultCuentas();
            JsonSerializer serializer = new JsonSerializer();

            try
            {
                using (rnCuentas oCuentas = new rnCuentas(configuracion))
                {

                    //oCuentas.Cuenta = Cuenta;
                    await oCuentas.ListarCuentas();

                    if (oCuentas.objError.bError) throw oCuentas.objError.uException;

                    string jsonAux = JsonConvert.SerializeObject(oCuentas.dt);
                    eResult.ListCuentas = JsonConvert.DeserializeObject<List<entCuentaDTO>>(jsonAux);

                    eResult.bError = false;
                    eResult.bValido = true;
                }

            }
            catch (Exception ex)
            {
                eResult.bError = true;
                eResult.Msg = "¡Se genero un error interno al momento de consultar el listado de cuentas!";
                eResult.Msg = ex.Message;
            }


            return Ok(eResult);
        }


        [Route("GetCuenta")]
        [HttpGet]
        public async Task<ActionResult<entResultCuenta>> GetCuenta(int Cuenta)
        {
            entResultCuenta eResult = new entResultCuenta();
            JsonSerializer serializer = new JsonSerializer();

            try
            {
                rnTransacciones oTrans = new rnTransacciones(configuracion);

                using (rnCuentas oCuenta = new rnCuentas(configuracion))
                {

                    oCuenta.Cuenta = Cuenta;
                    await oCuenta.CargarDatosCuenta();

                    if (oCuenta.objError.bError) throw oCuenta.objError.uException;

                    if (oCuenta.dt.Rows.Count > 0)
                    {
                        eResult.eCuenta = new entCuentaDTO();
                        eResult.eCuenta.Cuenta = (int)oCuenta.Propiedades["Cuenta"];
                        eResult.eCuenta.Saldo = (decimal)oCuenta.Propiedades["Saldo"];

                        oTrans.Cuenta = Cuenta;
                        await oTrans.ObtieneSaldoActualCuenta();

                        eResult.SaldoActual = oTrans.SaldoActual;

                        oTrans.Dispose();


                    }
                    
                    eResult.bError = false;
                    eResult.bValido = true;
                }

            }
            catch (Exception ex)
            {
                eResult.bError = true;
                eResult.Msg = "¡Se genero un error interno al momento de consultar la cuenta!";

            }


            return Ok(eResult);
        }

    }
}
