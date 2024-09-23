using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;

public class Army
{
    public int ID;
    public string TAG = "FRA";
    public string name;

    private const int reinforcementPerLevel = 20;

    public List<Battalion> battalions = new List<Battalion>();

    public Army(int ID, string name, string TAG)
    {
        this.ID = ID;
        this.name = name;
        this.TAG = TAG;
    }

    public Army(string TAG)
    {
        this.TAG = TAG;
        WorldUtility.AppendArmy(this);

        Initialize();
    }

    private void Initialize()
    {
        //GENERATE ARMY NAME
        armyName = TAG + ID;
    }

    public int GetArmySize()
    {
        int s = 0;
        foreach(Battalion b in battalions)
        {
            foreach(Company c in b.companies)
            {
                s += c.size;
            }
        }
        return s;
    }

    public bool CanReinforce()
    {
        //TODO: add reinforcement logic
        return true;
    }
    public void Reinforce(int reinforcementLVL)
    {
        if(CanReinforce())
        {
            foreach(Battalion b in battalions)
            {
                foreach (Company c in b.companies)
                {
                    c.Reinforce(reinforcementLVL * reinforcementPerLevel);
                }
            }
        }
    }
}

public class Battalion
{
    public int ID;
    public string name;
    public string TAG;

    public BattalionTemplate template;

    public List<Company> companies = new List<Company>();

    public bool Deployed;
    public int HomeBarrack;

    public Battalion(int ID, string name, string TAG, BattalionTemplate template)
    {
        this.ID = ID;
        this.name = name;
        this.TAG = TAG;
        this.template = template;
    }

    public void Initialize(BattalionTemplate bt)
    {
        template = bt;
    }

    public UnitType GetBattalionType()
    {
        return template.type;
    }
}

public class Company
{
    public CompanyTemplate template;

    public int ID;
    public string name;
    public string TAG = "FRA";

    public int size;
    public int experience_LVL;
    public int experience;

    public Company(int ID, string name, string TAG, CompanyTemplate template)
    {
        this.ID = ID;
        this.name = name;
        this.TAG = TAG;
        this.template = template;
    }

    public void Initialize(CompanyTemplate companyTemplate)
    {
        template = companyTemplate;

        size = template.CompanySize;
        experience_LVL = 1;
        experience = 0;
    }

    public UnitType GetCompanyType()
    {
        return template.type;
    }

    public void Reinforce(int reinforcement)
    {
        if(size != template.CompanySize)
        {
            size += template.CompanySize;
            if(size > template.CompanySize)
            {
                size = template.CompanySize;
            }
        }
    }
}