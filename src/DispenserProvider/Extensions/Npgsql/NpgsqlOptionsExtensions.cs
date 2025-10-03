using Microsoft.EntityFrameworkCore;

namespace DispenserProvider.Extensions.Npgsql;

public static class NpgsqlOptionsExtensions
{
    public static DbContextOptionsBuilder UseProdNpgsql(this DbContextOptionsBuilder options)
    {
        return options.UseNpgsql(NpgsqlConnectionStringFactory.BuildProd());
    }

    public static DbContextOptionsBuilder UseStageNpgsql(this DbContextOptionsBuilder options)
    {
        return options.UseNpgsql(NpgsqlConnectionStringFactory.BuildStage());
    }
}