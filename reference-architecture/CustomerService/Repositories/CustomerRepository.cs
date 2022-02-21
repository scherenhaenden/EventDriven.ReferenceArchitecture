﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;

namespace CustomerService.Repositories
{
    public class CustomerRepository : DocumentRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoCollection<Customer> collection) : base(collection)
        {
        }

        public async Task<IEnumerable<Customer>> GetAsync() =>
            await FindManyAsync();

        public async Task<Customer> GetAsync(Guid id) =>
            await FindOneAsync(e => e.Id == id);

        public async Task<Customer> AddAsync(Customer entity)
        {
            var existing = await FindOneAsync(e => e.Id == entity.Id);
            if (existing != null) return null;
            entity.SequenceNumber = 1;
            entity.ETag = Guid.NewGuid().ToString();
            return await InsertOneAsync(entity);
        }

        public async Task<Customer> UpdateAsync(Customer entity)
        {
            var existing = await GetAsync(entity.Id);
            if (existing == null) return null;
            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
                throw new ConcurrencyException();
            entity.SequenceNumber = existing.SequenceNumber + 1;
            entity.ETag = Guid.NewGuid().ToString();
            return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
        }

        public async Task<int> RemoveAsync(Guid id) =>
            await DeleteOneAsync(e => e.Id == id);
    }
}
