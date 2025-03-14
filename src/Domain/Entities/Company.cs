namespace Domain.Entities;

public class Company
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public List<User> Users { get; set; }
    public List<Resource> Resources { get; set; }

    public Company()
    {
        Users = new List<User>();
        Resources = new List<Resource>();
    }
}