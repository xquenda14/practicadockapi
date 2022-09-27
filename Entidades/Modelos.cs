using libCuentas;
using System.ComponentModel.DataAnnotations;

namespace PracticaDock.Api.Entidades
{
    public enum eTypeOperation
    {
        Deposit = 1,
        Withdrawal = 2
    }
    public class entCuenta:IValidatableObject
    {
        public int Acount { get; set; }
        public decimal Balance { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Acount <= 0) {
                yield return new ValidationResult("¡El número de cuenta no es válido!", new string[] { nameof(Acount) });
            }
        }
    }

    public class entTransaccion:IValidatableObject
    {
       
        public int Acount { get; set; }
        public string TypeOperation { get; set; } = "";
        public DateTime  DateTransaction { get; set; }
        public decimal Mount { get; set; }

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if((TypeOperation != "Deposit" && TypeOperation != "Withdrawal"))
            {
                yield return new ValidationResult("¡Tipo de operación no válida!", new string[] { nameof(TypeOperation) });
            }
            
            if (Mount <= 0)
            {
                yield return new ValidationResult("¡El monto de la transferencias no puede ser de valor cero o menor!", new string[] { nameof(Mount)});
            }



        }
    }

    
    public class entTransaccionDTO
    {
        public int IdTransaccion { get; set; }
        public int Cuenta { get; set; }
        public string? TipoOperacion { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public decimal Monto { get; set; }

    }

    public class entCuentaDTO
    {
        public int Cuenta { get; set; }
        public decimal Saldo { get; set; }

    }

}
