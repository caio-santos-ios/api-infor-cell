using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using MongoDB.Driver;

namespace api_infor_cell.src.Repository
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        public async Task<ResponseApi<dynamic>> GetCardsAsync(string startDate, string endDate, string storeId)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth = startOfMonth.AddMonths(1);
                DateTime startOfPrevMonth = startOfMonth.AddMonths(-1);

                var salesMonth = await context.SalesOrders.Find(x => x.CreatedAt >= startOfMonth).Project(x => new { x.Total, x.Status }).ToListAsync();
                var salesPrevMonth = await context.SalesOrders.Find(x => x.CreatedAt >= startOfPrevMonth && x.CreatedAt < startOfMonth).Project(x => new { x.Total }).ToListAsync();
                decimal totalSales = salesMonth.Sum(s => s.Total);
                decimal prevTotalSales = salesPrevMonth.Sum(s => s.Total);

                var stockData = await context.Products.Find(_ => true).Project(x => new { x.PriceTotal, x.QuantityStock }).ToListAsync();

                var custMonth = await context.Customers.CountDocumentsAsync(x => x.CreatedAt >= startOfMonth);
                var custPrevMonth = await context.Customers.CountDocumentsAsync(x => x.CreatedAt >= startOfPrevMonth && x.CreatedAt < startOfMonth);
                
                List<AccountReceivable> accountsReceivable = await context.AccountsReceivable.Find(x => x.CreatedAt >= startOfMonth && !x.Deleted && x.DueDate < endOfMonth).ToListAsync();
                List<AccountPayable> accountsPayable = await context.AccountsPayable.Find(x => x.CreatedAt >= startOfMonth && !x.Deleted && x.DueDate < endOfMonth).ToListAsync();


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

        public async Task<ResponseApi<dynamic>> GetRecentOrdersAsync(string startDate, string endDate, string storeId)
        {
            try
            {
                var recent = await context.SalesOrders.Find(x => !x.Deleted && x.CreatedAt.Date == DateTime.UtcNow.Date)
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

        public async Task<ResponseApi<dynamic>> GetMonthlySalesAsync(string startDate, string endDate, string storeId)
        {
            try
            {
                var year = DateTime.UtcNow.Year;
                var totals = new double[12];
                var counts = new int[12];
                var salesYear = await context.SalesOrders.Find(x => x.CreatedAt.Year == year).Project(x => new { x.Total, x.CreatedAt }).ToListAsync();

                for (int i = 1; i <= 12; i++) {
                    var monthSales = salesYear.Where(s => s.CreatedAt.Month == i).ToList();
                    totals[i - 1] = (double)monthSales.Sum(s => s.Total);
                    counts[i - 1] = monthSales.Count;
                }
                return new ResponseApi<dynamic>(new { totals, counts });
            }
            catch { return new ResponseApi<dynamic>(null, 500, "Erro ao carregar vendas mensais."); }
        }

        public async Task<ResponseApi<dynamic>> GetMonthlyTargetAsync(string startDate, string endDate, string storeId)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime startM = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime startP = startM.AddMonths(-1);

                var curS = await context.SalesOrders.Find(x => x.CreatedAt >= startM).Project(x => x.Total).ToListAsync();
                var preS = await context.SalesOrders.Find(x => x.CreatedAt >= startP && x.CreatedAt < startM).Project(x => x.Total).ToListAsync();
                var todS = await context.SalesOrders.Find(x => x.CreatedAt >= now.Date).Project(x => x.Total).ToListAsync();

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