using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api_infor_cell.src.Repository
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        public async Task<ResponseApi<dynamic>> GetCardsAsync(string plan, string company, string store)
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                DateTime startOfMonth     = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth       = startOfMonth.AddMonths(1).AddTicks(-1);
                DateTime startOfPrevMonth = startOfMonth.AddMonths(-1);
                DateTime endOfPrevMonth   = startOfMonth.AddTicks(-1);

                // ── Vendas do mês atual ───────────────────────────────────────
                List<BsonDocument> salesThisMonthPipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",   false },
                        { "plan",      plan },
                        { "company",   company },
                        { "store",     store },
                        { "status",    "Finalizado" },
                        { "createdAt", new BsonDocument { { "$gte", startOfMonth }, { "$lte", endOfMonth } } }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",   BsonNull.Value },
                        { "total", new BsonDocument("$sum", "$total") },
                        { "count", new BsonDocument("$sum", 1) }
                    })
                };
                BsonDocument? salesThisMonth = await context.SalesOrders
                    .Aggregate<BsonDocument>(salesThisMonthPipeline)
                    .FirstOrDefaultAsync();

                // ── Vendas do mês anterior ────────────────────────────────────
                List<BsonDocument> salesPrevMonthPipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",   false },
                        { "plan",      plan },
                        { "company",   company },
                        { "store",     store },
                        { "status",    "Finalizado" },
                        { "createdAt", new BsonDocument { { "$gte", startOfPrevMonth }, { "$lte", endOfPrevMonth } } }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",   BsonNull.Value },
                        { "total", new BsonDocument("$sum", "$total") },
                        { "count", new BsonDocument("$sum", 1) }
                    })
                };
                BsonDocument? salesPrevMonth = await context.SalesOrders
                    .Aggregate<BsonDocument>(salesPrevMonthPipeline)
                    .FirstOrDefaultAsync();

                // ── Pedidos em aberto ─────────────────────────────────────────
                long openOrdersCount = await context.SalesOrders.CountDocumentsAsync(
                    new BsonDocument
                    {
                        { "deleted",  false },
                        { "plan",     plan },
                        { "company",  company },
                        { "store",    store },
                        { "status",   "Em Aberto" }
                    }
                );

                // ── Estoque — valor total e quantidade de itens ───────────────
                List<BsonDocument> stockPipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",  false },
                        { "plan",     plan },
                        { "company",  company },
                        { "store",    store }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",        BsonNull.Value },
                        { "totalValue", new BsonDocument("$sum", new BsonDocument("$multiply", new BsonArray
                            {
                                new BsonDocument("$toDouble", "$price"),
                                new BsonDocument("$toDouble", "$quantity")
                            }))
                        },
                        { "totalItems", new BsonDocument("$sum", new BsonDocument("$toDouble", "$quantity")) }
                    })
                };
                BsonDocument? stockSummary = await context.Stocks
                    .Aggregate<BsonDocument>(stockPipeline)
                    .FirstOrDefaultAsync();

                // ── Clientes cadastrados no mês atual ─────────────────────────
                long customersThisMonth = await context.Customers.CountDocumentsAsync(
                    new BsonDocument
                    {
                        { "deleted",   false },
                        { "plan",      plan },
                        { "company",   company },
                        { "createdAt", new BsonDocument { { "$gte", startOfMonth }, { "$lte", endOfMonth } } }
                    }
                );

                // ── Clientes cadastrados no mês anterior ──────────────────────
                long customersPrevMonth = await context.Customers.CountDocumentsAsync(
                    new BsonDocument
                    {
                        { "deleted",   false },
                        { "plan",      plan },
                        { "company",   company },
                        { "createdAt", new BsonDocument { { "$gte", startOfPrevMonth }, { "$lte", endOfPrevMonth } } }
                    }
                );

                // ── Contas a Receber ──────────────────────────────────────────
                List<BsonDocument> arPipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",  false },
                        { "plan",     plan },
                        { "company",  company },
                        { "store",    store }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",   "$status" },
                        { "total", new BsonDocument("$sum", "$amount") },
                        { "count", new BsonDocument("$sum", 1) }
                    })
                };
                List<BsonDocument> arSummary = await context.AccountsReceivable
                    .Aggregate<BsonDocument>(arPipeline)
                    .ToListAsync();

                decimal arOpen = 0; long arOpenCount = 0;
                decimal arOverdue = 0; long arOverdueCount = 0;
                foreach (BsonDocument doc in arSummary)
                {
                    string status = doc["_id"].IsBsonNull ? "" : doc["_id"].AsString;
                    decimal val   = (decimal)doc["total"].ToDouble();
                    long cnt      = doc["count"].AsInt32;
                    if (status == "open")    { arOpen    = val; arOpenCount    = cnt; }
                    if (status == "overdue") { arOverdue = val; arOverdueCount = cnt; }
                }

                // ── Contas a Pagar ────────────────────────────────────────────
                List<BsonDocument> apPipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",  false },
                        { "plan",     plan },
                        { "company",  company },
                        { "store",    store }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",   "$status" },
                        { "total", new BsonDocument("$sum", "$amount") },
                        { "count", new BsonDocument("$sum", 1) }
                    })
                };
                List<BsonDocument> apSummary = await context.AccountsPayable
                    .Aggregate<BsonDocument>(apPipeline)
                    .ToListAsync();

                decimal apOpen = 0; long apOpenCount = 0;
                decimal apOverdue = 0; long apOverdueCount = 0;
                foreach (BsonDocument doc in apSummary)
                {
                    string status = doc["_id"].IsBsonNull ? "" : doc["_id"].AsString;
                    decimal val   = (decimal)doc["total"].ToDouble();
                    long cnt      = doc["count"].AsInt32;
                    if (status == "open")    { apOpen    = val; apOpenCount    = cnt; }
                    if (status == "overdue") { apOverdue = val; apOverdueCount = cnt; }
                }

                // ── Cálculo de crescimento ────────────────────────────────────
                decimal salesCurrentTotal = salesThisMonth != null ? (decimal)salesThisMonth["total"].ToDouble() : 0;
                long    salesCurrentCount = salesThisMonth != null ? salesThisMonth["count"].AsInt32 : 0;
                decimal salesPrevTotal    = salesPrevMonth != null ? (decimal)salesPrevMonth["total"].ToDouble() : 0;

                decimal salesGrowth = salesPrevTotal > 0
                    ? Math.Round((salesCurrentTotal - salesPrevTotal) / salesPrevTotal * 100, 1)
                    : salesCurrentTotal > 0 ? 100 : 0;

                decimal customersGrowth = customersPrevMonth > 0
                    ? Math.Round((customersThisMonth - customersPrevMonth) / (decimal)customersPrevMonth * 100, 1)
                    : customersThisMonth > 0 ? 100 : 0;

                decimal stockTotalValue = stockSummary != null ? (decimal)stockSummary["totalValue"].ToDouble() : 0;
                decimal stockTotalItems = stockSummary != null ? (decimal)stockSummary["totalItems"].ToDouble() : 0;

                // ── Monta resposta ────────────────────────────────────────────
                dynamic result = new
                {
                    sales = new
                    {
                        totalMonth    = salesCurrentTotal,
                        countMonth    = salesCurrentCount,
                        growthPercent = salesGrowth,
                        openOrders    = openOrdersCount,
                    },
                    stock = new
                    {
                        totalValue = stockTotalValue,
                        totalItems = stockTotalItems,
                    },
                    customers = new
                    {
                        countMonth    = customersThisMonth,
                        growthPercent = customersGrowth,
                    },
                    accountsReceivable = new
                    {
                        openAmount    = arOpen,
                        openCount     = arOpenCount,
                        overdueAmount = arOverdue,
                        overdueCount  = arOverdueCount,
                        totalAmount   = arOpen + arOverdue,
                        totalCount    = arOpenCount + arOverdueCount,
                    },
                    accountsPayable = new
                    {
                        openAmount    = apOpen,
                        openCount     = apOpenCount,
                        overdueAmount = apOverdue,
                        overdueCount  = apOverdueCount,
                        totalAmount   = apOpen + apOverdue,
                        totalCount    = apOpenCount + apOverdueCount,
                    }
                };

                return new(result);
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        // ── Vendas por mês do ano atual (gráfico de barras e linha) ───────────
        public async Task<ResponseApi<dynamic>> GetMonthlySalesAsync(string plan, string company, string store)
        {
            try
            {
                int currentYear = DateTime.UtcNow.Year;

                List<BsonDocument> pipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",   false },
                        { "plan",      plan },
                        { "company",   company },
                        { "store",     store },
                        { "status",    "Finalizado" },
                        { "createdAt", new BsonDocument
                            {
                                { "$gte", new DateTime(currentYear, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                                { "$lte", new DateTime(currentYear, 12, 31, 23, 59, 59, DateTimeKind.Utc) }
                            }
                        }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id",   new BsonDocument("$month", "$createdAt") },
                        { "total", new BsonDocument("$sum", "$total") },
                        { "count", new BsonDocument("$sum", 1) }
                    }),
                    new("$sort", new BsonDocument("_id", 1))
                };

                List<BsonDocument> results = await context.SalesOrders
                    .Aggregate<BsonDocument>(pipeline)
                    .ToListAsync();

                // Monta array com 12 posições (Jan-Dez), zero para meses sem venda
                decimal[] totals = new decimal[12];
                int[] counts = new int[12];
                foreach (BsonDocument doc in results)
                {
                    int month = doc["_id"].AsInt32 - 1;
                    totals[month] = (decimal)doc["total"].ToDouble();
                    counts[month] = doc["count"].AsInt32;
                }

                dynamic result = new
                {
                    totals,
                    counts
                };

                return new(result);
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        // ── Meta mensal: mês atual vs mês anterior ────────────────────────────
        public async Task<ResponseApi<dynamic>> GetMonthlyTargetAsync(string plan, string company, string store)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime startOfMonth     = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth       = startOfMonth.AddMonths(1).AddTicks(-1);
                DateTime startOfPrevMonth = startOfMonth.AddMonths(-1);
                DateTime endOfPrevMonth   = startOfMonth.AddTicks(-1);
                DateTime startOfToday     = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfToday       = startOfToday.AddDays(1).AddTicks(-1);

                BsonDocument baseFilter = new()
                {
                    { "deleted",  false },
                    { "plan",     plan },
                    { "company",  company },
                    { "store",    store },
                    { "status",   "Finalizado" }
                };

                // Mês atual
                System.Console.WriteLine(startOfMonth);
                System.Console.WriteLine(endOfMonth);
                BsonDocument thisMonthFilter = baseFilter.DeepClone().AsBsonDocument;
                thisMonthFilter.Add("createdAt", new BsonDocument { { "$gte", startOfMonth }, { "$lte", endOfMonth } });
                BsonDocument? thisMonth = await context.SalesOrders.Aggregate<BsonDocument>(new List<BsonDocument>
                {
                    new("$match", thisMonthFilter),
                    new("$group", new BsonDocument { { "_id", BsonNull.Value }, { "total", new BsonDocument("$sum", "$total") } })
                }).FirstOrDefaultAsync();

                // Mês anterior
                BsonDocument prevMonthFilter = baseFilter.DeepClone().AsBsonDocument;
                prevMonthFilter.Add("createdAt", new BsonDocument { { "$gte", startOfPrevMonth }, { "$lte", endOfPrevMonth } });
                BsonDocument? prevMonth = await context.SalesOrders.Aggregate<BsonDocument>(new List<BsonDocument>
                {
                    new("$match", prevMonthFilter),
                    new("$group", new BsonDocument { { "_id", BsonNull.Value }, { "total", new BsonDocument("$sum", "$total") } })
                }).FirstOrDefaultAsync();

                // Hoje
                BsonDocument todayFilter = baseFilter.DeepClone().AsBsonDocument;
                todayFilter.Add("createdAt", new BsonDocument { { "$gte", startOfToday }, { "$lte", endOfToday } });
                BsonDocument? today = await context.SalesOrders.Aggregate<BsonDocument>(new List<BsonDocument>
                {
                    new("$match", todayFilter),
                    new("$group", new BsonDocument { { "_id", BsonNull.Value }, { "total", new BsonDocument("$sum", "$total") } })
                }).FirstOrDefaultAsync();

                decimal currentTotal  = thisMonth != null ? (decimal)thisMonth["total"].ToDouble() : 0;
                decimal prevTotal     = prevMonth != null ? (decimal)prevMonth["total"].ToDouble() : 0;
                decimal todayTotal    = today     != null ? (decimal)today["total"].ToDouble() : 0;

                decimal growthPercent = prevTotal > 0
                    ? Math.Round((currentTotal - prevTotal) / prevTotal * 100, 1)
                    : currentTotal > 0 ? 100 : 0;

                dynamic result = new
                {
                    currentMonth  = currentTotal,
                    previousMonth = prevTotal,
                    today         = todayTotal,
                    growthPercent,
                };

                return new(result);
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        // ── Últimos pedidos de venda ───────────────────────────────────────────
        public async Task<ResponseApi<dynamic>> GetRecentOrdersAsync(string plan, string company, string store)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted",  false },
                        { "plan",     plan },
                        { "company",  company },
                        { "store",    store }
                    }),
                    new("$sort",  new BsonDocument("createdAt", -1)),
                    new("$limit", 8),
                    MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                    MongoUtil.Lookup("employees", ["$sellerId"],  ["$_id"], "_seller",   [["deleted", false]], 1),
                    MongoUtil.Lookup("users",     ["$sellerId"],  ["$_id"], "_user",     [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "id",           new BsonDocument("$toString", "$_id") },
                        { "customerName", MongoUtil.First("_customer.tradeName") },
                        { "sellerName",   new BsonDocument("$ifNull", new BsonArray
                            {
                                MongoUtil.First("_seller.name"),
                                MongoUtil.First("_user.name")
                            })
                        }
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id",      0 },
                        { "_customer",0 },
                        { "_seller",  0 },
                        { "_user",    0 }
                    })
                };

                List<BsonDocument> results = await context.SalesOrders
                    .Aggregate<BsonDocument>(pipeline)
                    .ToListAsync();

                List<dynamic> list = results
                    .Select(doc => MongoDB.Bson.Serialization.BsonSerializer.Deserialize<dynamic>(doc))
                    .ToList();

                return new(list);
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}