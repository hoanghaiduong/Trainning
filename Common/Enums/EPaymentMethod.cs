using System.Text.Json.Serialization;

namespace Trainning.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
   public enum EPaymentMethod
    {
        CreditCard = 1,   // Represents credit card payment
        DebitCard = 2,    // Represents debit card payment
        PayPal = 3,       // Represents PayPal payment
        BankTransfer = 4, // Represents bank transfer payment
        Cash = 5,         // Represents cash payment
        Bitcoin = 6,       // Represents Bitcoin or cryptocurrency payment
    }
}