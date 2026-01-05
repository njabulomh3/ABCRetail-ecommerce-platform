
    using ABCRetail_Cloud_.Models;
    using Azure;
    using Azure.Data.Tables;
  
    namespace ABCRetail_Cloud_.Services
    {
        public class TableStorageService
        {
            private readonly TableClient _productTableClient;
            private readonly TableClient _orderTableClient;
            private readonly TableClient _customerProfileTableClient;
            private readonly TableClient _contractTableClient;

            public TableStorageService(string connectionString)
            {
                _productTableClient = new TableClient(connectionString, "Products");
                _orderTableClient = new TableClient(connectionString, "Order");
                _customerProfileTableClient = new TableClient(connectionString, "CustomerProfiles");
                _contractTableClient = new TableClient(connectionString, "Contract");

                // Ensure the tables exist
                EnsureTableExistsAsync(_productTableClient).GetAwaiter().GetResult();
                EnsureTableExistsAsync(_orderTableClient).GetAwaiter().GetResult();
                EnsureTableExistsAsync(_customerProfileTableClient).GetAwaiter().GetResult();
                EnsureTableExistsAsync(_contractTableClient).GetAwaiter().GetResult();
            }

            private async Task EnsureTableExistsAsync(TableClient tableClient)
            {
                await tableClient.CreateIfNotExistsAsync();
            }

        // Products methods
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var entity in _productTableClient.QueryAsync<TableEntity>())
            {
                var product = new Product
                {
                    PartitionKey = entity.PartitionKey,
                    RowKey = entity.RowKey,
                    Product_Name = entity.GetString(nameof(Product.Product_Name)),
                    Description = entity.GetString(nameof(Product.Description)),
                    ImageUrl = entity.GetString(nameof(Product.ImageUrl)),
                    Price = entity.ContainsKey(nameof(Product.Price))
                        ? decimal.TryParse(entity[nameof(Product.Price)]?.ToString(), out var price) ? price : 0
                        : 0
                };

                products.Add(product);
            }
            return products;
        }

        public async Task AddProductAsync(Product product, string partitionKey, string rowKey)
            {
                product.PartitionKey = partitionKey;
                product.RowKey = rowKey;
                await _productTableClient.AddEntityAsync(product);
            }

            public async Task DeleteProductAsync(string partitionKey, string rowKey)
            {
                await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<Product> GetProductAsync(string partitionKey, string rowKey)
            {
                return await _productTableClient.GetEntityAsync<Product>(partitionKey, rowKey);
            }

            // Orders methods
            public async Task<List<Order>> GetAllOrdersAsync()
            {
                var orders = new List<Order>();
                await foreach (var order in _orderTableClient.QueryAsync<Order>())
                {
                    orders.Add(order);
                }
                return orders;
            }

            public async Task AddOrderAsync(Order order)
            {
                await _orderTableClient.AddEntityAsync(order);
            }

            public async Task DeleteOrderAsync(string partitionKey, string rowKey)
            {
                await _orderTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<Order> GetOrderAsync(string partitionKey, string rowKey)
            {
                return await _orderTableClient.GetEntityAsync<Order>(partitionKey, rowKey);
            }

            // CustomerProfiles methods
            public async Task<List<CustomerProfile>> GetAllCustomerProfilesAsync()
            {
                var customerProfiles = new List<CustomerProfile>();
                await foreach (var customerProfile in _customerProfileTableClient.QueryAsync<CustomerProfile>())
                {
                    customerProfiles.Add(customerProfile);
                }
                return customerProfiles;
            }

            public async Task AddCustomerProfileAsync(CustomerProfile customerProfile)
            {
                await _customerProfileTableClient.AddEntityAsync(customerProfile);
            }

            public async Task DeleteCustomerProfileAsync(string partitionKey, string rowKey)
            {
                await _customerProfileTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<CustomerProfile> GetCustomerProfileAsync(string partitionKey, string rowKey)
            {
                return await _customerProfileTableClient.GetEntityAsync<CustomerProfile>(partitionKey, rowKey);
            }

            // Contracts methods
            public async Task<List<Contract>> GetAllContractsAsync()
            {
                var contracts = new List<Contract>();
                await foreach (var contract in _contractTableClient.QueryAsync<Contract>())
                {
                    contracts.Add(contract);
                }
                return contracts;
            }

            public async Task AddContractAsync(Contract contract)
            {
                await _contractTableClient.AddEntityAsync(contract);
            }

            public async Task DeleteContractAsync(string partitionKey, string rowKey)
            {
                await _contractTableClient.DeleteEntityAsync(partitionKey, rowKey);
            }

            public async Task<Contract> GetContractAsync(string partitionKey, string rowKey)
            {
                return await _contractTableClient.GetEntityAsync<Contract>(partitionKey, rowKey);
            }

            public async Task UpdateContractAsync(Contract contract)
            {
                try
                {
                    await _contractTableClient.UpdateEntityAsync(contract, contract.ETag, TableUpdateMode.Replace);
                }
                catch (RequestFailedException ex) when (ex.Status == 412)
                {
                    // Handle precondition failed (ETag mismatch)
                    // Retry or log error
                    throw new InvalidOperationException("Contract was updated by someone else.");
                }
            }

        }
    }

