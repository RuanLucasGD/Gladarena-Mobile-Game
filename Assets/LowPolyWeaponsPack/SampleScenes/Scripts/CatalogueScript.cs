using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogueScript : MonoBehaviour
{
    public GameObject Weapon;
    
    public Dropdown TypeDropdown;
    public Dropdown WeaponDropdown;
    public Dropdown MaterialDropdown;
    public Slider LightLevelSlider;
    public Light DirectionalLight;

    public List<Mesh> Axes = new List<Mesh>();
    public List<Mesh> BowsAndArrows = new List<Mesh>();
    public List<Mesh> Clubs = new List<Mesh>();
    public List<Mesh> Daggers = new List<Mesh>();
    public List<Mesh> Hammers = new List<Mesh>();
    public List<Mesh> Maces = new List<Mesh>();
    public List<Mesh> Shields = new List<Mesh>();
    public List<Mesh> SpearsAndHalberds = new List<Mesh>();
    public List<Mesh> Staffs = new List<Mesh>();
    public List<Mesh> Swords = new List<Mesh>();
    public List<Mesh> Wands = new List<Mesh>();

    List<Mesh> CurrentWeapons = new List<Mesh>();

    public List<Material> materials = new List<Material>();

    public void SetLight()
    {
        DirectionalLight.intensity = LightLevelSlider.value;
    }

    public void ResetLight()
    {
        LightLevelSlider.value = 1;
    }

    public void SetType()
    {
        WeaponDropdown.options.Clear();
        switch (TypeDropdown.value)
        {
            case 0: CurrentWeapons = Axes;                
                break;
            case 1: CurrentWeapons = BowsAndArrows;
                break;
            case 2: CurrentWeapons = Clubs;
                break;
            case 3: CurrentWeapons = Daggers;
                break;
            case 4: CurrentWeapons = Hammers;
                break;
            case 5: CurrentWeapons = Maces;
                break;
            case 6: CurrentWeapons = Shields;
                break;
            case 7: CurrentWeapons = SpearsAndHalberds;
                break;
            case 8: CurrentWeapons = Staffs;
                break;
            case 9: CurrentWeapons = Swords;
                break;
            case 10: CurrentWeapons = Wands;
                break;                            
        }
        
        for (int i = 0; i < CurrentWeapons.Count; i++)
            WeaponDropdown.options.Add(new Dropdown.OptionData(CurrentWeapons[i].name));
        
        WeaponDropdown.value = 0;
        WeaponDropdown.RefreshShownValue();
        SetMesh();
    }

    public void SetMesh()
    {
        Weapon.GetComponent<MeshFilter>().sharedMesh = CurrentWeapons[WeaponDropdown.value];
    }

    public void SetMaterial()
    {
        Weapon.GetComponent<MeshRenderer>().material = materials[MaterialDropdown.value];
    }

    // Start is called before the first frame update
    void Start()
    {
        SetType();

        MaterialDropdown.options.Clear();
        for (int i = 0;i<materials.Count;i++)
            MaterialDropdown.options.Add(new Dropdown.OptionData(materials[i].name));
        MaterialDropdown.RefreshShownValue();
    }

    public void ResetCamera()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.transform.Translate(Vector3.forward *
                Input.mouseScrollDelta.y * Time.deltaTime * 50);
        }

        if (Input.GetMouseButton(2))
        {
            if (Input.GetAxis("Mouse X") != 0)
                Camera.main.transform.Translate(Vector3.left * Input.GetAxis("Mouse X") * 25 * Time.deltaTime);
            if (Input.GetAxis("Mouse Y") != 0)
                Camera.main.transform.Translate(Vector3.up * Input.GetAxis("Mouse Y") * -25 * Time.deltaTime);
        }

        if (Input.GetMouseButton(1))
        {
            if (Input.GetAxis("Mouse X") != 0)
                Camera.main.transform.RotateAround(Weapon.transform.position,
                    Vector3.up, Input.GetAxis("Mouse X") * 125 * Time.deltaTime);
            if (Input.GetAxis("Mouse Y") != 0)
                Camera.main.transform.RotateAround(Weapon.transform.position,
                    Vector3.left, Input.GetAxis("Mouse Y") * 125 * Time.deltaTime);
        }

    }
}
