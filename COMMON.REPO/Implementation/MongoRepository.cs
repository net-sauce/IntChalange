using COMMON.REPO.Abstraction;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging; // Add this namespace for logging
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace COMMON.REPO.Implementation
{
    public class MongoRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly ILogger<MongoRepository<T>> _logger; // Logger instance

        public MongoRepository(IMongoDatabase database, string collectionName, ILogger<MongoRepository<T>> logger)
        {
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is provided
            if (database == null) throw new ArgumentNullException(nameof(database));
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be null or empty.", nameof(collectionName));

            _collection = database.GetCollection<T>(collectionName);
            _logger.LogInformation("In MongoRepository Constructor: Checking existence of collection: {CollectionName}",
                collectionName);

            try
            {

                var collectionNames = database.ListCollectionNames().ToList();
                if (!collectionNames.Contains(collectionName))
                {
                    _logger.LogInformation("In MongoRepository Constructor: Collection {CollectionName} does not exist. Initializing it now.",
                        collectionName);
                    database.CreateCollection(collectionName);
                    _logger.LogInformation(
                        "In MongoRepository Constructor: Initialized {CollectionName}",
                        collectionName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while initializing database or collection.");
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            _logger.LogInformation("In GetAll: Retrieving all documents.");
            try
            {
                var result = await _collection.Find(_ => true).ToListAsync();
                _logger.LogInformation("In GetAll: Successfully retrieved all documents. Count: {Count}", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetAll: An error occurred while retrieving all documents.");
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllMatches(Expression<Func<T, bool>> predicate)
        {
            _logger.LogInformation("In GetAllMatches: Retrieving documents matching the predicate.");
            try
            {
                var result = (await _collection.FindAsync(predicate)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetAllMatches: An error occurred while retrieving matching documents.");
                throw;
            }
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate)
        {
            _logger.LogInformation("In GetSingle: Retrieving a single document matching the predicate.");
            try
            {
                var result = (await _collection.FindAsync(predicate)).FirstOrDefault();
                if (result != null)
                {
                    _logger.LogInformation("In GetSingle: Successfully retrieved the document.");
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetSingle: An error occurred while retrieving the document.");
                throw;
            }
        }

        public async Task<bool> Insert(T newState)
        {
            if (newState == null)
            {
                _logger.LogError("In Insert: The newState parameter is null.");
                throw new ArgumentNullException(nameof(newState));
            }

            _logger.LogInformation("In Insert:  Attempting to insert a new document.");
            try
            {
                await _collection.InsertOneAsync(newState);
                _logger.LogInformation("In Insert: Successfully inserted the new document.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In Insert: An error occurred while inserting the document.");
                return false;
            }
        }

        public async Task<bool> Update(T oldValue, T newValue)
        {
            if (oldValue == null)
            {
                _logger.LogError("In Update: oldValue is null.");
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (newValue == null)
            {
                _logger.LogError("In Update: newValue is null.");
                throw new ArgumentNullException(nameof(newValue));
            }

            _logger.LogInformation("In Update: Attempting to update a document.");
            try
            {
                var filter = Builders<T>.Filter
                    .Eq(r => r, oldValue);
                var result = await _collection.ReplaceOneAsync(filter, newValue);
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation("In Update: Successfully updated the document.");
                    return true;
                }

                _logger.LogWarning("In Update: No document was updated.");
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In Update: An error occurred while updating the document.");
                return false;
            }
        }
    }
}