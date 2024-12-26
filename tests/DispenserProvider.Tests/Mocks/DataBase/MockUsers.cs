using Nethereum.Signer;

namespace DispenserProvider.Tests.Mocks.DataBase;

internal static class MockUsers
{
    internal static class Admin
    {
        internal static string Address => "0xBF1400be0EfBDE76936329Fe57Ff0253E16AfA34";
        internal static EthECKey PrivateKey => new("0xe290f27189205c280e53b6194580c24b4fc49c97047b5ffcb62201c4d38feece");
    }

    internal static class UnauthorizedUser
    {
        internal static string Address => "0x4a71efdc6a1DD0f1D4a5AB485a9b31BE8036c2b8";
        internal static EthECKey PrivateKey => new("0xde20dbccf7f0526fbcd3ab2a9c87d8f0aa063a22e74169f65e54afa2250577d8");
    }
}