namespace DispenserProvider
{
    public interface IBuilder
    {
        public decimal[] Params { get; }
        public string SimpleProviderAddress { get; }
    }
}