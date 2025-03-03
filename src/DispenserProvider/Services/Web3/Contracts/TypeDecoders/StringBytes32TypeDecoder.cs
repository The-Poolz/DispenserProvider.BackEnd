using Nethereum.ABI.Decoders;

namespace DispenserProvider.Services.Web3.Contracts.TypeDecoders;

public class StringBytes32TypeDecoder : TypeDecoder
{
    public override Type GetDefaultDecodingType()
    {
        return typeof(string);
    }

    public override bool IsSupportedType(Type type)
    {
        return type == typeof(string) || type == typeof(object);
    }

    public override object Decode(byte[] encoded, Type type)
    {
        if (!IsSupportedType(type)) throw new NotSupportedException(type + " is not supported");
        return encoded.Length > 32 ?
            new StringTypeDecoder().Decode(encoded.Skip(32).ToArray()) :
            new Bytes32TypeDecoder().Decode<string>(encoded);
    }

    public override object DecodePacked(byte[] encoded, Type type)
    {
        throw new NotImplementedException();
    }
}