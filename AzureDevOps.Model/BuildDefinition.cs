namespace AzureDevOps.Model
{
    public class BuildDefinition
    {
        public int Id { get; set; }

        public BuildDefinition(int id)
        {
            this.Id = id;
        }
    }
}
