﻿using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(
    IDispenserManager dispenserManager,
    IValidator<DispenserDTO> retrieveValidator
)
    : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {
        var dispenser = dispenserManager.GetDispenser(request);

        retrieveValidator.ValidateAndThrow(dispenser);

        return new RetrieveSignatureResponse(dispenser, request);
    }
}