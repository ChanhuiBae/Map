public enum LightType
{
    Intensity = 0,
    Weakness = 15,
    Faint = 30,
    None = 50
}
public class ExplorationData
{
    public MapData mapData;
    private bool exploring;
    public bool Exploring
    {
        get => exploring;
    }

    public static int maxLatern = 150;
    private int lantern;
    public CharacterData[] characters;
    public int Lattern
    {
        get => lantern;
        set
        {
            lantern += value;
            if (lantern > maxLatern)
                lantern = maxLatern;
            else if(lantern < 0)
                lantern = 0;
        }
    }

    public LightType GetLightType()
    {
        if (lantern == 0)
            return LightType.None;
        else if (lantern < 51)
            return LightType.Faint;
        else if (lantern < 101)
            return LightType.Weakness;
        else
            return LightType.Intensity;
    }

    public Bag bag;

    #region »ý¼ºÀÚ
    public ExplorationData()
    {
        exploring = false;
        mapData = new MapData();
    }
    public ExplorationData(MapData mapData)
    {
        exploring = true;
        lantern = maxLatern;
        this.mapData = mapData;
        this.mapData.CreateMap();
        characters = new CharacterData[4];
        bag = new Bag();
    }
    #endregion
}
