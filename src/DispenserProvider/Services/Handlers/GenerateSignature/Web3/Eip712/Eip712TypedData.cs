using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nethereum.ABI.EIP712;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3.Eip712;

[JsonObject(MemberSerialization.OptIn)]
public class Eip712TypedData : TypedData<Eip712Domain>
{
    public Eip712TypedData(TransactionDetailDTO transactionDetail, EthereumAddress verifyingContract, DateTime validUntil)
    {
        Domain = new Eip712Domain(transactionDetail, verifyingContract);
        DomainRawValues = MemberValueFactory.CreateFromMessage(Domain);
        PrimaryType = "SigStruct";
        Types = MemberDescriptionFactory.GetTypesMemberDescription(
            types: [typeof(Builder), typeof(SignatureStruct), typeof(Eip712Domain)]
        );
        Message = MemberValueFactory.CreateFromMessage(
            new SignatureStruct(transactionDetail, validUntil)
        );
    }

    public string ToEip712Json()
    {
        var originalObj = JObject.Parse(this.ToJson());
        var reorderedObj = new JObject
        {
            ["types"] = originalObj["types"],
            ["domain"] = originalObj["domain"],
            ["primaryType"] = originalObj["primaryType"],
            ["message"] = originalObj["message"]
        };
        return reorderedObj.ToString(Formatting.Indented);
    }
}