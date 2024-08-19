using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ArmyUtility
{
    static string armyPath = "JSON/Army/";
    static string battallionPath = "JSON/Battalion/";
    static string companyPath = "JSON/Company/";

    static List<BattalionTemplate> battalionTemplates = new List<BattalionTemplate>();
    static List<CompanyTemplate> companyTemplates = new List<CompanyTemplate>();

    public static List<Army> armies = new List<Army>();
    public static List<Battalion> battalions = new List<Battalion>();
    public static List<Company> companies = new List<Company>();

    //LOADERS
    public static void LoadAllCompaniesTemplates()
    {
        string[] guids = AssetDatabase.FindAssets($"t:CompanyTemplate");
        List<string> assetPaths = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            assetPaths.Add(path);
        }

        foreach (string path in assetPaths)
        {
            CompanyTemplate companyTemplate = (CompanyTemplate)AssetDatabase.LoadAssetAtPath(path, typeof(CompanyTemplate));
            companyTemplates.Add(companyTemplate);
        }
    }
    public static void LoadAllBattalionsTemplates()
    {
        string[] guids = AssetDatabase.FindAssets($"t:BattalionTemplate");
        List<string> assetPaths = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            assetPaths.Add(path);
        }

        foreach (string path in assetPaths)
        {
            BattalionTemplate battalionTemplate = AssetDatabase.LoadAssetAtPath<BattalionTemplate>(path);
            battalionTemplates.Add(battalionTemplate);
        }
    }
    public static void LoadAllCompanies()
    {
        Debug.Log("Loading company files.");
        TextAsset[] companyFiles = Resources.LoadAll<TextAsset>(companyPath);

        if (companyFiles == null)
        {
            return;
        }

        foreach (TextAsset t in companyFiles)
        {
            Structure_Company structure_Company = JsonUtility.FromJson<Structure_Company>(t.text);
            Company company = new Company(
                structure_Company.ID,
                structure_Company.name,
                structure_Company.TAG,
                companyTemplates.Find(t => t.name == structure_Company.name)
                );

            companies.Add(company);
        }
    }
    public static void LoadAllBattalions()
    {
        Debug.Log("Loading battalion files.");
        TextAsset[] battallionFiles = Resources.LoadAll<TextAsset>(battallionPath);

        if (battallionFiles == null)
        {
            return;
        }

        foreach (TextAsset t in battallionFiles)
        {
            Structure_Battalion structure_Battallion = JsonUtility.FromJson<Structure_Battalion>(t.text);
            Battalion battalion = new Battalion(
                structure_Battallion.ID,
                structure_Battallion.name,
                structure_Battallion.TAG,
                battalionTemplates.Find(t => t.name == structure_Battallion.template_name)
                );
            //GET COMPANIES
            foreach (int ID in structure_Battallion.companies)
            {
                battalion.companies.Add(companies.Find(c => c.ID == ID));
            }

            battalions.Add(battalion);
        }
    }
    public static void LoadAllArmies()
    {
        Debug.Log("Loading army files.");
        TextAsset[] armyFiles = Resources.LoadAll<TextAsset>(armyPath);

        if (armyFiles == null)
        {
            return;
        }

        Debug.Log(armyFiles.Length + " army files found.");

        foreach(TextAsset t in armyFiles)
        {
            Structure_Army structure_Army = JsonUtility.FromJson<Structure_Army>(t.text);
            Army army = new Army(
                structure_Army.ID,
                structure_Army.name,
                structure_Army.TAG
                );
            //GET BATTALIONS
            foreach(int ID in structure_Army.battalions)
            {
                army.battalions.Add(battalions.Find(b => b.ID == ID));
            }

            armies.Add(army);
        }
    }
    public static void LoadArmy(string armyName)
    {
        Debug.Log("Loading" + armyName + "army file.");
        TextAsset armyFile = Resources.Load<TextAsset>(armyPath + armyName);

        if (armyFile == null)
        {
            return;
        }

        Structure_Army structure_Army = JsonUtility.FromJson<Structure_Army>(armyFile.text);
        Army army = new Army(
            structure_Army.ID,
            structure_Army.name,
            structure_Army.TAG
            );
        //GET BATTALIONS
        foreach (int ID in structure_Army.battalions)
        {
            army.battalions.Add(battalions.Find(b => b.ID == ID));
        }

        armies.Add(army);
    }

    //BUILDERS
    public static void CreateBattalionFile(int ID, string name, string TAG, string template_name)
    {
        Battalion battalion = new Battalion(ID, name, TAG, GetBattalionTemplate(template_name));
        string battalionJson = JsonUtility.ToJson(battalion);

        File.WriteAllText(battallionPath + name + ".json", battalionJson);
    }

    //GETTERS
    public static BattalionTemplate GetBattalionTemplate(string template_name)
    {
        return battalionTemplates.Find(t => t.name == template_name);
    }
    public static CompanyTemplate GetCompanyTemplate(string template_name)
    {
        return companyTemplates.Find(t => t.name == template_name);
    }
}