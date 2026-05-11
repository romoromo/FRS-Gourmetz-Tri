using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories.Interfaces
{
    public interface ISqlAppLock
    {
        Task<IAsyncDisposable?> TryAcquireAsync(string resource, int timeoutMs = 0, CancellationToken ct = default);
    }

    public sealed class SqlAppLock : ISqlAppLock
    {
        private readonly string _connStr;
        // Old constructor — STILL THERE, not removed
        public SqlAppLock(string connStr) => _connStr = connStr;
		
		// New constructor for Dependency Injection
        public SqlAppLock(IConfiguration configuration)
        {
            _connStr = configuration["ConnectionStrings:DefaultConnection"];
        }

        public async Task<IAsyncDisposable?> TryAcquireAsync(string resource, int timeoutMs = 0, CancellationToken ct = default)
        {
            var conn = new SqlConnection(_connStr);
            await conn.OpenAsync(ct);

            try
            {
                using var cmd = new SqlCommand("sp_getapplock", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Resource", resource);
                cmd.Parameters.AddWithValue("@LockMode", "Exclusive");
                cmd.Parameters.AddWithValue("@LockOwner", "Session");
                cmd.Parameters.AddWithValue("@LockTimeout", timeoutMs);

                var ret = cmd.Parameters.Add("@return_value", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                await cmd.ExecuteNonQueryAsync(ct);
                var code = (int)ret.Value;

                if (code < 0)
                {
                    await conn.DisposeAsync();
                    return null;
                }

                return new Releaser(conn, resource);
            }
            catch
            {
                await conn.DisposeAsync();
                throw;
            }
        }

        private sealed class Releaser : IAsyncDisposable
        {
            private readonly SqlConnection _conn;
            private readonly string _resource;

            public Releaser(SqlConnection conn, string resource)
            {
                _conn = conn;
                _resource = resource;
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    using var cmd = new SqlCommand("sp_releaseapplock", _conn)
                    { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@Resource", _resource);
                    cmd.Parameters.AddWithValue("@LockOwner", "Session");
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await _conn.DisposeAsync();
                }
            }
        }
    }
}
