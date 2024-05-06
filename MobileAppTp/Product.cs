using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public string Title { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public string MainPicture { get; set; }
    public string[] Pictures { get; set; }
    public DateOnly PublishDate { get; set; }
    public int Quantity { get; set; }
}