using System.Data;
using Dapper;
using projectDemo.UnitOfWorks;

namespace projectDemo.Repository.BaseData
{
    public class RepositoryProcBase
    {
        protected readonly IDbConnection _connection;
        protected readonly IDbTransaction _transaction;

        public RepositoryProcBase(IUnitOfWork uow)
        {
            _connection = uow.connection;
            _transaction = uow.GetTransaction();
        }

        // trả về list
        protected async Task<IEnumerable<T>> QueryAsync<T>(string proc, object param = null)
        {
            return await _connection.QueryAsync<T>(
                proc,
                param,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        // trả về 1 đối tượng
        protected async Task<T?> QueryFirstAsync<T>(string proc, object param = null)
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(
                proc,
                param,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        //thực hiện câu truy vấn
        protected async Task<int> ExecuteAsync(string proc, object param = null)
        {
            return await _connection.ExecuteAsync(
                proc,
                param,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
