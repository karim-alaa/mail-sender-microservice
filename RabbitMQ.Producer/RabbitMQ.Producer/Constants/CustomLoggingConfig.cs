using RabbitMQ.Producer.Dtos.Config;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Constants
{
    public interface ICustomLoggingConfig
    {
        ColumnOptions GetColumnOptions();
        MSSqlServerSinkOptions GetSinkOpts();
    }
    public class CustomLoggingConfig : ICustomLoggingConfig
    {
        private readonly AppConfig _config;
        public CustomLoggingConfig(AppConfig config)
        {
            _config = config;
        }

        private readonly ColumnOptions ColumnOptions = new()
        {
            // Custom logging table
            AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "MachineName", DataLength = 256, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "Method", DataLength = 20, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.Int, ColumnName = "StatusCode",DataLength = 3, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "Uri", DataLength = -1, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "ReqBody", DataLength = -1, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "ReqHead", DataLength = -1, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "ResBody", DataLength = -1, AllowNull = true},
                    new SqlColumn {DataType = SqlDbType.NVarChar, ColumnName = "ResHead", DataLength = -1, AllowNull = true},
                }
        };

        private readonly MSSqlServerSinkOptions SinkOpts = new()
        {
            AutoCreateSqlTable = true,
            BatchPostingLimit = 1000
        };

        public MSSqlServerSinkOptions GetSinkOpts()
        {
            SinkOpts.TableName = _config.CustomLogging.WriteTo.FirstOrDefault().Args.TableName;
            return SinkOpts;
        }

        public ColumnOptions GetColumnOptions()
        {
            ColumnOptions.Store.Remove(StandardColumn.Properties);
            ColumnOptions.Store.Remove(StandardColumn.Message);
            ColumnOptions.Store.Remove(StandardColumn.MessageTemplate);
            ColumnOptions.Store.Remove(StandardColumn.Exception);
            return ColumnOptions;
        }
    }
}
