using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using TokenSchedule.FluentValidation.Models;
using DispenserProvider.MessageTemplate.Models.Create;
using DispenserProvider.MessageTemplate.Models.Eip712;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using ValidationUser = DispenserProvider.MessageTemplate.Models.Create.User;
using ValidationRefund = DispenserProvider.MessageTemplate.Models.Create.Refund;
using ValidationSchedule = DispenserProvider.MessageTemplate.Models.Create.Schedule;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetMessage : IValidatedMessage, IValidatedCreateMessage
{
    [JsonRequired]
    public Schedule[] Schedules { get; set; } = [];

    [JsonRequired]
    public User[] Users { get; set; } = [];

    public Refund? Refund { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonIgnore]
    public AbstractMessage Eip712Message => Refund == null
        ? new CreateMessage(
            chainId: ChainId,
            poolId: PoolId,
            schedules: Schedules.Select(x => new ValidationSchedule(x.ProviderAddress, x.Ratio, x.StartDate, x.FinishDate)),
            users: Users.Select(x => new ValidationUser(x.UserAddress, x.WeiAmount))
        )
        : new CreateMessageWithRefund(
            chainId: ChainId,
            poolId: PoolId,
            schedules: Schedules.Select(x => new ValidationSchedule(x.ProviderAddress, x.Ratio, x.StartDate, x.FinishDate)),
            users: Users.Select(x => new ValidationUser(x.UserAddress, x.WeiAmount)),
            refund: new ValidationRefund(
                chainId: Refund.ChainId,
                poolId: Refund.PoolId,
                ratio: Refund.Ratio,
                dealProvider: Refund.DealProvider,
                finishTime: Refund.FinishTime
            )
        );

    [JsonIgnore]
    public IEnumerable<EthereumAddress> UsersToValidate => Users.Select(x => x.UserAddress);

    [JsonIgnore]
    public IEnumerable<IValidatedScheduleItem> ScheduleToValidate => Schedules.Select(x => new ValidatedSchedule(x));
}