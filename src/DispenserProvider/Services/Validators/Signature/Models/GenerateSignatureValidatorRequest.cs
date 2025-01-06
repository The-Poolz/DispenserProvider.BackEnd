using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature.Models;

public class GenerateSignatureValidatorRequest(DispenserDTO dispenser, bool isRefund) : SignatureValidatorRequest(dispenser, isRefund);