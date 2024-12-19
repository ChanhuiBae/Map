using System;

public struct Appearance
{
    public string Head;
    public string Ears;
    public string Eyes;
    public string Body;
    public string Hair;
    public string Armor;
    public string Helmet;
    public string Weapon;
    public string Firearm;
    public string Shield;
    public string Cape;
    public string Back;
    public string Mask;
    public string Horns;
}
public class CharacterData
{
    private int id;
    public string name;
    public Appearance appearance;
    private float maxHP;
    public float MaxHP
    {
        get => maxHP;
    }
    private float currentHP;
    public float HP
    {
        get => currentHP;
        set => currentHP += value;
    }
    private int satiety;
    public int Satiety
    {
        get => satiety;
        set => satiety += value;
    }
    private int fatigue;
    public int Fatigue
    {
        get => fatigue;
        set => fatigue += value;
    }

    public CharacterData(int ID)
    {
        id = ID;
        name = "name" + id;
        appearance = new Appearance();
        appearance.Head = "Human";
        appearance.Ears = "Human";
        appearance.Eyes = "Human";
        appearance.Body = "Human";
        appearance.Hair = "Hair2";
        appearance.Weapon = "ShordDagger";
        maxHP = 100;
        currentHP = maxHP;
        satiety = 100;
        fatigue = 100;
    }
}
