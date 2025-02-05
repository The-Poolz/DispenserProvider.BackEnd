using DispenserProvider.Attributes;

namespace DispenserProvider;

public enum ErrorCode
{
    [Error($"No one valid stage in '{nameof(Env.PRODUCTION_MODE)}' environment variable found.")]
    INVALID_STAGE,
    [Error("No one implemented request found.")]
    INVALID_HANDLER_REQUEST,
    [Error("Asset by provided PoolId and ChainId for user, not found.")]
    DISPENSER_NOT_FOUND,
    [Error("Some addresses, specified by ChainId and PoolId, were not found.")]
    USERS_FOR_DELETE_NOT_FOUND,
    [Error("ChainId, not supported.")]
    CHAIN_NOT_SUPPORTED,
    [Error("Cannot generate signature, because asset already withdrawn.")]
    ASSET_ALREADY_WITHDRAWN,
    [Error("Cannot generate signature, because asset already refunded.")]
    ASSET_ALREADY_REFUNDED,
    [Error("Cannot generate signature for refund, because refund time has expired.")]
    REFUND_TIME_IS_EXPIRED,
    [Error("Signature for user, not found.")]
    SIGNATURE_NOT_FOUND,
    [Error("Cannot retrieve signature, because the valid time for retrieving has not yet arrived.")]
    SIGNATURE_VALID_TIME_NOT_ARRIVED,
    [Error("Cannot retrieve signature, because the valid time for using signature is expired.")]
    SIGNATURE_VALID_TIME_IS_EXPIRED,
    [Error("Cannot retrieve signature, because it was generated for a other operation.")]
    SIGNATURE_TYPE_IS_INVALID,
    [Error("Cannot generate signature, because it is still valid until.")]
    SIGNATURE_IS_STILL_VALID,
    [Error("Cannot generate signature, because the next valid time for generation has not yet arrived.")]
    SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED
}