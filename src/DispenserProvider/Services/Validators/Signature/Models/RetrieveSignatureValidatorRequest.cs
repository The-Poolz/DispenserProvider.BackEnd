using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Validators.Signature.Models;

public class RetrieveSignatureValidatorRequest(DispenserDTO dispenser, bool isRefund) : SignatureValidatorRequest(dispenser, isRefund);