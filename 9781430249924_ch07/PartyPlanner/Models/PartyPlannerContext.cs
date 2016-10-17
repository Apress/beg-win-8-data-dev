using System.Data.Entity;

namespace PartyPlanner.Models
{
public class PartyPlannerContext : DbContext
{        
    public PartyPlannerContext() : base("name=PartyPlannerContext")
    {
    }

    public DbSet<Party> Parties { get; set; }
}
}
