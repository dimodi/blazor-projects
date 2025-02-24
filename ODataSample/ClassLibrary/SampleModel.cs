namespace ClassLibrary;

public class SampleModel
{
    public int Id { get; set; }
    public string StringProperty { get; set; } = string.Empty;
    public int IntProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
    public DateOnly DateOnlyProperty { get; set; }
    public TimeOnly TimeOnlyProperty { get; set; }
    public bool BoolProperty { get; set; }
}
