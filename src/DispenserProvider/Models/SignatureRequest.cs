﻿using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Models;

public class SignatureRequest : IGetDispenserRequest
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }
}