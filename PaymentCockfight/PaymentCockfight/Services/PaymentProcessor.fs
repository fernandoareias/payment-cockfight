module PaymentCockfight.Services.PaymentProcessor

open PaymentCockfight.Services.IPaymentProcessor

type PaymentProcessor() =
    interface IPaymentProcessor with
        member _.Execute x =
            0