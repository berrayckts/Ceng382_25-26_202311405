using Northwind.Mvc.Models;

namespace Northwind.Mvc.Services;

public interface IPdfService
{
    Task<byte[]> CreateReceiptAsync(int orderId);
    Task<byte[]> CreateAgreementAsync(int orderId);
}
