namespace PharmaWarehouse.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} با شناسه {key} پیدا نشد.") { }
}