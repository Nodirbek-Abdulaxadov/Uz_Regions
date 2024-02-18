public class Region
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public List<District> districts { get; set; } = new();   
}

public class District
{
    public int id { get; set; }
    public int region_id { get; set; }
    public string name { get; set; } = string.Empty;
    public List<Quarter> Quarters { get; set; } = new();
}

public class Quarter
{
    public int id { get; set; }
    public int district_id { get; set; }
    public string name { get; set; } = string.Empty;
}
