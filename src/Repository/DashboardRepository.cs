using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
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
                DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth = startOfMonth.AddMonths(1);
                DateTime startOfPrevMonth = startOfMonth.AddMonths(-1);

                var filterSalesMonth = Builders<SalesOrder>.Filter.Gte(x => x.CreatedAt, startOfMonth);

                filterSalesMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filterSalesMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                filterSalesMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);

                if (store != "all") filterSalesMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);

                var filterSalesPrevMonth = Builders<SalesOrder>.Filter.Gte(x => x.CreatedAt, startOfPrevMonth);

                if (store != "all") filterSalesPrevMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);
                filterSalesPrevMonth &= Builders<SalesOrder>.Filter.Lte(x => x.CreatedAt, startOfMonth);
                filterSalesPrevMonth &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);


                var salesMonth = await context.SalesOrders.Find(filterSalesMonth).Project(x => new { x.Total, x.Status }).ToListAsync();
                var salesPrevMonth = await context.SalesOrders.Find(filterSalesPrevMonth).Project(x => new { x.Total }).ToListAsync();
                decimal totalSales = salesMonth.Sum(s => s.Total);
                decimal prevTotalSales = salesPrevMonth.Sum(s => s.Total);

                var stockData = await context.Products.Find(_ => true).Project(x => new { x.PriceTotal, x.QuantityStock }).ToListAsync();

                var filterCustMonth = Builders<Customer>.Filter.Gte(x => x.CreatedAt, startOfMonth);
                filterCustMonth &= Builders<Customer>.Filter.Eq(x => x.Deleted, false);
                filterCustMonth &= Builders<Customer>.Filter.Eq(x => x.Plan, plan);
                filterCustMonth &= Builders<Customer>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterCustMonth &= Builders<Customer>.Filter.Eq(x => x.Store, store);

                var custMonth = await context.Customers.CountDocumentsAsync(filterCustMonth);
                
                var filterPrevMonth = Builders<Customer>.Filter.Gte(x => x.CreatedAt, startOfPrevMonth);
                filterPrevMonth &= Builders<Customer>.Filter.Lte(x => x.CreatedAt, startOfMonth);
                filterPrevMonth &= Builders<Customer>.Filter.Eq(x => x.Deleted, false);
                filterPrevMonth &= Builders<Customer>.Filter.Eq(x => x.Plan, plan);
                filterPrevMonth &= Builders<Customer>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterPrevMonth &= Builders<Customer>.Filter.Eq(x => x.Store, store);

                var custPrevMonth = await context.Customers.CountDocumentsAsync(filterPrevMonth);
                
                var filterReceivable = Builders<AccountReceivable>.Filter.Gte(x => x.CreatedAt, startOfMonth);
                filterReceivable &= Builders<AccountReceivable>.Filter.Lt(x => x.DueDate, endOfMonth);
                filterReceivable &= Builders<AccountReceivable>.Filter.Eq(x => x.Deleted, false);
                filterReceivable &= Builders<AccountReceivable>.Filter.Eq(x => x.Plan, plan);
                filterReceivable &= Builders<AccountReceivable>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterReceivable &= Builders<AccountReceivable>.Filter.Eq(x => x.Store, store);

                List<AccountReceivable> accountsReceivable = await context.AccountsReceivable.Find(filterReceivable).ToListAsync();
                
                var filterPayable = Builders<AccountPayable>.Filter.Gte(x => x.CreatedAt, startOfMonth);
                filterPayable &= Builders<AccountPayable>.Filter.Lt(x => x.DueDate, endOfMonth);
                filterPayable &= Builders<AccountPayable>.Filter.Eq(x => x.Deleted, false);
                filterPayable &= Builders<AccountPayable>.Filter.Eq(x => x.Plan, plan);
                filterPayable &= Builders<AccountPayable>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterPayable &= Builders<AccountPayable>.Filter.Eq(x => x.Store, store);
                List<AccountPayable> accountsPayable = await context.AccountsPayable.Find(filterPayable).ToListAsync();


                dynamic obj = new
                {
                    sales = new {
                        totalMonth = (double)totalSales,
                        countMonth = salesMonth.Count,
                        growthPercent = CalculateGrowth(totalSales, prevTotalSales),
                        openOrders = salesMonth.Count(x => x.Status != "Finalizado")
                    },
                    stock = new {
                        totalValue = (double)stockData.Sum(x => x.PriceTotal),
                        totalItems = (int)stockData.Sum(x => x.QuantityStock)
                    },
                    customers = new {
                        countMonth = (int)custMonth,
                        growthPercent = CalculateGrowth(custMonth, custPrevMonth)
                    },
                    accountsReceivable = new 
                    { 
                        openAmount = accountsReceivable.Where(x => x.Status == "open").Sum(x => x.Amount), 
                        openCount = accountsReceivable.Where(x => x.Status == "open").Count(), 
                        overdueAmount = accountsReceivable.Where(x => x.DueDate.Date < DateTime.UtcNow.Date && x.Status == "open").Sum(x => x.Amount),
                        overdueCount = accountsReceivable.Where(x => x.DueDate.Date < DateTime.UtcNow.Date && x.Status == "open").Count(), 
                        totalAmount = accountsReceivable.Where(x => true).Sum(x => x.Amount), 
                        totalCount = accountsReceivable.Count
                    },
                    accountsPayable = new 
                    { 
                        openAmount = accountsPayable.Where(x => x.Status == "open").Sum(x => x.Amount), 
                        openCount = accountsPayable.Where(x => x.Status == "open").Count(), 
                        overdueAmount = accountsPayable.Where(x => x.DueDate.Date < DateTime.UtcNow.Date && x.Status == "open").Sum(x => x.Amount),
                        overdueCount = accountsPayable.Where(x => x.DueDate.Date < DateTime.UtcNow.Date && x.Status == "open").Count(), 
                        totalAmount = accountsPayable.Where(x => true).Sum(x => x.Amount), 
                        totalCount = accountsPayable.Count
                    }
                };

                return new ResponseApi<dynamic>(obj);
            }
            catch { return new ResponseApi<dynamic>(null, 500, "Erro ao carregar cards."); }
        }

        public async Task<ResponseApi<dynamic>> GetRecentOrdersAsync(string plan, string company, string store)
        {
            try
            {
                var filter = Builders<SalesOrder>.Filter.Eq(x => x.CreatedAt.Date, DateTime.UtcNow.Date);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                if (store != "all") filter &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);

                var recent = await context.SalesOrders.Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .Limit(3)
                    .ToListAsync();

                var result = recent.Select(x => new {
                    id = x.Id,
                    code = x.Code,
                    customerName = "", 
                    sellerName = "", 
                    total = (double)x.Total,
                    status = x.Status,
                    createdAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

                return new ResponseApi<dynamic>(result);
            }
            catch 
            { 
                return new ResponseApi<dynamic>(null, 500, "Erro ao carregar pedidos recentes."); 
            }
        }

        public async Task<ResponseApi<dynamic>> GetMonthlySalesAsync(string plan, string company, string store)
        {
            try
            {
                var year = DateTime.UtcNow.Year;
                var totals = new double[12];
                var counts = new int[12];

                var filter = Builders<SalesOrder>.Filter.Eq(x => x.CreatedAt.Year, year);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                if (store != "all") filter &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);

                var salesYear = await context.SalesOrders.Find(filter).Project(x => new { x.Total, x.CreatedAt }).ToListAsync();

                for (int i = 1; i <= 12; i++) {
                    var monthSales = salesYear.Where(s => s.CreatedAt.Month == i).ToList();
                    totals[i - 1] = (double)monthSales.Sum(s => s.Total);
                    counts[i - 1] = monthSales.Count;
                }
                return new ResponseApi<dynamic>(new { totals, counts });
            }
            catch { return new ResponseApi<dynamic>(null, 500, "Erro ao carregar vendas mensais."); }
        }

        public async Task<ResponseApi<dynamic>> GetMonthlyTargetAsync(string plan, string company, string store)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime startM = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime startP = startM.AddMonths(-1);

                var filter = Builders<SalesOrder>.Filter.Gte(x => x.CreatedAt, startM);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filter &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                if (store != "all") filter &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);
                
                var curS = await context.SalesOrders.Find(filter).Project(x => x.Total).ToListAsync();

                var filterPre = Builders<SalesOrder>.Filter.Gte(x => x.CreatedAt, startP);
                filterPre &= Builders<SalesOrder>.Filter.Lt(x => x.CreatedAt, startM);
                filterPre &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);
                filterPre &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filterPre &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterPre &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);

                var preS = await context.SalesOrders.Find(filterPre).Project(x => x.Total).ToListAsync();

                var filterTod = Builders<SalesOrder>.Filter.Gte(x => x.CreatedAt, now.Date);
                filterTod &= Builders<SalesOrder>.Filter.Eq(x => x.Deleted, false);
                filterTod &= Builders<SalesOrder>.Filter.Eq(x => x.Plan, plan);
                filterTod &= Builders<SalesOrder>.Filter.Eq(x => x.Company, company);
                if (store != "all") filterTod &= Builders<SalesOrder>.Filter.Eq(x => x.Store, store);
                var todS = await context.SalesOrders.Find(filterTod).Project(x => x.Total).ToListAsync();

                decimal cur = curS.Sum();
                decimal pre = preS.Sum();

                return new ResponseApi<dynamic>(new {
                    currentMonth = (double)cur,
                    previousMonth = (double)pre,
                    today = (double)todS.Sum(),
                    growthPercent = CalculateGrowth(cur, pre)
                });
            }
            catch { return new ResponseApi<dynamic>(null, 500, "Erro ao carregar metas."); }
        }

        private static double CalculateGrowth(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return (double)Math.Round(((current - previous) / previous) * 100, 2);
        }
    }
}