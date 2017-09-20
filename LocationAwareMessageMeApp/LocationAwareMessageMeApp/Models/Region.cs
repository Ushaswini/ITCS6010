namespace LocationAwareMessageMeApp.Models
{
    public class Region
    {
        public string RegionName { get; set; }
        public string RegionId { get; set; }

        public override string ToString()
        {
            return RegionName;
        }
    }
}