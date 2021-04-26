[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] Items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key { get; set; }
    public string value { get; set; }

    public LocalizationItem(string _key, string _value)
    {
        key = _key;
        value = _value;
    }
}
