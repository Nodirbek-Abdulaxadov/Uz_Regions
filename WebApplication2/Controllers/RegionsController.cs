[Route("api/[controller]")]
[ApiController]
public class RegionsController(Database database,
                               IDistributedCache cache) 
    : ControllerBase
{
    private List<Region> regions = new();
    private List<District> districts = new();
    private List<Quarter> quarters = new();
    private readonly Database _database = database;
    private readonly IDistributedCache _cache = cache;

    [HttpGet("regions")]
    public async Task<IActionResult> GetRegions()
    {
        var cachedData = _cache.GetString("regions");
        if (cachedData != null && cachedData != "[]")
        {
            return Ok(cachedData);
        }

        var fromDatabase = _database.Regions.Find(_ => true).ToList();

        if (fromDatabase.Count != 0)
        {
            await _cache.SetStringAsync("regions", JsonConvert.SerializeObject(fromDatabase)).ConfigureAwait(false);
            return Ok(fromDatabase);
        }
        ReadFromFile();
        List<Region> result = new();
        foreach (var region in regions)
        {
            region.districts = districts.Where(d => d.region_id == region.id).ToList();
            foreach (var district in region.districts)
            {
                district.Quarters = quarters.Where(q => q.district_id == district.id).ToList();
            }
            result.Add(region);
        }

        await _cache.SetStringAsync("regions", JsonConvert.SerializeObject(result)).ConfigureAwait(false);
        await _database.Regions.InsertManyAsync(result).ConfigureAwait(false);
        return Ok(result);
    }

    private void ReadFromFile()
    {
        regions = JsonConvert.DeserializeObject<List<Region>>(System.IO.File.ReadAllText("StaticData/Regions.json"))!;
        districts = JsonConvert.DeserializeObject<List<District>>(System.IO.File.ReadAllText("StaticData/Districts.json"))!;
        quarters = JsonConvert.DeserializeObject<List<Quarter>>(System.IO.File.ReadAllText("StaticData/Villages.json"))!;
    }
}