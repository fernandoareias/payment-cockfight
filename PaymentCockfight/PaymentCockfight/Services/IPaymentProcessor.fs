module PaymentCockfight.Services.IPaymentProcessor

type IPaymentProcessor =
    abstract Execute: string -> int