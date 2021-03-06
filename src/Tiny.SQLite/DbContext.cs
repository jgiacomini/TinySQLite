﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Tiny.SQLite
{
    public sealed class DbContext : IDisposable
    {
        #region Fields
        private readonly Dictionary<Type, TableMapping> _mappings = new Dictionary<Type, TableMapping>();
        private readonly QueriesManager _queriesManager;
        private readonly bool _storeDateTimeAsTicks;
        private readonly bool _removeDiacriticsOnTableNameAndColumnName;
        #endregion

        /// <summary>
        /// Create a Database context
        /// </summary>
        /// <param name="filePath">file path of the database</param>
        /// <param name="removeDiacriticsOnTableNameAndColumnName">if true the tableName and column name are generated without diacritics ex : 'crèmeBrûlée' would become 'cremeBrulee'</param>
        /// <param name="storeDateTimeAsTicks">if true store dateTime as ticks(long)</param>
        public DbContext(
            string filePath,
            bool removeDiacriticsOnTableNameAndColumnName = true,
            bool storeDateTimeAsTicks = false)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException($"{nameof(filePath)}");
            }

            _removeDiacriticsOnTableNameAndColumnName = removeDiacriticsOnTableNameAndColumnName;
            _storeDateTimeAsTicks = storeDateTimeAsTicks;
            var connection = new SqliteConnection($"Data Source={filePath};Mode=ReadWriteCreate");

            _queriesManager = new QueriesManager(connection);
            Database = new Database(_queriesManager, filePath);
        }

        private DbContext(
            bool removeDiacriticsOnTableNameAndColumnName,
            bool storeDateTimeAsTicks)
        {
            _removeDiacriticsOnTableNameAndColumnName = removeDiacriticsOnTableNameAndColumnName;

            var connection = new SqliteConnection("Data Source=:memory:;Mode=Memory");
            _storeDateTimeAsTicks = storeDateTimeAsTicks;
            _queriesManager = new QueriesManager(connection);
            Database = new Database(_queriesManager, null);
        }

        /// <summary>
        /// Create a in memory Database context
        /// </summary>
        /// <param name="busyTimeout">A timeout, in milliseconds, to wait when the database is locked before throwing a SqliteBusyException
        /// The default value is 0, which means to throw a SqliteBusyException immediately if the database is locked.
        /// </param>
        /// <param name="removeDiacriticsOnTableNameAndColumnName">if true the tableName and column name are generated without diacritics ex : 'crèmeBrûlée' would become 'cremeBrulee'</param>
        /// <param name="storeDateTimeAsTicks">if true store dateTime as ticks(long)</param>
        /// <returns>returns in memory DbContext</returns>
        public static DbContext InMemory(
            bool removeDiacriticsOnTableNameAndColumnName = true,
            bool storeDateTimeAsTicks = false)
        {
            return new DbContext(removeDiacriticsOnTableNameAndColumnName, storeDateTimeAsTicks);
        }

        public Database Database { get; private set; }

        public TableQuery<T> Table<T>()
            where T : new()
        {
            var type = typeof(T);
            TableMapping mapping = null;
            if (_mappings.ContainsKey(type))
            {
                mapping = _mappings[type];
            }
            else
            {
                var mapper = new TableMapper(_storeDateTimeAsTicks, _removeDiacriticsOnTableNameAndColumnName);
                mapping = mapper.Map(type);
                _mappings.Add(type, mapping);
            }

            return new TableQuery<T>(mapping, _queriesManager);
        }

        public Task<object> ExecuteScalarAsync(string sql, CancellationToken cancellationToken)
        {
            return _queriesManager.ExecuteScalarAsync(sql, cancellationToken);
        }

        public Task<int> ExecuteNonQueryAsync(string sql, CancellationToken cancellationToken)
        {
            return _queriesManager.ExecuteNonQueryAsync(sql, null, null, cancellationToken);
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _queriesManager.Dispose();
                }

                _disposedValue = true;
            }
        }

        ~DbContext()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}