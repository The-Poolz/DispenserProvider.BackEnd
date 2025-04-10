﻿using Newtonsoft.Json;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class ValidatedAdminRequest<TMessage>
    where TMessage : IValidatedMessage
{
    [JsonRequired]
    public string Signature { get; set; } = null!;

    [JsonRequired]
    public TMessage Message { get; set; } = default!;
}