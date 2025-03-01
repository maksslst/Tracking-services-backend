namespace Domain.Entities;

public class Company
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public List<User> Users { get; set; }
    public List<Service> Services { get; set; }

    public Company()
    {
        Users = new List<User>();
        Services = new List<Service>();
    }
}