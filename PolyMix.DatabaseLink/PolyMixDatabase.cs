using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using PolyMix.DatabaseLink.Models;

namespace PolyMix.DatabaseLink
{
    public class PolyMixDatabase
    {
        private readonly string _connectionString;

        public PolyMixDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateOrder(CreateOrderRequest request)
        {
            return CreateOrderAsync(request).GetAwaiter().GetResult();
        }

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var cmd = GetCreateOrderCommand(request, connection);
                await cmd.ExecuteNonQueryAsync();

                var orderId = (int) cmd.Parameters["ID"].Value;

                await InsertClientPriceInfo(orderId, request, connection);

                return orderId;
            }
        }

        public int CreateProcessItem(CreateProcessItemRequest request)
        {
            return CreateProcessItemAsync(request).GetAwaiter().GetResult();
        }

        public async Task<int> CreateProcessItemAsync(CreateProcessItemRequest request)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var cmd = GetCreateOrderProcessItemCommand(request, connection);
                await cmd.ExecuteNonQueryAsync();

                var itemId = (int)cmd.Parameters["RETURN_VALUE"].Value;

                return itemId;
            }
        }

        public int CreateProcessItemData(CreateProcessItemRequest itemRequest, Dictionary<string, object> itemData)
        {
            return CreateProcessItemDataAsync(itemRequest, itemData).GetAwaiter().GetResult();
        }

        public async Task<int> CreateProcessItemDataAsync(CreateProcessItemRequest itemRequest, Dictionary<string, object> itemData)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var itemId = await CreateProcessItemAsync(itemRequest);

                var processName = await GetProcessName(itemRequest.ProcessId, connection);

                var fields = itemData.Keys.Aggregate((r, x) => r + ", " + x);
                var values = itemData.Values.Select(FormatSql).Aggregate((r, x) => r + ", " + x);

                var cmd = new SqlCommand(
                    $"insert into Service_{processName} (ItemID, {fields}) values ({itemId}, {values})",
                    connection);

                await cmd.ExecuteNonQueryAsync();

                return itemId;
            }
        }

        private string FormatSql(object v)
        {
            if (v == null)
            {
                return "null";
            }

            if (v is string)
            {
                return "'" + v + "'";
            }

            if (v is bool)
            {
                return (bool) v ? "1" : "0";
            }

            // TODO: datetime

            return v.ToString();
        }

        protected SqlCommand GetCreateOrderCommand(CreateOrderRequest request, SqlConnection connection)
        {
            var cmd = new SqlCommand("dbo.up_NewOrder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("IsDraft", SqlDbType.Bit).Value = request.IsDraft ? 1 : 0;
            cmd.Parameters.Add("Tirazz", SqlDbType.Int).Value = request.Quantity;
            cmd.Parameters.Add("Comment", SqlDbType.VarChar).Value = request.Name;
            cmd.Parameters.Add("Comment2", SqlDbType.VarChar).Value = DBNull.Value;
            cmd.Parameters.Add("ID_Date", SqlDbType.SmallInt).Value = 0;
            cmd.Parameters.Add("ID_kind", SqlDbType.SmallInt).Value = 0;
            cmd.Parameters.Add("ID_char", SqlDbType.SmallInt).Value = 0;
            cmd.Parameters.Add("ID_color", SqlDbType.SmallInt).Value = 0;
            cmd.Parameters.Add("TotalCost", SqlDbType.Decimal).Value = request.TotalCost;
            cmd.Parameters.Add("TotalGrn", SqlDbType.Decimal).Value = request.TotalCostNative;
            cmd.Parameters.Add("ClientTotal", SqlDbType.Decimal).Value = request.CustomerTotalCost;
            cmd.Parameters.Add("KindID", SqlDbType.Int).Value = request.KindId;
            cmd.Parameters.Add("StartDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("FinishDate", SqlDbType.DateTime).Value = request.FinishDate;
            cmd.Parameters.Add("Customer", SqlDbType.Int).Value = request.CustomerId;
            cmd.Parameters.Add("Course", SqlDbType.Decimal).Value = request.Rate;
            cmd.Parameters.Add("IncludeAdv", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("RowColor", SqlDbType.Int).Value = request.RowColor;
            cmd.Parameters.Add("OrderState", SqlDbType.Int).Value = request.OrderState;
            cmd.Parameters.Add("PayState", SqlDbType.Int).Value = request.PayState;
            cmd.Parameters.Add("CostProtected", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("ContentProtected", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("PrePayPercent", SqlDbType.Float).Value = 0;
            cmd.Parameters.Add("PreShipPercent", SqlDbType.Float).Value = 100;
            cmd.Parameters.Add("PayDelay", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("IsPayDelayInBankDays", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("CallCustomer", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("CallCustomerPhone", SqlDbType.VarChar).Value = DBNull.Value;
            cmd.Parameters.Add("HaveLayout", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("HavePattern", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("ProductFormat", SqlDbType.VarChar).Value = DBNull.Value;
            cmd.Parameters.Add("ProductPages", SqlDbType.Int).Value = DBNull.Value;
            cmd.Parameters.Add("SignManager", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("SignProof", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("IncludeCover", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("IsComposite", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("HaveProof", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("ExternalId", SqlDbType.VarChar).Value = DBNull.Value;
            cmd.Parameters.Add("ID", SqlDbType.Int).Direction = ParameterDirection.InputOutput;
            cmd.Parameters["ID"].Value = DBNull.Value;
            return cmd;
        }

        protected SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        protected async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected SqlCommand GetCreateOrderProcessItemCommand(CreateProcessItemRequest request, SqlConnection connection)
        {
            var cmd = new SqlCommand("dbo.up_NewOrderProcessItem", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("OrderID", SqlDbType.Int).Value = request.OrderId;
            cmd.Parameters.Add("Part", SqlDbType.Int).Value = request.Part;
            cmd.Parameters.Add("ItemDesc", SqlDbType.VarChar).Value = request.Description ?? (object) DBNull.Value;
            cmd.Parameters.Add("Cost", SqlDbType.Decimal).Value = request.Cost;
            cmd.Parameters.Add("Enabled", SqlDbType.Bit).Value = request.Enabled ? 1 : 0;
            cmd.Parameters.Add("ProcessID", SqlDbType.Int).Value = request.ProcessId;
            cmd.Parameters.Add("PlanStartDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("PlanFinishDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("FactStartDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("FactFinishDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("ItemProfit", SqlDbType.Decimal).Value = (object) request.ItemProfit ?? DBNull.Value;
            cmd.Parameters.Add("IsItemInProfit", SqlDbType.Bit).Value = request.IsItemInProfit ? 0 : 1;
            cmd.Parameters.Add("EquipCode", SqlDbType.Int).Value = (object) request.EquipCode ?? DBNull.Value;
            cmd.Parameters.Add("ProductIn", SqlDbType.Int).Value = (object) request.ProductIn ?? DBNull.Value;
            cmd.Parameters.Add("ProductOut", SqlDbType.Int).Value = (object) request.ProductOut ?? DBNull.Value;
            cmd.Parameters.Add("Multiplier", SqlDbType.Float).Value = (object) request.Multiplier ?? DBNull.Value;
            cmd.Parameters.Add("FactProductIn", SqlDbType.Int).Value = DBNull.Value;
            cmd.Parameters.Add("FactProductOut", SqlDbType.Int).Value = DBNull.Value;
            cmd.Parameters.Add("Contractor", SqlDbType.Int).Value = (object) request.ContractorId ?? DBNull.Value;
            cmd.Parameters.Add("ContractorPercent", SqlDbType.Decimal).Value = (object) request.ContractorPercent ?? DBNull.Value;
            cmd.Parameters.Add("ContractorProcess", SqlDbType.Bit).Value = request.ContractorProcess ? 0 : 1;
            cmd.Parameters.Add("OwnCost", SqlDbType.Decimal).Value = request.OwnCost;
            cmd.Parameters.Add("ContractorCost", SqlDbType.Decimal).Value = request.ContractorCost;
            cmd.Parameters.Add("OwnPercent", SqlDbType.Decimal).Value = request.OwnPercent;
            cmd.Parameters.Add("MatCost", SqlDbType.Decimal).Value = request.MatCost;
            cmd.Parameters.Add("MatPercent", SqlDbType.Decimal).Value = (object) request.MatPercent ?? DBNull.Value;
            cmd.Parameters.Add("FactContractorCost", SqlDbType.Decimal).Value = DBNull.Value;
            cmd.Parameters.Add("EstimatedDuration", SqlDbType.Int).Value = (object) request.EstimatedDuration ?? DBNull.Value;
            cmd.Parameters.Add("LinkedItemID", SqlDbType.Int).Value = (object) request.LinkedItemId ?? DBNull.Value;
            cmd.Parameters.Add("ContractorPayDate", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("SideCount", SqlDbType.Int).Value = (object) request.SideCount ?? DBNull.Value;
            var returnParameter = cmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            return cmd;
        }

        private async Task InsertClientPriceInfo(int orderId, CreateOrderRequest request, SqlConnection connection)
        {
            var clientPriceId = await GetProcessId("ClientPrice", connection);

            var itemId = await CreateProcessItemAsync(new CreateProcessItemRequest
            {
                OrderId = orderId,
                Enabled = true,
                Description = String.Empty,
                Part = 1001,
                ProcessId = clientPriceId,
                Cost = 1,
                ItemProfit = 0,
                ContractorPercent = 0,
                OwnPercent = 0
            });

            var cmd = new SqlCommand(
                "insert into Service_ClientPrice (ItemID, Profit, ProfitPercent, ProfitByPercent, CommonContractorPercent, ContractorProfit, CommonMatPercent," +
                " MatProfitByPercent, ContractorProfitByPercent, CommonOwnPercent, OwnProfitByPercent, OwnProfit, TotalOwnCostCopy, TotalContractorCostCopy, TotalMatCostCopy," +
                " TotalProfitCost, FinalOwnCost, FinalContractorCost, FinalMatCost, FinalCost)" +
                $" values ({itemId}, 0, 0, 1, 0, 0, 0," + 
                $" 1, 1, 0, 1, 0, {request.OwnCost}, {request.ContractorCost}, {request.MatCost}," +
                $" {request.TotalCost}, {request.OwnCost}, {request.ContractorCost}, {request.MatCost}, {request.TotalCost})",
                connection);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<int> GetProcessId(string processName, SqlConnection connection)
        {
            var query = new SqlCommand($"select SrvId from Services where SrvName = '{processName}'", connection);
            var id = await query.ExecuteScalarAsync();
            return (int)id;
        }

        private async Task<string> GetProcessName(int processId, SqlConnection connection)
        {
            var query = new SqlCommand($"select SrvName from Services where SrvId = '{processId}'", connection);
            var name = await query.ExecuteScalarAsync();
            return (string)name;
        }
    }
}
