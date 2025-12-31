namespace api_infor_cell.src.Shared.DTOs
{
public class UpdateAddressDTO
{
    public string Id { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string Neighborhood { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Complement { get; set; } = string.Empty;

    public string ParentId { get; set; } = string.Empty;
    
    public string Parent { get; set; } = string.Empty;
}
}