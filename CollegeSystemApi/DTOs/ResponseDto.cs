using System.Collections.Generic;
using System.Text;

namespace CollegeSystemApi.DTOs;

public  class ResponseDto<T> where T : class?
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    // Backing field for data
    private object? _data;

    // Property to get or set data as a single item or a list
    public object? Data
    {
        get => _data;
        set
        {
            if (value is T || value is List<T> || value == null)
            {
                _data = value;
            }
            else
            {
                throw new ArgumentException("Data must be of type T or List<T>.");
            }
        }
    }

    // Override ToString to provide a meaningful representation
    public override string ToString()
    {
        var sb = new StringBuilder();

        // Append StatusCode if it has a value
        sb.AppendLine($"StatusCode: {StatusCode}");

        // Append Message only if it's not null or empty
        if (!string.IsNullOrEmpty(Message))
        {
            sb.AppendLine($"Message: {Message}");
        }

        // Append Data only if it's not null
        if (_data != null)
        {
            if (_data is T singleItem)
            {
                sb.AppendLine("Data (Single Item):");
                sb.AppendLine(singleItem.ToString());
            }
            else if (_data is List<T> list)
            {
                sb.AppendLine("Data (List):");
                foreach (var item in list)
                {
                    sb.AppendLine(item?.ToString());
                }
            }
        }

        return sb.ToString();
    }
}