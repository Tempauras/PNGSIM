using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person
{
    private string _name;
    private int _age;
    private List<Trait> _traits = new List<Trait>();
    private Sprite _sprite;

    public string Name { get => _name; }
    public int Age { get => _age; }
    public List<Trait> Traits { get => _traits; }
    public Sprite Sprite { get => _sprite; }


    public Person(string Name, int Age, List<Trait> Traits)
    {
        _name = Name;
        _age = Age;
        _traits = Traits;
        GenerateSprite();
    }



    private void GenerateSprite()
    {
        //Todo Generate sprite code
    }
}
